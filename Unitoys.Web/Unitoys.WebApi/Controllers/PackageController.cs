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
    public class PackageController : ApiController
    {
        private IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            this._packageService = packageService;
        }

        /// <summary>
        /// 查询套餐列表
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get(int? pageNumber, int? pageSize, CategoryType? category)
        {
            if (category.HasValue && category == CategoryType.Call)
            {
                category = CategoryType.DualSimStandby;
            }
            Expression<Func<UT_Package, bool>> exp;

            if (!category.HasValue)
            {
                exp = x => x.Lock4 == 0 && x.IsDeleted == false;
            }
            else
            {
                exp = x => x.Category == category && x.Lock4 == 0 && x.IsDeleted == false;
            }

            //排序Expression
            Expression<Func<UT_Package, object>> packageExp = x => new { x.DisplayOrder };

            //如果pageNumber和pageSize为null，则设置默认值。
            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 10;

            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var packageResult = _packageService.GetEntitiesForPagingAsync((int)pageNumber, (int)pageSize, packageExp, "DESC", exp);


            var totalRows = _packageService.GetEntitiesCountAsync(exp);

            var data = from i in await packageResult
                       select new
                       {
                           PackageId = i.ID,
                           PackageName = i.PackageName,
                           PackageNum = i.PackageNum,
                           Operators = i.Operators,
                           Price = i.Price.ToString(),
                           //Flow = i.Flow,
                           Flow = "不限制流量",
                           Desction = i.Desction,
                           CallMinutes = i.CallMinutes,
                           Pic = i.Pic.GetPackageCompleteUrl(),
                           ExpireDays = i.ExpireDays.ToString(),
                       };
            return Ok(new { status = 1, data = new { totalRows = await totalRows, list = data } });
        }

        /// <summary>
        /// 查询套餐列表
        /// </summary>
        /// <param name="CountryID">国家ID</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetByCountry(Guid CountryID)
        {
            if (CountryID == Guid.Empty)
            {
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "传入ID格式错误"));
            }
            var packageResult = await _packageService.GetEntitiesAsync(c => (c.CountryId != null && c.CountryId == CountryID) && c.Lock4 == 0 && c.IsDeleted == false);

            var data = from i in packageResult.OrderBy(x => x.DisplayOrder)
                       select new
                       {
                           PackageId = i.ID,
                           PackageName = i.PackageName,
                           PackageNum = i.PackageNum,
                           Operators = i.Operators,
                           Price = i.Price.ToString(),
                           //Flow = i.Flow,
                           Flow = "不限制流量",
                           Desction = i.Desction,
                           //Pic = i.Pic.GetPackageCompleteUrl(),
                           ExpireDays = i.ExpireDays.ToString(),
                       };

            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 根据ID查询套餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetByID(Guid id)
        {
            var packageResult = await _packageService.GetEntityAndCountryByIdAsync(id);

            var data = new
                       {
                           PackageId = packageResult.ID,
                           PackageName = packageResult.PackageName,
                           PackageNum = packageResult.PackageNum,
                           Operators = packageResult.Operators,
                           Price = packageResult.Price.ToString(),
                           //Flow = packageResult.Flow,
                           Flow = "不限制流量",
                           Desction = packageResult.Desction,
                           Pic = packageResult.UT_Country != null ? packageResult.UT_Country.Pic.GetCountryPicCompleteUrl() : packageResult.Pic.GetPackageCompleteUrl(),
                           LogoPic = packageResult.UT_Country != null ? packageResult.UT_Country.LogoPic.GetPackageCompleteUrl() : packageResult.Pic.GetPackageCompleteUrl(),
                           ExpireDays = packageResult.ExpireDays.ToString(),
                           Features = packageResult.Features,
                           Details = packageResult.Details,
                           IsCanBuyMultiple = packageResult.IsCanBuyMultiple
                       };
            return Ok(new { status = 1, data = new { list = data } });
        }
    }
}
