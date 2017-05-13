using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers.Util
{
    public class ConfirmationController : ApiController
    {
        private ISMSConfirmationService _smsConfirmationService;
        private IUserService _userService;

        public ConfirmationController(ISMSConfirmationService smsConfirmationService, IUserService userService)
        {
            this._smsConfirmationService = smsConfirmationService;
            this._userService = userService;
        }

        static Dictionary<string, DateTime> dicSendSMSTime = new Dictionary<string, DateTime>();

        /// <summary>
        /// 发送验证短信
        /// </summary>
        /// <param name="toNum">号码</param>
        /// <param name="type">验证类型</param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> SendSMS([FromBody]SendSMSBindingModel model)
        {
            if (!ValidateHelper.IsMobile(model.ToNum))
            {
                return Ok(new StatusCodeRes(StatusCodeType.手机号码格式不正确));
            }
            else if (model.Type != 1 && model.Type != 2 && model.Type != 3)
            {
                return Ok(new StatusCodeRes(StatusCodeType.验证类型错误));
            }
            else if (model.Type == 1 && _userService.CheckTelExist(model.ToNum))
            {
                return Ok(new StatusCodeRes(StatusCodeType.您输入的手机号码已注册2, "您输入的手机号码已注册"));
            }
            else if (model.Type == 2 && !_userService.CheckTelExist(model.ToNum))
            {
                return Ok(new StatusCodeRes(StatusCodeType.您输入的手机号码未注册));
            }

            var result = await SendSMSConfirmation(model.ToNum, model.Type);
            return Ok(result);
        }

        /// <summary>
        /// 发送验证短信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<StatusCodeRes> SendSMSConfirmation(string toNum, int type)
        {
            DateTime dtNow = DateTime.Now;
            StatusCodeRes errorMsg = new StatusCodeRes();

            if (dicSendSMSTime.ContainsKey(toNum) &&
                (dtNow - dicSendSMSTime[toNum]).TotalSeconds <= 60)
            {
                int tLeft = Convert.ToInt32(60 - (dtNow - dicSendSMSTime[toNum]).TotalSeconds);
                string msg = string.Format("一分钟内不能再次发送,{0}秒以后可以再次发送", tLeft);

                errorMsg = new StatusCodeAndDataRes(StatusCodeType.一分钟内不能再次发送_RemainingSeconds秒以后可以再次发送, msg, new { RemainingSeconds = tLeft + "" });
                //return Ok(new { status = StatusCodeType.一分钟内不能再次发送_RemainingSeconds秒以后可以再次发送, msg = msg, data = new { RemainingSeconds = tLeft + "" } });
            }
            else
            {
                Random random = new Random();

                //1. 生成随机四位数为验证码。            
                string code = random.Next(1000, 10000).ToString();

                //2. 判断是否发送短信成功。
                var rsp = await WebUtil.SendSMSAsync(toNum, code, type);
                if (rsp.code == "0")
                {
                    //3. 插入验证码记录。
                    UT_SMSConfirmation smsConfirmation = new UT_SMSConfirmation()
                    {
                        Tel = toNum,
                        Code = code,
                        CreateDate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddMinutes(10), //过期时间默认为10分钟.
                        Type = type,
                        IsConfirmed = false //是否已经验证默认为否.
                    };

                    //3.写入此次短信发送时间
                    //建议一段时间后清理一次此集合，可使用缓存依赖24小时过期回调来处理
                    dicSendSMSTime[toNum] = DateTime.Now;

                    if (await _smsConfirmationService.InsertAsync(smsConfirmation))
                    {
                        //return Ok(new { status = 1, msg = "发送成功" });
                        errorMsg = new StatusCodeRes(StatusCodeType.成功, "发送成功");
                    }
                    else
                    {
                        LoggerHelper.Error("发送短信接口错误（短信已发送，保存短信记录到数据库时出错！）");
                        errorMsg = new StatusCodeRes(StatusCodeType.短信服务器异常_请联系客服人员);
                    }
                }
                else if (rsp.code == "15")
                {
                    errorMsg = new StatusCodeRes(StatusCodeType.您发送的太频繁了);
                }
                else
                {
                    errorMsg = new StatusCodeRes(StatusCodeType.阿里云短信调用失败) { msg = rsp.msg };
                }
            }
            return errorMsg;
        }
    }
}
