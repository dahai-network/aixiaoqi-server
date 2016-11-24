using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.IServices
{
    public interface IBaseService<T>
    {
        /// <summary>
        /// 获取所有Entity
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="exp">Lambda条件的where</param>
        /// <returns></returns>
        Task<int> GetEntitiesCountAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderName">lambda排序名称</param>
        /// <param name="sortOrder">排序(升序or降序)</param>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetEntitiesForPagingAsync(int pageNumber, int pageSize, Expression<Func<T, object>> orderName, string sortOrder, Expression<Func<T, bool>> exp);

        /// <summary>
        /// 分页查询(Linq分页方式)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderName">lambda排序名称</param>
        /// <param name="sortOrder">排序(升序or降序)</param>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="Count">总个数</param>
        /// <returns></returns>
        Task<KeyValuePair<int, object>> GetEntitiesForPagingAsync(string tableName, int pageNumber, int pageSize, string orderName, string sortOrder, string CommandText);

        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        Task<T> GetEntityAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取所有Entity(立即执行请使用ToList()
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetEntitiesAsync(string CommandText);

        /// <summary>
        /// 计算总个数(分页)
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <returns></returns>
        Task<int> GetEntitiesCountAsync(string CommandText);

      
        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="CommandText"></param>
        /// <returns></returns>
        Task<T> GetEntityAsync(string CommandText);

        /// <summary>
        /// 根据id查找
        /// </summary>
        /// <param name="CommandText"></param>
        /// <returns></returns>
        Task<T> GetEntityByIdAsync(Guid id);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// 异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(T entity);

        /// <summary>
        /// 更新Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, params string[] excludeField);

        /// <summary>
        /// 删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity);
    }
}
