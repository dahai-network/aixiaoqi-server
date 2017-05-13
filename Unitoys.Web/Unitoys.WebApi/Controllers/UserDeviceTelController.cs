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
    public class UserDeviceTelController : ApiController
    {
        private IUserDeviceTelService _userDeviceTelService;
        private ISMSConfirmationService _smsConfirmationService;
        private IUserService _userService;

        public UserDeviceTelController(IUserDeviceTelService userDeviceTelService, ISMSConfirmationService smsConfirmationService, IUserService userService)
        {
            this._userDeviceTelService = userDeviceTelService;
            this._smsConfirmationService = smsConfirmationService;
            this._userService = userService;
        }

        /// <summary>
        /// 查询所有用户绑定号码
        /// </summary>
        /// <returns></returns>
        //public async Task<IHttpActionResult> Get()
        //{
        //    var currentUser = WebUtil.GetApiUserSession();

        //    //var packageResult = await _packageService.GetEntitiesAsync(exp);
        //    //如果查询条件不为空，则根据查询条件查询，反则查询所有。
        //    var dataResult = await _userDeviceTelService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

        //    var data = from i in dataResult.OrderByDescending(x => x.UpdateDate)
        //               select new
        //               {
        //                   ID = i.ID,
        //                   Tel = i.Tel,
        //                   ICCID = i.ICCID,
        //               };

        //    return Ok(new
        //    {
        //        status = 1,
        //        data =  //dic,
        //        new { list = data }
        //    });
        //}

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Confirmed([FromBody]AddUserDeviceTelBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(model.Tel))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "电话不能为空"));
            }
            if (string.IsNullOrEmpty(model.ICCID))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "ICCID不能为空"));
            }

            if (await _userDeviceTelService.GetEntitiesCountAsync(x => x.Tel == model.Tel && x.ICCID == model.ICCID) > 0)
            {
                var entity = await _userDeviceTelService.GetEntityAsync(x => x.Tel == model.Tel && x.ICCID == model.ICCID);

                if (entity.IsConfirmed)
                {
                    return Ok(new StatusCodeRes(StatusCodeType.重复数据, "已验证"));
                }

                entity.UpdateDate = CommonHelper.GetDateTimeInt();
                if (!await _userDeviceTelService.UpdateAsync(entity))
                {
                    return Ok(new { status = 0, msg = "失败！" });
                }
            }
            else
            {
                UT_UserDeviceTel entity = new UT_UserDeviceTel()
                {
                    UserId = currentUser.ID,
                    CreateDate = CommonHelper.GetDateTimeInt(),
                    UpdateDate = CommonHelper.GetDateTimeInt(),
                    Tel = model.Tel,
                    ICCID = model.ICCID,
                    IsConfirmed = false
                };

                if (!await _userDeviceTelService.InsertAsync(entity))
                {
                    return Ok(new { status = 0, msg = "失败！" });
                }
            }

            //发送验证短信
            var result = await new Unitoys.WebApi.Controllers.Util.ConfirmationController(_smsConfirmationService, _userService).SendSMSConfirmation(model.Tel, 4);

            return Ok(result);
            //return Ok(new StatusCodeRes(StatusCodeType.失败, "添加失败"));
        }

        /// <summary>
        /// 最新已验证信息
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetFirst()
        {
            var currentUser = WebUtil.GetApiUserSession();

            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var dataResult = await _userDeviceTelService.GetFirst(currentUser.ID);

            if (dataResult == null)
            {
                return Ok(new
               {
                   status = 1,
                   data = new { },
               });
            }

            return Ok(new
            {
                status = 1,
                data =  //dic,
                new
                {
                    Tel = dataResult.Tel,
                    ICCID = dataResult.ICCID,
                }
            });
        }

        [HttpGet]
        /// <summary>
        /// 检查是否需要验证
        /// </summary>
        /// <param name="ICCID">ICCID</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> CheckConfirmed(string ICCID)
        {
            var currentUser = WebUtil.GetApiUserSession();

            var dataResult = await _userDeviceTelService.CheckConfirmed(currentUser.ID, ICCID);

            return Ok(new
            {
                status = 1,
                data =  //dic,
                new
                {
                    IsConfirmed = dataResult.Key,
                    Tel = dataResult.Value,
                }
            });
        }
    }
}
