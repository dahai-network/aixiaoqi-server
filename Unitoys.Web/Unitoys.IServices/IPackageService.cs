using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IPackageService : IBaseService<UT_Package>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="packageName">套餐名称</param>
        /// <param name="countryId">国家ID</param>
        /// <param name="operators">运营商</param>
        /// <param name="category">类型</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Package>>> SearchAsync(int page, int rows, string packageName, Guid? countryId, string operators, CategoryType? category, bool? isCategoryFlow, bool? isCategoryCall, bool? isCategoryDualSimStandby, bool? isCategoryKingCard);

        /// <summary>
        /// 根据ID获取套餐和套餐中的国家
        /// </summary>
        /// <returns></returns>
        Task<UT_Package> GetEntityAndCountryByIdAsync(Guid ID);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="packageName">套餐名称</param>
        /// <param name="countryId">国家ID</param>
        /// <param name="operators">运营商</param>
        /// <param name="category">类型</param>
        /// <returns></returns>
        //Task<KeyValuePair<int, List<UT_Package>>> GetRelaxed(Guid userId);

        /// <summary>
        /// 新增套餐实体和多属性组合
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertedList"></param>
        /// <returns></returns>
        Task<bool> InsertEntityPackageAttributeAsync(UT_Package entity, List<UT_PackageAttribute> insertedList);
        /// <summary>
        /// 更新套餐实体和多属性组合
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertedList"></param>
        /// <param name="updatedList"></param>
        /// <param name="deletedList"></param>
        /// <returns></returns>
        Task<bool> UpdateEntityPackageAttributeAsync(UT_Package entity, List<UT_PackageAttribute> insertedList, List<UT_PackageAttribute> updatedList, List<UT_PackageAttribute> deletedList);
        /// <summary>
        /// 删除套餐实体和多属性组合
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Task<bool> DeleteEntityPackageAttributeAsync(Guid ID);
    }
}
