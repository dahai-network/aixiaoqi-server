using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Eims;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class SMSController : ApiController
    {
        private ISMSService _smsService;
        private ISMSConfirmationService _smsConfirmationService;
        private IDeviceGoipService _deviceGoipService;
        private IEjoinDevSlotService _ejoinDevSlotService;
        public SMSController() { }
        public SMSController(ISMSService smsService, ISMSConfirmationService smsConfirmationService, IDeviceGoipService deviceGoipService, IEjoinDevSlotService ejoinDevSlotService)
        {
            this._smsService = smsService;
            this._smsConfirmationService = smsConfirmationService;
            this._deviceGoipService = deviceGoipService;
            this._ejoinDevSlotService = ejoinDevSlotService;
        }

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
        public async Task<IHttpActionResult> Send(SendSMSBindingModelForContent model)
        {
            model.SMSContent = System.Web.HttpUtility.UrlDecode(model.SMSContent, System.Text.Encoding.UTF8);
            model.To = System.Web.HttpUtility.UrlDecode(model.To, System.Text.Encoding.UTF8);
            long TryParseInt = 0;

            LoggerHelper.Info(model.To);
            foreach (var item in model.To.Split(','))
            {
                LoggerHelper.Info("item：" + item);
                LoggerHelper.Info("item TryParse：" + Int64.TryParse(item, out TryParseInt));
            }

            if (model.To.Split(',').Any(x => !Int64.TryParse(x, out TryParseInt)))
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "接收号码格式不正确"));
            }

            LoggerHelper.Info("发送短信");
            if (string.IsNullOrEmpty(model.SMSContent))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "内容不允许为空"));
            }

            var currentUser = WebUtil.GetApiUserSession();

            EimsApi api = new EimsApi();

            int TId = CommonHelper.GetDateTimeInt();
            string DevName = null;
            string Port = null;
            string IccId = null;
            var goipEntity = await _ejoinDevSlotService.GetUsedAAndEjoinDevsync(currentUser.ID);
            if (goipEntity == null)
            {
                return Ok(new StatusCodeRes(StatusCodeType.手环内的卡未注册成功));
            }
            else
            {
                DevName = goipEntity.UT_EjoinDev.Name;
                Port = goipEntity.PortNum + "";
            }

            List<SMSTaskModel> list = new List<SMSTaskModel>()
            {
                new  SMSTaskModel()
                {
                tid=TId,
                devname=DevName,
                port=Port,
                iccid=IccId,
                to=model.To,
                sms=model.SMSContent,
                sdr=null,//是否开启短信成功状态报告
                }
            };

            UT_SMS entity = new UT_SMS()
            {
                UserId = currentUser.ID,
                DevName = DevName,
                Fm = currentUser.Tel,
                To = model.To,
                SMSTime = CommonHelper.GetDateTimeInt(),
                SMSContent = model.SMSContent,
                Status = SMSStatusType.Int,
                IsSend = true,
                Port = Port,
                IccId = IccId,
                TId = TId,
                IsRead = true,
                UpdateDate = 0// CommonHelper.GetDateTimeInt()
            };

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
                                    tid = TId,
                                    SMSID = entity.ID
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
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "短信服务端调用异常：" + ex.Message));
                }
            }
            return Ok(new
            {
                status = StatusCodeType.失败,
                msg = "失败",
                data = new
                {
                    tid = TId
                }
            });
        }

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
        public async Task<IHttpActionResult> SendRetryForError(SendRetryBindingModelForContent model)
        {
            if (model.SMSID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空));
            }

            var currentUser = WebUtil.GetApiUserSession();

            EimsApi api = new EimsApi();

            string DevName = null;
            string Port = null;
            string IccId = null;
            var goipEntity = await _ejoinDevSlotService.GetUsedAAndEjoinDevsync(currentUser.ID);
            if (goipEntity == null)
            {
                return Ok(new StatusCodeRes(StatusCodeType.手环内的卡未注册成功));
            }
            else
            {
                DevName = goipEntity.UT_EjoinDev.Name;
                Port = goipEntity.PortNum + "";
            }

            UT_SMS entity = await _smsService.GetEntityByIdAsync(model.SMSID);

            if (entity.Status != SMSStatusType.Error)
            {

                if (entity.Status == SMSStatusType.Success)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.短信已发送成功));
                }
                else
                {
                    return Ok(new StatusCodeRes(StatusCodeType.短信处理中));
                }
            }
            if (entity.UserId != currentUser.ID)
            {
                return Ok(new
                {
                    status = 0,
                    msg = "信息异常",
                });
            }

            List<SMSTaskModel> list = new List<SMSTaskModel>()
            {
                new  SMSTaskModel()
                {
                tid=entity.TId.Value,
                devname=DevName,
                port=Port,
                iccid=IccId,
                to=entity.To,
                sms=entity.SMSContent,
                sdr=null,//是否开启短信成功状态报告
                }
            };

            entity.SMSTime = CommonHelper.GetDateTimeInt();
            entity.Status = SMSStatusType.Int;
            entity.UpdateDate = CommonHelper.GetDateTimeInt();

            if (await _smsService.UpdateAsync(entity))
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
                                    tid = entity.TId.Value
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
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误, "短信服务端调用异常：" + ex.Message));
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
                        tid = entity.TId.Value
                    }
                });
            }
            return Ok(new
            {
                status = StatusCodeType.失败,
                msg = "失败",
                data = new
                {
                    tid = entity.TId.Value
                }
            });
        }

        /// <summary>
        /// 添加短信
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddSMS([FromBody]AddSMSBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            UT_SMS sms = new UT_SMS()
            {
                UserId = currentUser.ID,
                //FromNum = model.FromNum,
                //ToNum = model.ToNum,
                //SendDate = DateTime.Now,
                //SMSContent = model.SMSContent,
                //MsgType = model.MsgType,
                //StatusType = model.StatusType,
                //IsRead = 0 //默认为0：未读
            };

            //TODO. 发送短信


            if (await _smsService.InsertAsync(sms))
            {
                return Ok(new { status = 1, msg = "短信添加成功！" });
            }
            return Ok(new { status = 0, msg = "短信添加失败！" });
        }

        /// <summary>
        /// 根据短信ID获取
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid SMSID)
        {
            UT_SMS modal = await _smsService.GetEntityByIdAsync(SMSID);
            if (modal != null)
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        ID = modal.ID,
                        //LoginName = modal.LoginName,
                        //FromNum = modal.FromNum,
                        //ToNum = modal.ToNum,
                        //SendTime = modal.SendDate.ToString(),
                        //SMSContent = modal.SMSContent,
                        //MsgType = modal.MsgType,
                        //IsRead = modal.IsRead
                    }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 0,
                    msg = "查找的内容不存在！"
                });
            }
        }

        /// <summary>
        /// 查询用户的所有信息记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var smsResult = await _smsService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            var data = from i in smsResult
                       select new
                       {
                           //LoginName = i.LoginName,
                           //FromNum = i.FromNum,
                           //ToNum = i.ToNum,
                           //SMSContent = i.SMSContent,
                           //MsgType = i.MsgType,
                           //StatusType = i.StatusType,
                           //IsRead = i.IsRead
                       };
            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 获取用户联系手机号的最后一条往来信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserContactTelLastSMS(int pageNumber = 1, int pageSize = 10)
        {
            var currentUser = WebUtil.GetApiUserSession();

            var smsResult = await _smsService.GetLastSMSByUserContactTelAsync(pageNumber, pageSize, currentUser.ID, currentUser.Tel);
            var data = smsResult.Select(i => new
            {
                Fm = i.Fm,
                To = i.To,
                SMSTime = i.SMSTime.ToString(),
                //SMSTimeDescr = GetSMSTimeDescr(i.SMSTime),
                SMSContent = i.SMSContent,
                IsSend = i.IsSend,
                IsRead = i.IsRead == true ? 1 + "" : 0 + "",
                Status = (int)i.Status + "",
                //SMSID = i.ID

            });

            return Ok(new { status = 1, data = data });
        }

        private string GetSMSTimeDescr(DateTime dt)
        {
            TimeSpan ts = (DateTime.Now - dt);
            string SMSTimeDescr = "";
            if (ts.Days == 0)
            {
                SMSTimeDescr = dt.ToString("HH:mm");
            }
            else if (ts.Days == 1)
            {
                SMSTimeDescr = "昨天";
            }
            else if (ts.Days == 2)
            {
                SMSTimeDescr = "前天";
            }
            else if (DateTime.Now.Year == dt.Year)
            {
                SMSTimeDescr = dt.ToString("M月d号 HH:mm");
            }
            else
            {
                SMSTimeDescr = dt.ToString("yyyy年M月d号 HH:mm");
            }
            return SMSTimeDescr;
        }

        /// <summary>
        /// 根据用户和来往手机号获取信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetByTel(string Tel, int pageNumber = 1, int pageSize = 10)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(Tel))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "Tel不允许为空"));
            }

            var smsResult = await _smsService.GetByUserAndTelAsync(pageNumber, pageSize, currentUser.ID, currentUser.Tel, Tel);

            var data = from i in smsResult
                       select new
                       {
                           Fm = i.Fm,
                           To = i.To,
                           SMSTime = i.SMSTime.ToString(),
                           SMSContent = i.SMSContent,
                           IsSend = i.IsSend,
                           //IsRead = i.IsRead == true ? 1 : 0,
                           Status = (int)i.Status + "",
                           SMSID = i.ID
                       };
            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 根据ID删除短信
        /// </summary>
        /// <param name="smsId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Delete([FromBody]DeleteSMSBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            UT_SMS sms = await _smsService.GetEntityByIdAsync(model.Id);

            if (sms != null && sms.UserId == currentUser.ID && await _smsService.DeleteAsync(sms))
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }
            return Ok(new { status = 0, Msg = "删除失败！" });
        }

        /// <summary>
        /// 删除多条短信
        /// </summary>
        /// <param name="smsId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Deletes([FromBody]DeleteSMSBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (await _smsService.DeletesAsync(currentUser.ID, model.Ids))
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }
            return Ok(new { status = 0, Msg = "删除失败！" });
        }

        /// <summary>
        /// 删除联系人短信
        /// </summary>
        /// <param name="smsId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> DeletesByTel([FromBody]DeleteSMSBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (await _smsService.DeletesByTelAsync(currentUser.ID, currentUser.Tel, model.Tel))
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }
            return Ok(new { status = 0, Msg = "删除失败！" });
        }
    }
}
