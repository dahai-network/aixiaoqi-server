using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.JiGuang;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class SpeakRecordController : ApiController
    {
        private ISpeakRecordService _speakRecordService;

        public SpeakRecordController(ISpeakRecordService speakRecordService)
        {
            this._speakRecordService = speakRecordService;
        }

        /// <summary>
        /// 添加通话记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> Add([FromBody]AddSpeakRecordBindingModel model)
        {
            string errorMsg = "";

            if (string.IsNullOrEmpty(model.DeviceName) || !ValidateHelper.IsMobile(model.DeviceName))
            {
                errorMsg = "Caller ID is incorrect|主叫号码不正确";
            }
            else if (string.IsNullOrEmpty(model.CalledTelNum))
            {
                errorMsg = "The called number can not be empty|被叫号码不能为空";
            }
            else if (!model.CalledTelNum.StartsWith("972") && !model.CalledTelNum.StartsWith("973") && !model.CalledTelNum.StartsWith("981"))
            {
                errorMsg = "The called number is not formatted correctly|被叫号码格式不正确";
            }
            else
            {
                if (await _speakRecordService.AddRecordAndDeDuction(model.DeviceName, model.CalledTelNum, model.CallStartTime, model.CallStopTime, model.CallSessionTime, model.CallSourceIp, model.CallServerIp, model.Acctterminatedirection))
                {
                    return Ok(new { status = 1, msg = "Add call record success|添加通话记录成功" });
                }
                else
                {
                    errorMsg = "Add call record failed|添加通话记录失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 添加漏接通话记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> AddMissing([FromBody]AddSpeakRecordBindingModel model)
        {
            LoggerHelper.Info(Newtonsoft.Json.JsonConvert.SerializeObject(model));

            string errorMsg = "";

            if (string.IsNullOrEmpty(model.DeviceName) || !ValidateHelper.IsMobile(model.DeviceName.Substring(2)))
            {
                errorMsg = "Caller ID is incorrect|主叫号码不正确";
            }
            else if (string.IsNullOrEmpty(model.CalledTelNum))
            {
                errorMsg = "The called number can not be empty|被叫号码不能为空";
            }
            //else if (!model.CalledTelNum.StartsWith("972") && !model.CalledTelNum.StartsWith("973") && !model.CalledTelNum.StartsWith("981"))
            //{
            //    errorMsg = "The called number is not formatted correctly|被叫号码格式不正确";
            //}
            else
            {
                model.DeviceName = model.DeviceName.Substring(2);

                bool result = false;

                //漏接电话
                result = await _speakRecordService.AddRecordMissing(model.DeviceName, model.CalledTelNum, model.CallStartTime, model.CallStopTime, model.CallSessionTime, model.CallSourceIp, model.CallServerIp, model.Acctterminatedirection);

                //发送极光通知漏接
                JPushApi j = new JPushApi();
                string userToken = WebUtil.GetApiKeyByTel(model.CalledTelNum);
                j.Push_all_alias_alert("aixiaoqi" + userToken, "漏接" + model.DeviceName + "电话", "漏接" + model.DeviceName + "电话", new Dictionary<string, string>()
                        {
                            {"alertType","SpeakMissing"},
                            {"Tel",model.DeviceName},
                        });

                if (result)
                {
                    return Ok(new { status = 1, msg = "Add call record success|添加通话记录成功" });
                }
                else
                {
                    errorMsg = "Add call record failed|添加通话记录失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 根据用户ID查询通话记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var speakRecords = await _speakRecordService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            var data = from i in speakRecords
                       select new
                       {
                           ID = i.ID,
                           DeviceName = i.DeviceName,
                           CalledTelNum = i.CalledTelNum,
                           CallStartTime = CommonHelper.ConvertDateTimeInt(i.CallStartTime).ToString(),
                           CallStopTime = CommonHelper.ConvertDateTimeInt(i.CallStopTime).ToString(),
                           CallSessionTime = i.CallSessionTime,
                           CallAgoRemainingCallSeconds = i.CallAgoRemainingCallSeconds,
                           CallSourceIp = i.CallSourceIp,
                           CallServerIp = i.CallServerIp,
                           Acctterminatedirection = i.Acctterminatedirection
                       };
            return Ok(new { status = 1, msg = "success", data = data });
        }

        /// <summary>
        /// 删除通话记录
        /// </summary>
        /// <param name="speakRecordId"></param>
        /// <returns></returns>
        [HttpGet]
        [NonAction]
        public async Task<IHttpActionResult> Delete(Guid speakRecordId)
        {
            UT_SpeakRecord speakRecord = await _speakRecordService.GetEntityByIdAsync(speakRecordId);

            if (await _speakRecordService.DeleteAsync(speakRecord))
            {
                return Ok(new { Type = "Success", Msg = "删除成功！", Code = 200 });
            }

            return Ok(new { Type = "Success", Msg = "删除失败！", Code = 401 });
        }
    }
}
