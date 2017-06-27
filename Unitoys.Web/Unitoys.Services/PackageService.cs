using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
namespace Unitoys.Services
{
    public class PackageService : BaseService<UT_Package>, IPackageService
    {
        public async Task<KeyValuePair<int, List<UT_Package>>> SearchAsync(int page, int rows, string packageName, Guid? countryId, string operators, CategoryType? category, bool? isCategoryFlow, bool? isCategoryCall, bool? isCategoryDualSimStandby, bool? isCategoryKingCard)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Package.Include(x => x.UT_Country).Where(x => true);

                if (!string.IsNullOrEmpty(packageName))
                {
                    query = query.Where(x => x.PackageName.Contains(packageName));
                }

                if (countryId.HasValue)
                {
                    query = query.Where(x => x.CountryId != null && x.CountryId == countryId);
                }

                if (!string.IsNullOrEmpty(operators))
                {
                    query = query.Where(x => x.Operators.Contains(operators));
                }
                if (category.HasValue)
                {
                    query = query.Where(x => x.Category == category);
                }
                if (isCategoryFlow.HasValue)
                {
                    query = query.Where(x => x.IsCategoryFlow == isCategoryFlow.Value);
                }
                if (isCategoryCall.HasValue)
                {
                    query = query.Where(x => x.IsCategoryCall == isCategoryCall.Value);
                }
                if (isCategoryDualSimStandby.HasValue)
                {
                    query = query.Where(x => x.IsCategoryDualSimStandby == isCategoryDualSimStandby.Value);
                }
                if (isCategoryKingCard.HasValue)
                {
                    query = query.Where(x => x.IsCategoryKingCard == isCategoryKingCard.Value);
                }

                query = query.Where(x => x.IsDeleted == false);
                var result = await query.OrderBy(x => x.DisplayOrder).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Package>>(count, result);
            }
        }

        /// <summary>
        /// 根据ID获取套餐和套餐中的国家
        /// </summary>
        /// <returns></returns>
        public async Task<UT_Package> GetEntityAndCountryByIdAsync(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_Package.Include(x => x.UT_Country).FirstOrDefaultAsync(x => x.ID == ID);
            }
        }

        /// <summary>
        /// 新增套餐实体和多属性组合
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertedList"></param>
        /// <returns></returns>
        public async Task<bool> InsertEntityPackageAttributeAsync(UT_Package entity, List<UT_PackageAttribute> insertedList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                db.UT_Package.Add(entity);

                foreach (var attrEntity in insertedList)
                {
                    attrEntity.PackageId = entity.ID;
                    db.UT_PackageAttribute.Add(attrEntity);
                }
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 更新套餐实体和多属性组合
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertedList"></param>
        /// <param name="updatedList"></param>
        /// <param name="deletedList"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEntityPackageAttributeAsync(UT_Package entity, List<UT_PackageAttribute> insertedList, List<UT_PackageAttribute> updatedList, List<UT_PackageAttribute> deletedList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                db.UT_Package.Attach(entity);
                db.Entry<UT_Package>(entity).State = EntityState.Modified;

                if (insertedList != null)
                    foreach (var attrEntity in insertedList)
                    {
                        attrEntity.PackageId = entity.ID;
                        db.UT_PackageAttribute.Add(attrEntity);
                    }
                if (updatedList != null)
                    foreach (var attrEntity in updatedList)
                    {
                        attrEntity.PackageId = entity.ID;
                        db.UT_PackageAttribute.Attach(attrEntity);
                        db.Entry<UT_PackageAttribute>(attrEntity).State = EntityState.Modified;
                    }
                if (deletedList != null)
                    foreach (var attrEntity in deletedList)
                    {
                        attrEntity.PackageId = entity.ID;
                        db.UT_PackageAttribute.Attach(attrEntity);
                        db.Entry<UT_PackageAttribute>(attrEntity).State = EntityState.Deleted;
                    }
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 删除套餐实体和多属性组合
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEntityPackageAttributeAsync(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var entity = await db.UT_Package.Include(x => x.UT_PackageAttribute).FirstOrDefaultAsync(x => x.ID == ID);

                if (entity.UT_PackageAttribute != null)
                {
                    var list = entity.UT_PackageAttribute.ToList();
                    foreach (var attrEntity in entity.UT_PackageAttribute.ToList())
                    {
                        attrEntity.PackageId = entity.ID;
                        db.UT_PackageAttribute.Attach(attrEntity);
                        db.Entry<UT_PackageAttribute>(attrEntity).State = EntityState.Deleted;
                    }
                }

                db.UT_Package.Attach(entity);
                db.Entry<UT_Package>(entity).State = EntityState.Deleted;

                return await db.SaveChangesAsync() > 0;
            }
        }

        //public async Task<KeyValuePair<int, List<UT_Package>>> GetRelaxed(Guid userId)
        //{
        //    using (UnitoysEntities db = new UnitoysEntities())
        //    {
        //        //return await db.UT_UserReceive.Where(x => x.UserId == userId);
        //        //return await db.UT_Package.Where(x => x.Category == CategoryType.Relaxed || x.Category == CategoryType.FreeReceive);

        //    }
        //}
    }
}
