using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.AliPay;
using Unitoys.Core.JiGuang;
using Unitoys.Eims;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;
using WxPayAPI;


namespace Unitoys.WebApi.Controllers
{
    public class EimsController : ApiController
    {
        private ISMSService _smsService;
        private IUserService _userService;
        private IDeviceGoipService _deviceGoipService;
        private IEjoinDevService _ejoinDevService;
        private IEjoinDevSlotService _ejoinDevSlotService;
        public EimsController(ISMSService smsService, IUserService userService, IDeviceGoipService deviceGoipService, IEjoinDevService ejoinDevService, IEjoinDevSlotService ejoinDevSlotService)
        {
            this._smsService = smsService;
            this._userService = userService;
            this._deviceGoipService = deviceGoipService;
            this._ejoinDevService = ejoinDevService;
            this._ejoinDevSlotService = ejoinDevSlotService;
        }

        #region 1.SMS
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="tid">任务id</param>
        /// <param name="devname">设备名称</param>
        /// <param name="port">
        /// 发送端口
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </param>
        /// <param name="iccid">一个或多个（逗号分隔）发送sim卡的iccid</param>
        /// <param name="to">短信接收者号码
        /// 一个或多个（逗号连接）短信接收者号码 
        /// </param>
        /// <param name="SMSContent">短信内容</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SMSSend(AddSMSBindingModel model)
        {
            LoggerHelper.Info("发送短信");
            EimsApi api = new EimsApi();
            model.TId = CommonHelper.GetDateTimeInt();

            List<SMSTaskModel> list = new List<SMSTaskModel>()
            {
                new  SMSTaskModel()
                {
                tid=model.TId,
                devname=model.DevName,
                port=model.Port,
                iccid=model.IccId,
                to=model.To,
                sms=model.SMSContent,
                sdr=null,//是否开启短信成功状态报告
                }
            };

            var currentUser = WebUtil.GetApiUserSession();



            UT_SMS entity = new UT_SMS()
            {
                UserId = currentUser.ID,
                DevName = model.DevName,
                Fm = currentUser.Tel,
                To = model.To,
                SMSTime = CommonHelper.GetDateTimeInt(),
                SMSContent = model.SMSContent,
                Status = SMSStatusType.Int,
                IsSend = true,
                Port = model.Port,
                IccId = model.IccId,
                TId = model.TId,
                IsRead = true
            };

            //TODO. 发送短信
            //可能出错地方，传递了参数值为null
            if (await _smsService.InsertAsync(entity))
            {
                try
                {
                    var result = await api.SMSSend(list);
                    if (result.status.Length > 0)
                    {
                        if (result.status[0].status.Contains('0'))
                        {
                            return Ok(new
                            {
                                status = 1,
                                data = new
                                {
                                    tid = model.TId
                                }
                            });
                        }
                        else
                        {
                            return Ok(new { status = 0, msg = result.status[0].status });
                        }
                    }
                }
                catch (Exception ex)
                {
                    entity.Status = SMSStatusType.Error;
                    entity.ErrorMsg = "短信服务端调用异常：" + ex.Message;
                    _smsService.UpdateAsync(entity);
                    return Ok(new { status = 0, msg = "短信服务端调用异常：" + ex.Message });
                }
            }
            else
            {
                return Ok(new
                {
                    status = 0,
                    msg = "失败",
                    data = new
                    {
                        tid = model.TId
                    }
                });
            }
            return Ok(new
            {
                status = 0,
                msg = "失败",
                data = new
                {
                    tid = model.TId
                }
            });
        }

        /// <summary>
        /// 状态报告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rpt_num"></param>
        /// <param name="rpts"></param>
        /// <returns></returns>
        [NoAuthenticate]
        public async Task<IHttpActionResult> SMSReport()
        {
            LoggerHelper.Info("短信状态报告");
            LoggerHelper.Info("短信：" + await Request.Content.ReadAsStringAsync());
            //从body中获取内容
            var model = await Request.Content.ReadAsAsync<TranslateEimsSMSReportQueryModel>();
            LoggerHelper.Info(string.Format(model.rpts.tid + "短信状态报告type：{0},rpt_num:{1},sdrCount:{2},fdrCount:{3}", model.type, model.rpt_num, model.rpts.sdr.Count, model.rpts.fdr.Count));

            bool result = false;

            if (model.type == "status-report" && model.rpt_num > 0)
            {
                //如果有未发送短信数
                if (model.rpts.unsent > 0)
                {

                }

                JPushApi j = new JPushApi();

                //一个号码一个描述
                foreach (var item in model.rpts.sdr)
                {
                    LoggerHelper.Info(string.Format(model.rpts.tid + "sdr短信状态报告,号码索引：{0},号码：{1},短信发送设备和端口：{2},短信发送sim卡的iccid：{3},短信发送时UTC时间戳：{4}", item[0], item[1], item[2], item[3], item[4]));

                    var to = item[1];

                    var entity = await _smsService.GetEntityAsync(x => x.TId == model.rpts.tid);
                    if (entity != null)
                    {
                        entity.Status = SMSStatusType.Success;
                        if (await _smsService.UpdateAsync(entity))
                        {

                            j.Push_all_alias_message("aixiaoqi" + entity.Fm, "短信发送成功", "SMSSendResult", new Dictionary<string, string>()
                                {
                                    {"Tel",entity.To},
                                    {"Status",(int)entity.Status + ""},
                                    {"SMSID",entity.ID.ToString()}
                                });
                        }
                    }
                }
                foreach (var item in model.rpts.fdr)
                {
                    LoggerHelper.Info(string.Format(model.rpts.tid + "fdr短信状态报告,号码索引：{0},号码：{1},短信发送设备和端口：{2},短信发送sim卡的iccid：{3},短信发送时UTC时间戳：{4}", item[0], item[1], item[2], item[3], item[4]));

                    var to = item[1];

                    var entity = await _smsService.GetEntityAsync(x => x.TId == model.rpts.tid);
                    if (entity != null)
                    {
                        entity.Status = SMSStatusType.Error;
                        result = await _smsService.UpdateAsync(entity);

                        j.Push_all_alias_message("aixiaoqi" + entity.Fm, "短信发送失败", "SMSSendResult", new Dictionary<string, string>()
                                {
                                    {"Tel",entity.To},
                                    {"Status",(int)entity.Status + ""},
                                    {"SMSID",entity.ID.ToString()}
                                });
                    }
                }
            }
            return Ok(new { status = 1, msg = "" });
        }

        /// <summary>
        /// 查询短信发送状态
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="devname"></param>
        /// <param name="port"></param>
        /// <param name="iccid"></param>
        /// <param name="to"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SMSReportQuery([FromBody]SMSReportQueryBindingModels model)
        {
            EimsApi api = new EimsApi();
            //1.查询数据库短信表

            //2.如果表未更新，则查询Api
            var result = await api.SMSReportQuery(model.TId, model.DevName, model.Port, model.IccId);
            if (result.rpt_num > 0)
            {
                return Ok(new
                {
                    status = 1,
                    rpt_num = result.rpt_num,
                    data = new
                    {
                        rpts = result.rpts
                    }
                });
            }
            else
            {
                return Ok(new { status = 1, msg = "rpt_num:" + result.rpt_num });
            }
            //return Ok(new
            //{
            //    status = 0,
            //    data = new
            //    {
            //        tid = tid
            //    }
            //});
        }

        /// <summary>
        /// 短信查询
        /// 客户可以通过HTTP的GET请求主动查询EIMS接收到的SMS。
        /// </summary>
        /// <param name="sms_num">
        /// 指定要查询的短信数
        /// 0：表示查询所有短信
        /// </param>
        /// <param name="start">
        /// 查询开始的时间戳
        /// 若是这个为空，默认查询前10分钟收到的短信
        /// </param>
        /// <param name="devname">设备名称</param>
        /// <param name="port">
        /// 端口号
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </param>
        /// <param name="iccid">
        /// sim卡唯一标识
        /// 一个或多个（逗号分隔）发送sim卡的iccid
        /// </param>
        /// <returns></returns>
        public async Task<IHttpActionResult> SMSQuery([FromBody]SMSQueryBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            //查询短信
            var result = await new EimsApi().SMSQuery(model.sms_num, (int)model.start, model.DevName, model.Port, model.IccId);

            if (result.sms_num > 0)
            {
                List<UT_SMS> smsList = new List<UT_SMS>();
                var groupSMS = result.data.GroupBy(x => x[5]);

                foreach (var item in result.data)
                {
                    UT_SMS entity = new UT_SMS()
                    {
                        UserId = currentUser.ID,
                        DevName = item[1].Substring(0, item[1].IndexOf('.')),
                        Port = item[1].Substring(item[1].IndexOf('.') + 1),
                        IccId = item[2],
                        SMSTime = Convert.ToInt32(item[3]),
                        Fm = item[4],
                        To = item[5],
                        Status = SMSStatusType.Success,
                        IsSend = false,
                        IsRead = false
                    };

                    //var userModel = await _userService.GetEntityAsync(x => x.ID == entity.UserId);

                    //普通短信
                    if (item[0] == "0")
                    {
                        if (SecureHelper.IsBase64String(item[6]))
                        {
                            entity.SMSContent = System.Text.ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(item[6]));
                        }
                        else
                        {
                            entity.SMSContent = "此信息格式错误";
                            entity.Status = SMSStatusType.Error;
                        }

                        //判断用户收件人是否存在对应用户
                        //推送至客户端
                        //todo 由于用户不在线的时候不进行发送，考虑将离线信息保留时间设为1分钟
                        JPushApi j = new JPushApi();
                        j.Push_all_alias_message("aixiaoqi" + entity.To, "有一条消息", "", new Dictionary<string, string>()
                        {
                            {"Tel",entity.Fm}
                        });
                    }
                    //送达报告
                    else
                    {
                        entity.SMSContent = item[6];
                    }

                    smsList.Add(entity);
                }

                if (await _smsService.InsertAsync(smsList))
                {
                    return Ok(new
                    {
                        status = 1,

                        data = new
                        {
                            sms_num = result.sms_num,
                            result.data
                        }
                    });
                }
                else
                {
                    return Ok(new { status = 0, msg = "查询短信失败" });
                }
            }
            else
            {
                return Ok(new { status = 1, msg = "sms_num:" + result.sms_num });
            }
        }

        [HttpGet]
        [NoAuthenticate]
        [NonAction]
        public async Task<IHttpActionResult> SMSAllGoipQuery()
        {
            JPushApi j = new JPushApi();
            var usedGoip = await _deviceGoipService.GetUsedEntitysAndUserByIdAsync();
            SMSQueryBindingModels model = null;
            List<UT_SMS> smsList = new List<UT_SMS>();

            foreach (var goip in usedGoip)
            {
                //_smsService.gete()
                model = new SMSQueryBindingModels()
                {
                    sms_num = 0,
                    start = await _smsService.GetMaxNotSendTimeByUserAsync((Guid)goip.UserId) + 1,//获取下一秒开始的短信
                    DevName = goip.DeviceName,
                    Port = goip.Port.ToString(),
                    IccId = goip.IccId,
                };

                //查询短信
                var result = await new EimsApi().SMSQuery(model.sms_num, (int)model.start, model.DevName, model.Port, model.IccId);

                if (result.sms_num > 0)
                {
                    var groupSMS = result.data.GroupBy(x => x[5]);
                    foreach (var item in result.data)
                    {
                        UT_SMS entity = new UT_SMS()
                        {
                            UserId = (Guid)goip.UserId,
                            DevName = item[1].Substring(0, item[1].IndexOf('.')),
                            Port = item[1].Substring(item[1].IndexOf('.') + 1),
                            IccId = item[2],
                            SMSTime = Convert.ToInt32(item[3]),
                            Fm = item[4],
                            To = goip.UT_Users.Tel,// item[5],
                            Status = SMSStatusType.Success,
                            IsSend = false,
                            IsRead = false
                        };

                        //普通短信
                        if (item[0] == "0")
                        {
                            if (SecureHelper.IsBase64String(item[6]))
                            {
                                entity.SMSContent = System.Text.ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(item[6]));
                            }
                            else
                            {
                                entity.SMSContent = "此信息格式错误";
                                entity.Status = SMSStatusType.Error;
                            }

                            //判断用户收件人是否存在对应用户
                            //推送至客户端
                            //todo 由于用户不在线的时候不进行发送，考虑将离线信息保留时间设为1分钟

                            var loginUser = WebUtil.ExistsSession(goip.UT_Users.Tel);

                            if (loginUser != 0)
                            {
                                j.Push_all_alias_message("aixiaoqi" + entity.To, entity.Fm, entity.SMSContent.Length > 10 ? entity.SMSContent.Substring(0, 10) : entity.SMSContent,
                                        new Dictionary<string, string>()
                                        {
                                            {"Tel",entity.Fm}
                                        });
                            }
                        }
                        //送达报告
                        else
                        {
                            entity.SMSContent = item[6];
                        }
                        smsList.Add(entity);
                    }
                }
            }

            if (await _smsService.InsertAsync(smsList))
            {
                return Ok(new
                {
                    status = 1,

                    data = new
                    {
                        sms_num = smsList.Count,
                    }
                });
            }
            else
            {
                //return Ok(new { status = 0, msg = "查询短信失败" });
            }

            return Ok(new
            {
                status = 1,
                msg = "ok",
                data = new
                    {
                        sms_num = 0,
                    }
            });
        }

        /// <summary>
        /// 新短信报告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rpt_num"></param>
        /// <param name="rpts"></param>
        /// <returns></returns>
        [NoLogin]
        public async Task<IHttpActionResult> SMSNewReport([FromBody]SMSNewReportBindingModel model)
        {
            #region 日志
            LoggerHelper.Info("新短信报告");
            LoggerHelper.Info("新短信报告" + DateTime.Now.ToString());
            LoggerHelper.Info(System.Web.HttpContext.Current.Request.Form.ToString());
            LoggerHelper.Info("新短信报告：SerializeObject" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
            LoggerHelper.Info("新短信报告 SerializeObject End");
            LoggerHelper.Info("新短信报告 BodyInputStream:" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());
            LoggerHelper.Info("新短信报告 Content:" + Request.Content.ReadAsStringAsync().Result);
            #endregion
            JPushApi j = new JPushApi();
            UT_EjoinDevSlot ejoinDevSlot = null;

#if DEBUG
            //j.Push_alias_message("aixiaoqi18719053898", "收到10086短信", "SMSReceiveNew", new Dictionary<string, string>()
            //                    {
            //                        {"Tel","10086"},
            //                        {"SMSContent","您当前帐户总余额15.10元,本月可用余额19.45元.中国移动"},
            //                        {"SMSID","0000"}
            //                    });
#endif

            ejoinDevSlot = await _ejoinDevSlotService.GetUsedEntityAndUserAsync(model.DevName, Convert.ToInt32(model.Port));

            if (ejoinDevSlot == null)
            {
                LoggerHelper.Info("无法找到可接收此新短信的资源");
                return Ok(new { status = 0, msg = "无法找到可接收此新短信的资源" });
            }

            string fm = "";
            if (model.Fm.Substring(0, 2) == "86")
            {
                fm = model.Fm.Substring(2);
            }
            else
            {
                fm = model.Fm;
            }

            LoggerHelper.Info("新短信报告 UserTel:" + ejoinDevSlot.UT_Users.Tel);

            UT_SMS entity = new UT_SMS()
            {
                UserId = (Guid)ejoinDevSlot.UserId,
                DevName = model.DevName,//item[1].Substring(0, item[1].IndexOf('.')),
                Port = model.Port,//item[1].Substring(item[1].IndexOf('.') + 1),
                IccId = model.IccId,
                SMSTime = Convert.ToInt32(model.SMSTime),
                Fm = fm,
                To = ejoinDevSlot.UT_Users.Tel,// item[5],
                Status = SMSStatusType.Success,
                IsSend = false,
                IsRead = false
            };

            if (SecureHelper.IsBase64String(model.SMSContent))
            {
                entity.SMSContent = System.Text.ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(model.SMSContent.Replace(' ', '+')));//处理Base64传递过程中会出现的+号变空格问题
            }
            else
            {
                entity.SMSContent = "此信息格式错误";
                entity.Status = SMSStatusType.Error;
            }

            if (await _smsService.InsertAsync(entity))
            {
                //判断用户收件人是否存在对应用户
                //推送至客户端
                //todo 由于用户不在线的时候不进行发送，考虑将离线信息保留时间设为1分钟
                var loginUser = WebUtil.ExistsSession(ejoinDevSlot.UT_Users.Tel);

                if (loginUser != 0)
                {
                    j.Push_android_alias_message("aixiaoqi" + entity.To, "收到" + entity.Fm + "短信", "SMSReceiveNew", new Dictionary<string, string>()
                                {
                                    {"Tel",entity.Fm},
                                    {"SMSContent",entity.SMSContent},
                                    {"SMSID",entity.ID.ToString()}
                                });
                    j.Push_ios_alias_alert("aixiaoqi" + entity.To, "有一条新短信", "有一条新短信", new Dictionary<string, string>()
                        {
                            {"Tel",entity.Fm},
                            {"SMSContent",entity.SMSContent},
                            {"SMSID",entity.ID.ToString()}
                        });
                }

                LoggerHelper.Info("新短信报告loginUser" + loginUser + ejoinDevSlot.UT_Users.Tel);

                return Ok(new
                {
                    status = 1,
                    msg = "ok"
                });
            }
            else
            {
                LoggerHelper.Info("新短信报告保存失败");
            }

            LoggerHelper.Info("新短信报告保存失败2");
            return Ok(new { status = 0, msg = "" });
        }
        #endregion

        #region 2.AT&USSD
        /// <summary>
        /// 发送AT&USSD
        /// 客户端通过网络用HTTP POST请求提交AT&USSD请求给EIMS，任务信息通过JSON格式的数据携带。
        /// 备注：
        /// 1、	devname、iccid，至少有一个是非空的。
        ///     可以支持单个属性的发送也可以支持多个属性同时发送。当只有devname而port为空时，就是往整个设备的所有端口发送。
        /// 2、	编码集= utf8时，表明content是utf-8的字符串，编码集=base64时，表明content是utf-8的BASE64编码的字符串；
        /// </summary>
        /// <param name="type">at/ussd</param>
        /// <param name="devname">设备名称</param>
        /// <param name="port">一个或多个（逗号，短横线连接）发送端口（从1开始）</param>
        /// <param name="iccid">一个或多个（逗号分隔）发送sim卡的iccid</param>
        /// <param name="content">命令内容</param>
        /// <param name="chs">编码集（utf8|base64）</param>
        /// <returns></returns>
        [HttpPost]
        //[NonAction]
        public async Task<IHttpActionResult> ATUSSDSend(AddATUSSDBindingModel model)
        {
            EimsApi api = new EimsApi();
            var result = await api.ATUSSDSend(model.Type, model.Devname, model.Port, model.Iccid, model.Content, model.Chs);
            if (result.status.Length > 0)
            {
                if (result.status.Contains('0'))
                {
                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            //tid = tid
                        }
                    });
                }
                else
                {
                    return Ok(new { status = 0, msg = result.status });
                }
            }
            return Ok(new
            {
                status = 0,
                data = new
                {
                    //tid = tid
                }
            });
        }

        /// <summary>
        /// AT&USSD报告
        /// </summary>
        /// <param name="type">
        /// 数据类型
        /// at-report/ussd-report
        /// </param>
        /// <param name="rpt_num">
        /// 报告数
        /// </param>
        /// <param name="rpts">
        /// 报告对象数组
        /// [0]: 短信发送设备和端口（devname.1A, devname 2B,…），字符串
        /// [1]: 短信发送sim卡的iccid，字符串
        /// [2]: EIMS收到报告时的时间戳
        /// [3]: 报告内容：“ utf-8的BASE64编码”

        /// </param>
        /// <returns></returns>
        [NoAuthenticate]
        //[NonAction]
        public async Task<IHttpActionResult> ATUSSDReport()
        {
            var str = await Request.Content.ReadAsStringAsync();
            LoggerHelper.Info("ATUSSDReport" + str);

            //从body中获取内容
            var model = await Request.Content.ReadAsAsync<TranslateEimsATUSSDModel>();
            LoggerHelper.Info(string.Format("ATUSSDReport,短信发送设备和端口：{0},短信发送sim卡的iccid：{1},报告内容utf-8的BASE64编码{2},EIMS收到报告时的时间戳：{3}", model.rpts[0][0], model.rpts[0][1], model.rpts[0][2], model.rpts[0].Count > 3 ? model.rpts[0][3] : "rpts未传递时间戳"));

            //string type, int rpt_num, string[] rpts

            if (model.type == "at-report" || model.type == "ussd-report" && model.rpt_num > 0)
            {
                foreach (var item in model.rpts)
                {

                }
            }
            return Ok(new { status = 1, msg = "" });
        }
        #endregion
    }
}
