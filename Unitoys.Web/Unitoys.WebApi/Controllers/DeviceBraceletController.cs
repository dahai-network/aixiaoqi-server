﻿using System;
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
        private IEjoinDevSlotService _ejoinDevSlotService;
        public DeviceBraceletController(IDeviceBraceletService deviceBraceletService, IEjoinDevSlotService ejoinDevSlotService)
        {
            this._deviceBraceletService = deviceBraceletService;
            this._ejoinDevSlotService = ejoinDevSlotService;
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
                    DeviceType = ((int)entity.DeviceType).ToString(),
                    Version = entity.Version == null ? "" : entity.Version,
                    CreateDate = entity.CreateDate.ToString()
                };
                return Ok(new { status = 1, msg = "success", data = data });
            }
            else
            {
                //return Ok(new StatusCodeRes(StatusCodeType.成功, "empty") {});
                return Ok(new { status = 1, msg = "empty", data = new { } });
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


            model.IMEI = System.Web.HttpUtility.UrlDecode(model.IMEI, Encoding.UTF8);

            //验证设备唯一性,友好提示
            var bEntity = await _deviceBraceletService.GetEntityAsync(x => x.IMEI.Equals(model.IMEI));
            if (bEntity != null)
            {
                if (bEntity.UserId == currentUser.ID)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.设备重复绑定, "请勿重复绑定"));
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
                Version = "0", //string.IsNullOrEmpty(model.Version) ? "0" : model.Version,
                CreateDate = CommonHelper.GetDateTimeInt(),
                UserId = currentUser.ID,
                DeviceType = model.DeviceType.HasValue ? model.DeviceType.Value : DeviceType.Bracelet
            };

            var result = await _deviceBraceletService.InsertAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "绑定成功！" });
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "绑定失败"));
        }

        /// <summary>
        /// 连接信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> UpdateConnectInfo([FromBody]DeviceBraceletConnectInfoModels model)
        {
            model.Version = string.IsNullOrEmpty(model.Version) ? "0" : model.Version;
            if (string.IsNullOrEmpty(model.Power))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "电量为空"));
            }
            if (string.IsNullOrEmpty(model.Version))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "版本号为空"));
            }
            //if (!model.DeviceType.HasValue)
            //{
            //    return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "设备类型为空"));
            //}
            switch (await UpdateVersion(model))
            {
                case 0:
                    return Ok(new StatusCodeRes(StatusCodeType.失败, "更新失败"));
                    break;
                case 1:
                    return Ok(new StatusCodeRes(StatusCodeType.成功, "更新成功"));
                    break;
                case 2:
                    return Ok(new StatusCodeRes(StatusCodeType.用户未绑定设备, "未绑定设备"));
                    break;
                default:
                    return Ok(new StatusCodeRes(StatusCodeType.内部错误));
            }
        }

        /// <summary>
        /// 更新用户设备版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns>0失败/1成功/2用户未绑定设备</returns>
        private async Task<int> UpdateVersion(DeviceBraceletConnectInfoModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            var entity = await _deviceBraceletService.GetEntityAsync(x => x.UserId == currentUser.ID);

            if (entity != null)
            {
                //更新设备版本号
                entity.Power = model.Power;
                entity.ConnectDate = CommonHelper.GetDateTimeInt();
                if (model.DeviceType.HasValue)
                    entity.DeviceType = model.DeviceType.Value;
                entity.Version = model.Version;
                entity.UpdateDate = CommonHelper.GetDateTimeInt();

                return await _deviceBraceletService.UpdateAsync(entity) == true ? 1 : 0;
            }
            else
            {
                //该用户不存在绑定设备
                return 2;
            }
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> UnBind()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var entity = await _deviceBraceletService.GetEntityAsync(x => x.UserId == currentUser.ID);

            if (entity == null)
            {
                return Ok(new { status = 1, msg = "解除绑定成功！" });
            }

            var result = await _deviceBraceletService.DeleteAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "解除绑定成功！" });
            }

            return Ok(new StatusCodeRes(StatusCodeType.失败, "解除绑定失败"));
        }

        /// <summary>
        /// 空中升级
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> OTA([FromUri]DeviceBraceletOTABindingModels model)
        {
            //switch (await UpdateVersion(Version + ""))
            //{
            //    case 0:
            //        return Ok(new StatusCodeRes(StatusCodeType.失败, "更新失败"));
            //        break;
            //    //case 2:
            //    //    return Ok(new StatusCodeRes(StatusCodeType.用户未绑定设备, "未绑定设备"));
            //    //    break;
            //}

            if (!model.DeviceType.HasValue || model.DeviceType.Value == Unitoys.Model.DeviceType.Bracelet)
            {
                if (model.Version < Convert.ToDouble(UTConfig.DeviceBraceletOTAConfigInfo.Version))
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
            else
            {
                if (model.Version < Convert.ToDouble(UTConfig.DeviceBraceletUniBoxOTAConfigInfo.Version))
                {
                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            Version = UTConfig.DeviceBraceletUniBoxOTAConfigInfo.Version,// "20",
                            VersionName = UTConfig.DeviceBraceletUniBoxOTAConfigInfo.VersionName,// "1.0.1",
                            Descr = UTConfig.DeviceBraceletUniBoxOTAConfigInfo.Descr.Replace("\\n", "\n"),//"1.更新时间更新\n2.优化传输速度",
                            Url = UTConfig.DeviceBraceletUniBoxOTAConfigInfo.Url//"https://api.unitoys.com/unitoys6.zip",
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

        [HttpGet]
        /// <summary>
        /// 获取手环设备注册状态
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetRegStatus()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var model = await _ejoinDevSlotService.GetUsedAAndEjoinDevsync(currentUser.ID);

            if (model != null && (model.Status == DevPortStatus.REGSUCCESS || model.Status == DevPortStatus.CALLING))
            {
                return Ok(new
                {
                    status = 1,
                    msg = "success",
                    data = new { RegStatus = "1" }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 1,
                    msg = "success",
                    data = new { RegStatus = "0" }
                });
            }
        }
    }
}
