using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;
namespace Unitoys.WebApi.Controllers
{
    public class DeviceGoipController : ApiController
    {
        private IDeviceGoipService _deviceGoipService;

        public DeviceGoipController(IDeviceGoipService deviceGoipService)
        {
            this._deviceGoipService = deviceGoipService;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NonAction]
        public async Task<IHttpActionResult> Bind([FromBody]DeviceGoipBindingModels model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            //else if (string.IsNullOrEmpty(model.IccId))
            //{
            //    errorMsg = "IccId不能为空！";
            //}
            else
            {

                var result = await _deviceGoipService.GetNotUsedPortAsync(currentUser.ID);
                var data = new
                {
                    Mac = result.Mac,
                    DeviceName = result.DeviceName,
                    Port = result.Port,
                    //IccId=result.IccId,
                    Status = result.Status
                };
                if (result != null)
                {
                    return Ok(new { status = 1, msg = "绑定成功！", data = data });
                }
                else
                {
                    errorMsg = "绑定失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NonAction]
        public async Task<IHttpActionResult> UnBind([FromBody]DeviceGoipBindingModels model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            //else if (string.IsNullOrEmpty(model.IccId))
            //{
            //    errorMsg = "IccId不能为空！";
            //}
            else
            {
                var result = await _deviceGoipService.CancelUsedPortAsync(currentUser.ID);
                if (result == 1)
                {
                    return Ok(new { status = 1, msg = "解除绑定成功！" });
                }
                else if (result == 2)
                {
                    errorMsg = "不存在正在使用的端口";
                }
                else
                {
                    errorMsg = "解除绑定失败";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });
        }
    }
}
