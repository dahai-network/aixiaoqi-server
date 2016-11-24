using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Unitoys.Services
{
    /// <summary>
    /// 公共接口
    /// </summary>
    public class BaseService<T> : IBaseService<T> where T : class,new()   //限制T为class
    {
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().Where(exp).ToListAsync();
            }
        }
        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        public virtual async Task<int> GetEntitiesCountAsync(Expression<Func<T, bool>> exp)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().CountAsync(exp);
            }
        }
     
        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderName">lambda排序名称</param>
        /// <param name="sortOrder">排序(升序or降序)</param>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetEntitiesForPagingAsync(int pageNumber, int pageSize, Expression<Func<T, object>> orderName, string sortOrder, Expression<Func<T, bool>> exp)
        {

            using (UnitoysEntities db = new UnitoysEntities())
            {

                if (sortOrder == "asc") //升序排列
                {
                    return await db.Set<T>().Where(exp).OrderBy(orderName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                }
                else
                    return await db.Set<T>().Where(exp).OrderByDescending(orderName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            }
        }
        /// <summary>
        /// 分页查询(SQL分页方式)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderName">lambda排序名称</param>
        /// <param name="sortOrder">排序(升序or降序)</param>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="Count">总个数</param>
        /// <returns></returns>
        public virtual async Task<KeyValuePair<int, object>> GetEntitiesForPagingAsync(string tableName, int pageNumber, int pageSize, string orderName, string sortOrder, string commandText)
        {
            PageHelper pager = new PageHelper(tableName, orderName, pageSize, pageNumber, sortOrder, commandText);
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var temp = db.Set<T>().SqlQuery(pager.GetSelectTopByMaxOrMinPagination()).ToListAsync();
                var count = GetEntitiesCountAsync(commandText);
                return new KeyValuePair<int, object>(await count, await temp);
            }
        }
        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual async Task<T> GetEntityAsync(Expression<Func<T, bool>> exp)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().Where(exp).SingleOrDefaultAsync();
            }
        }
        /// <summary>
        /// 获取所有Entity(立即执行请使用ToList()
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetEntitiesAsync(string CommandText)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().SqlQuery("select * from " + typeof(T).Name + " where " + CommandText).ToListAsync();
            }
        }
        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <returns></returns>
        public virtual async Task<int> GetEntitiesCountAsync(string CommandText)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().SqlQuery("select * from " + typeof(T).Name + " where " + CommandText).CountAsync();
            }
        }

        
        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <returns></returns>
        public virtual async Task<T> GetEntityAsync(string CommandText)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().SqlQuery("select * from " + typeof(T).Name + " where " + CommandText).SingleOrDefaultAsync();
            }
        }
        /// <summary>
        /// 根据id查找
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public virtual async Task<T> GetEntityByIdAsync(Guid id)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var result = await db.Set<T>().FindAsync(id);
                return result;
            }
        }

        /// <summary>
        /// 异步获取所有Entity
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll()
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.Set<T>().ToListAsync();
            }
        }

        /// <summary>
        /// 异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(T entity)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                db.Set<T>().Add(entity);
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 更新Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, params string[] excludeField)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Modified;
                if(excludeField.Length > 0)
                {
                    foreach (var property in excludeField)
	                {
                        db.Entry<T>(entity).Property(property).IsModified = false;
	                }
                }
                return await db.SaveChangesAsync() > 0;
            }
        }
        /// <summary>
        /// 删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (entity != null)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry<T>(entity).State = EntityState.Deleted;
                    return await db.SaveChangesAsync() > 0;

                }
                return false;
            }
        }
    }
}
