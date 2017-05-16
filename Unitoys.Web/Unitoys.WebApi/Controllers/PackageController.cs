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
        private IUserReceiveService _userReceiveService;
        private IPackageAttributeService _packageAttributeService;

        public PackageController(IPackageService packageService, IUserReceiveService userReceiveService, IPackageAttributeService packageAttributeService)
        {
            this._packageService = packageService;
            this._userReceiveService = userReceiveService;
            this._packageAttributeService = packageAttributeService;
        }

        /// <summary>
        /// 查询套餐列表
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri]GetPackageBindingModels model)
        {
            //if (model.Category.HasValue && model.Category == CategoryType.Call)
            //{
            //    model.Category = CategoryType.DualSimStandby;
            //}
            //Expression<Func<UT_Package, bool>> exp;

            //exp = x => x.Lock4 == 0 && x.IsDeleted == false;

            //if (!category.HasValue)
            //{

            //}
            //else
            //{
            //    exp = x => x.Category == category && x.Lock4 == 0 && x.IsDeleted == false;
            //}

            //查询Expression
            Expression<Func<UT_Package, bool>> exp = GetPackageSearchExpression<UT_Package>(model);



            //排序Expression
            Expression<Func<UT_Package, object>> packageExp = x => new { x.DisplayOrder };

            //如果pageNumber和pageSize为null，则设置默认值。
            model.PageNumber = model.PageNumber ?? 1;
            model.PageSize = model.PageSize ?? 10;

            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var packageResult = _packageService.GetEntitiesForPagingAsync((int)model.PageNumber, (int)model.PageSize, packageExp, "DESC", exp);

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
                           //Desction = i.Desction,
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
                           //Desction = i.Desction,
                           Pic = i.Pic.GetPackageCompleteUrl(),
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
                           //Desction = packageResult.Desction,
                           Pic = packageResult.UT_Country != null ? packageResult.UT_Country.Pic.GetCountryPicCompleteUrl() : packageResult.Pic.GetPackageCompleteUrl(),
                           LogoPic = packageResult.UT_Country != null ? packageResult.UT_Country.LogoPic.GetPackageCompleteUrl() : packageResult.Pic.GetPackageCompleteUrl(),
                           CountryName = packageResult.UT_Country != null ? packageResult.UT_Country.CountryName : "",
                           ExpireDays = packageResult.ExpireDays.ToString(),
                           Features = packageResult.Features,
                           Details = packageResult.Details,
                           UseDescr = packageResult.UseDescr,
                           IsCanBuyMultiple = packageResult.IsCanBuyMultiple,
                           IsSupport4G = packageResult.IsSupport4G,
                           IsApn = packageResult.IsApn,
                           ApnName = packageResult.ApnName,
                           DescTitlePic = packageResult.DescTitlePic.GetPackageCompleteUrl(),
                           DescPic = packageResult.DescPic.GetPackageCompleteUrl(),
                           OriginalPrice = packageResult.OriginalPrice,
                       };
            return Ok(new { status = 1, data = new { list = data } });
        }

        /// <summary>
        /// 获取轻松服务
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetRelaxed()
        {
            var currentUser = WebUtil.GetApiUserSession();
            Guid userid = currentUser.ID;

            var packageResult = await _packageService.GetEntitiesAsync(x => x.Category == CategoryType.FreeReceive || x.Category == CategoryType.Relaxed);

            var data = from i in packageResult.OrderByDescending(x => x.DisplayOrder)
                       select new
                       {
                           PackageId = i.ID,
                           PackageName = i.PackageName,
                           PicHaveed = i.PicHaveed.GetPackageCompleteUrl(),//已领图片
                           Pic = i.Pic.GetPackageCompleteUrl(),
                           Haveed = _userReceiveService.Haveed(userid, i.ID),//是否已领取
                           Category = ((int)i.Category).ToString()
                       };
            return Ok(new { status = 1, data = new { totalRows = packageResult.Count(), list = data } });
        }

        /// <summary>
        /// 根据订单ID查属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetAttrsByID(Guid id)
        {

            var packageResult = await _packageAttributeService.GetByPackageId(id);

            var data = from i in packageResult.Value
                       select new
                       {
                           ID = i.ID,
                           //PackageId = i.PackageId,
                           CallMinutes = i.CallMinutes.HasValue ? i.CallMinutes.ToString() : "",
                           ExpireDays = i.ExpireDays.HasValue ? i.ExpireDays.ToString() : "",
                           Flow = i.Flow.HasValue ? i.Flow.ToString() : "",
                           CallMinutesDescr = i.CallMinutesDescr ?? "",
                           ExpireDaysDescr = i.ExpireDaysDescr ?? "",
                           FlowDescr = i.FlowDescr ?? "",
                           Price = i.Price.ToString(),
                           OriginalPrice = i.OriginalPrice.ToString(),
                       };

            return Ok(new { status = 1, data = new { list = data } });
        }

        #region 获取查询的Expression表达式
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private Expression<Func<TEntity, bool>> GetPackageSearchExpression<TEntity>(GetPackageBindingModels model)
        {
            Expression expression = null;
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            try
            {
                Expression searchExpression = null;
                Expression inExpression;
                MemberExpression left = null;
                ConstantExpression right;

                left = Expression.Property(parameter, "Lock4");
                right = Expression.Constant(0, typeof(int));
                inExpression = Expression.Equal(left, right);
                searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);

                left = Expression.Property(parameter, "IsDeleted");
                right = Expression.Constant(false, typeof(bool));
                inExpression = Expression.Equal(left, right);
                searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);

                if (model.Category.HasValue)
                {
                    left = Expression.Property(parameter, "Category");
                    right = Expression.Constant(model.Category.Value, typeof(CategoryType));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.IsCategoryFlow.HasValue)
                {
                    left = Expression.Property(parameter, "IsCategoryFlow");
                    right = Expression.Constant(model.IsCategoryFlow.Value, typeof(bool));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.IsCategoryCall.HasValue)
                {
                    left = Expression.Property(parameter, "IsCategoryCall");
                    right = Expression.Constant(model.IsCategoryCall.Value, typeof(bool));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.IsCategoryDualSimStandby.HasValue)
                {
                    left = Expression.Property(parameter, "IsCategoryDualSimStandby");
                    right = Expression.Constant(model.IsCategoryDualSimStandby.Value, typeof(bool));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }
                if (model.IsCategoryKingCard.HasValue)
                {
                    left = Expression.Property(parameter, "IsCategoryKingCard");
                    right = Expression.Constant(model.IsCategoryKingCard.Value, typeof(bool));
                    inExpression = Expression.Equal(left, right);
                    searchExpression = searchExpression == null ? inExpression : Expression.And(inExpression, searchExpression);
                }

                expression = expression == null ? searchExpression : Expression.And(expression, searchExpression);
            }
            catch (Exception)
            {
                return null;
            }

            return expression == null ? x => true : Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
        }
        #endregion
    }
}
