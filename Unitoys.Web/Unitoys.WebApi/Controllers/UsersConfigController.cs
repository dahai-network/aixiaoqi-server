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
    public class UsersConfigController : ApiController
    {
        private IUsersConfigService _usersConfigService;
        public UsersConfigController(IUsersConfigService UsersConfigService)
        {
            this._usersConfigService = UsersConfigService;
        }

        /// <summary>
        /// 上传配置
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> UploadConfig([FromBody]AddUsersConfigBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (!model.Status.HasValue)
            {
                //return Ok(new { status = 0, msg = "状态不能为空！" });
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "状态不能为空"));
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                //return Ok(new { status = 0, msg = "配置名不能为空！" });
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "配置名不能为空"));
            }
            UT_UsersConfig UsersConfig = await _usersConfigService.GetEntityAsync(x => x.UserId == currentUser.ID && x.Name == model.Name);

            if (UsersConfig != null)
            {
                if (UsersConfig.UserId != currentUser.ID)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.参数错误));
                }

                UsersConfig.Status = model.Status.Value;
                UsersConfig.UpdateDate = CommonHelper.GetDateTimeInt();

                if (!await _usersConfigService.UpdateAsync(UsersConfig))
                {
                    return Ok(new StatusCodeRes(StatusCodeType.失败));
                }
            }
            else
            {
                UsersConfig = new UT_UsersConfig()
                {
                    UserId = currentUser.ID,
                    CreateDate = CommonHelper.GetDateTimeInt(),
                    UpdateDate = CommonHelper.GetDateTimeInt(),
                    Name = model.Name,
                    Status = model.Status.Value,
                };

                if (!await _usersConfigService.InsertAsync(UsersConfig))
                {
                    return Ok(new StatusCodeRes(StatusCodeType.失败));
                }
            }

            return Ok(new StatusCodeRes(StatusCodeType.成功));
        }

        /// <summary>
        /// 获取用户配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var dataResult = await _usersConfigService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            var data = from i in dataResult.OrderByDescending(x => x.UpdateDate)
                       select new
                       {
                           //UsersConfigId = i.ID,
                           Name = i.Name,
                           Status = ((int)i.Status).ToString(),
                       };

            return Ok(new { status = 1, data = data });
        }

    }
}
