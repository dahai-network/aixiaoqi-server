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

namespace Unitoys.WebApi.Controllers
{
    public class PhoneCallbackController : ApiController
    {
        private readonly IPhoneCallbackService _phoneCallbackService;

        public PhoneCallbackController(IPhoneCallbackService phoneCallbackService)
        {
            this._phoneCallbackService = phoneCallbackService;
        }

        /// <summary>
        /// 获取所有电话回拨记录
        /// </summary>
        /// <returns></returns>
        [NoLogin]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var phoneCallbacks = await _phoneCallbackService.GetEntitiesAsync(x => !x.IsDeleted);

            var data = from i in phoneCallbacks
                       select new
                       {
                           ID = i.ID,                           
                           To = i.To,
                           From = i.From,
                           MaximumPhoneCallTime = i.MaximumPhoneCallTime,
                           Priority = i.Priority,
                           RequestTime = i.RequestTime
                       };

            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 新增电话回拨记录
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]AddPhoneCallbackBindingModel model)
        {
            string errorMsg = "";

            if(string.IsNullOrEmpty(model.To) || !ValidateHelper.IsMobile(model.To))
            {
                errorMsg = "被叫号码不正确";
            }
            else if(string.IsNullOrEmpty(model.From) || !ValidateHelper.IsMobile(model.From))
            {
                errorMsg = "主叫号码不正确";
            }
            else if(model.Priority != 1 && model.Priority != 2)
            {
                errorMsg = "回拨属性必须为隐号或者透传";
            }
            else
            {
                var currentUser = WebUtil.GetApiUserSession();

                UT_PhoneCallback phoneCallback = new UT_PhoneCallback()
                {
                    UserId = currentUser.ID,
                    From = model.From,
                    To = model.To,
                    MaximumPhoneCallTime = model.MaximumPhoneCallTime,
                    Priority = model.Priority,
                    RequestTime = DateTime.Now,
                    IsDeleted = false
                };

                if(await _phoneCallbackService.InsertAsync(phoneCallback))
                {
                    return Ok(new { status = 1, msg = "新增回拨记录成功" });
                }
                else
                {
                    errorMsg = "新增回拨记录失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 逻辑删除回拨记录
        /// </summary>
        /// <param name="phoneCallbackId">回拨记录ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Delete(Guid phoneCallbackId)
        {
            string errorMsg = "";

            if(phoneCallbackId == Guid.Empty)
            {
                errorMsg = "回拨记录ID不正确";
            }
            else
            {
                UT_PhoneCallback phoneCallback = await _phoneCallbackService.GetEntityByIdAsync(phoneCallbackId);

                if(phoneCallback != null)
                {
                    phoneCallback.IsDeleted = true;

                    if(await _phoneCallbackService.UpdateAsync(phoneCallback))
                    {
                        return Ok(new { status = 1, msg = "删除回拨记录成功" });
                    }
                    else
                    {
                        errorMsg = "删除回拨记录失败";
                    }
                }
                else
                {
                    errorMsg = "没有此回拨记录";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

    }
}
