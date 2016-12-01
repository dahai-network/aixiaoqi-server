using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Core.Util;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class ZCSelectionNumberController : ApiController
    {
        private IZCSelectionNumberService _zCSelectionNumberService;

        public ZCSelectionNumberController(IZCSelectionNumberService zCSelectionNumberService)
        {
            this._zCSelectionNumberService = zCSelectionNumberService;
        }

        /// <summary>
        /// 获取归属地列表
        /// </summary>
        /// <param name="queryModel">订单查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetLocationList()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var searchZCSelectionNumbers = await _zCSelectionNumberService.GetEntitiesAsync(x => x.OrderByZCSelectionNumberId == null);


            var result = from i in searchZCSelectionNumbers.GroupBy(x => x.ProvinceName)
                         select new
                         {
                             Province = i.Key,
                             Citys = i.Select(x => x.CityName).Distinct()
                         };

            return Ok(new { status = 1, data = result });
        }

        /// <summary>
        /// 获取众筹选号列表
        /// </summary>
        /// <param name="queryModel">订单查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri]ZCSelectionNumberGetBindingModel model)
        {
            Expression<Func<UT_ZCSelectionNumber, bool>> exp = x =>
                x.OrderByZCSelectionNumberId == null
                && x.ProvinceName == model.Province
                && x.CityName == model.City
             && (!string.IsNullOrEmpty(model.MobileNumber) ? x.MobileNumber.Contains(model.MobileNumber) : true);

            //if (!string.IsNullOrEmpty(model.Province))
            //{
            //    exp.And(x => x.ProvinceName.Contains(model.Province));
            //}
            //if (!string.IsNullOrEmpty(model.City))
            //{
            //    exp.And(x => x.CityName.Contains(model.City));
            //}
            //if (!string.IsNullOrEmpty(model.MobileNumber))
            //{
            //    exp.And(x => x.MobileNumber.Contains(model.MobileNumber));
            //}

            //排序Expression
            Expression<Func<UT_ZCSelectionNumber, object>> orderByExp = x => new { x.CreateDate };

            //如果pageNumber和pageSize为null，则设置默认值。
            model.PageNumber = model.PageNumber ?? 1;
            model.PageSize = model.PageSize ?? 10;

            var searchZCSelectionNumbers = _zCSelectionNumberService.GetEntitiesForPagingAsync((int)model.PageNumber, (int)model.PageSize, orderByExp, "DESC", exp);

            var totalRows = _zCSelectionNumberService.GetEntitiesCountAsync(exp);

            var result = from i in await searchZCSelectionNumbers
                         select new
                         {
                             MobileNumber = i.MobileNumber,
                             Price = i.Price.ToString(),
                         };

            return Ok(new { status = 1, data = new { totalRows = await totalRows, list = result } });
        }

    }
}
