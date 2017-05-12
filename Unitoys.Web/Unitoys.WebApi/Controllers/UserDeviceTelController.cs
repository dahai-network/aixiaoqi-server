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

        public UserDeviceTelController(IUserDeviceTelService userDeviceTelService)
        {
            this._userDeviceTelService = userDeviceTelService;
        }

        /// <summary>
        /// 查询所有用户绑定号码
        /// </summary>
        /// <returns></returns>
        //public async Task<IHttpActionResult> Get()
        //{
        //    //var packageResult = await _packageService.GetEntitiesAsync(exp);
        //    //如果查询条件不为空，则根据查询条件查询，反则查询所有。
        //    var dataResult = await _userDeviceTelService.GetAll();


        //    var data = from i in dataResult.OrderByDescending(x => x.CreateDate)
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
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
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

            if (await _userDeviceTelService.GetEntitiesCountAsync(x => x.Tel == model.Tel) > 0)
            {
                var entity = await _userDeviceTelService.GetEntityAsync(x => x.Tel == model.Tel);
                entity.UpdateDate = CommonHelper.GetDateTimeInt();

                if (await _userDeviceTelService.UpdateAsync(entity))
                {
                    return Ok(new { status = 1, msg = "成功！" });
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
                    IsConfirmed = true
                };

                if (await _userDeviceTelService.InsertAsync(entity))
                {
                    return Ok(new { status = 1, msg = "添加成功！" });
                }
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "添加失败"));
        }

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
