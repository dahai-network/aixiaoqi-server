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
    public class BlackListController : ApiController
    {
        private IBlackListService _blackListService;
        private IEjoinDevSlotService _ejoinDevSlotService;
        public BlackListController(IBlackListService blackListService, IEjoinDevSlotService ejoinDevSlotService)
        {
            this._blackListService = blackListService;
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

            var result = await _blackListService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            var data = from i in result.OrderByDescending(x => x.CreateDate)
                       select new
                       {
                           BlackNum = i.BlackNum
                       };

            //if (data != null)
            //{
            //    return Ok(new { status = 1, msg = "success", data = data });
            //}
            //else
            //{
            //    //return Ok(new StatusCodeRes(StatusCodeType.成功, "empty") {});
            //    return Ok(new { status = 1, msg = "empty", data = new { } });
            //}
            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]BlackListBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(model.BlackNum))
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "号码不能为空"));
            }

            var bEntity = await _blackListService.GetEntityAsync(x => x.BlackNum == model.BlackNum && x.UserId == currentUser.ID);
            if (bEntity != null)
            {
                return Ok(new StatusCodeRes(StatusCodeType.重复数据, "请勿重复添加"));
            }

            UT_BlackList entity = new UT_BlackList()
            {
                BlackNum = model.BlackNum,
                CreateDate = CommonHelper.GetDateTimeInt(),
                UserId = currentUser.ID,
            };

            var result = await _blackListService.InsertAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "添加成功！" });
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "添加失败"));
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Delete([FromBody]BlackListBindingModels model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            var entity = await _blackListService.GetEntityAsync(x => x.UserId == currentUser.ID && x.BlackNum == model.BlackNum);

            if (entity == null)
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }

            var result = await _blackListService.DeleteAsync(entity);
            if (result)
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }

            return Ok(new StatusCodeRes(StatusCodeType.失败, "删除失败"));
        }
    }
}
