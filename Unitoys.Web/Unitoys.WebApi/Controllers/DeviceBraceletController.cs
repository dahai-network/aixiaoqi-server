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
    public class DeviceBraceletController : ApiController
    {
        private IDeviceBraceletService _deviceBraceletService;

        public DeviceBraceletController(IDeviceBraceletService deviceBraceletService)
        {
            this._deviceBraceletService = deviceBraceletService;
        }

        /// <summary>
        /// 根据用户ID查询设备
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var entity = await _deviceBraceletService.GetEntityAsync(x => x.UserId == currentUser.ID);

            if (entity != null)
            {
                var data = new
                {
                    IMEI = entity.IMEI,
                    Version = entity.Version == null ? "" : entity.Version,
                    CreateDate = entity.CreateDate.ToString()
                };
                return Ok(new { status = 1, msg = "success", data = data });
            }
            else
            {
                return Ok(new StatusCodeRes(StatusCodeType.失败, "empty"));
            }
        }

        [HttpGet]
        /// <summary>
        /// 查询设备是否被其他用户绑定
        /// </summary>
        /// <param name="IMEI">设备号</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> IsBind(string IMEI)
        {
            var currentUser = WebUtil.GetApiUserSession();

            bool result = await _deviceBraceletService.CheckIMEIByNotUserExist(currentUser.ID, IMEI);

            if (result)
            {
                return Ok(new
                {
                    status = 1,
                    msg = "success",
                    data = new { BindStatus = "1" }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 1,
                    msg = "success",
                    data = new { BindStatus = "0" }
                });
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Bind([FromBody]DeviceBraceletBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(model.IMEI))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "设备号不能为空"));
            }

            model.Version = string.IsNullOrEmpty(model.Version) ? "0" : model.Version;
            model.IMEI = System.Web.HttpUtility.UrlDecode(model.IMEI, Encoding.UTF8);

            //验证设备唯一性,友好提示
            var bEntity = await _deviceBraceletService.GetEntityAsync(x => x.IMEI.Equals(model.IMEI));
            if (bEntity != null)
            {
                if (bEntity.UserId == currentUser.ID)
                {
                    if (bEntity.Version == model.Version)
                    {
                        return Ok(new StatusCodeRes(StatusCodeType.设备重复绑定, "请勿重复绑定"));
                    }
                    else
                    {
                        //更新设备版本号
                        bEntity.Version = model.Version;
                        bEntity.UpdateDate = CommonHelper.GetDateTimeInt();

                        if (await _deviceBraceletService.UpdateAsync(bEntity))
                        {
                            return Ok(new { status = 1, msg = "绑定成功！" });
                        }
                        return Ok(new StatusCodeRes(StatusCodeType.失败, "绑定失败"));
                    }
                }
                else
                {
                    return Ok(new StatusCodeRes(StatusCodeType.设备重复绑定, "此设备已绑定其他用户"));
                }
            }

            if (await _deviceBraceletService.CheckUserIdExist(currentUser.ID))
            {
                return Ok(new StatusCodeRes(StatusCodeType.设备重复绑定, "用户已绑定设备"));
            }

            UT_DeviceBracelet entity = new UT_DeviceBracelet()
            {
                IMEI = model.IMEI,
                Version = string.IsNullOrEmpty(model.Version) ? "0" : model.Version,
                CreateDate = CommonHelper.GetDateTimeInt(),
                UserId = currentUser.ID
            };

            var result = await _deviceBraceletService.InsertAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "绑定成功！" });
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "绑定失败"));
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> UnBind()
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            var entity = await _deviceBraceletService.GetEntityAsync(x => x.UserId == currentUser.ID);

            var result = await _deviceBraceletService.DeleteAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "解除绑定成功！" });
            }
            else
            {
                return Ok(new StatusCodeRes(StatusCodeType.失败, "解除绑定失败"));
            }

        }

        /// <summary>
        /// 空中升级
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> OTA(double Version)
        {
            if (Version < Convert.ToDouble(UTConfig.DeviceBraceletOTAConfigInfo.Version))
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        Version = UTConfig.DeviceBraceletOTAConfigInfo.Version,// "20",
                        VersionName = UTConfig.DeviceBraceletOTAConfigInfo.VersionName,// "1.0.1",
                        Descr = UTConfig.DeviceBraceletOTAConfigInfo.Descr.Replace("\\n", "\n"),//"1.更新时间更新\n2.优化传输速度",
                        Url = UTConfig.DeviceBraceletOTAConfigInfo.Url//"https://api.unitoys.com/unitoys6.zip",
                    }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 1,
                    msg = "已是最新版本",
                    data = new
                    {

                    }
                });
            }
        }
    }
}
