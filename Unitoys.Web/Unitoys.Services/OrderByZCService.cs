using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Unitoys.Services
{
    public class OrderByZCService : BaseService<UT_OrderByZC>, IOrderByZCService
    {
        /// <summary>
        /// 绑定众筹订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="callPhone">联系电话</param>
        /// <returns>0失败/1成功/2不存在/3已被绑定</returns>
        public async Task<int> BindOrder(Guid userId, string callPhone)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var orderZCList = db.UT_OrderByZC.Where(x => x.CallPhone == callPhone).ToList();

                //不存在
                if (orderZCList.Count == 0)
                {
                    return 2;
                }
                //已被其他用户绑定
                if (orderZCList.Where(x => x.UserId != null && x.UserId != userId).Count() > 0)
                {
                    return 3;
                }

                foreach (var orderZC in orderZCList)
                {
                    db.UT_OrderByZC.Attach(orderZC);
                    db.Entry<UT_OrderByZC>(orderZC).State = EntityState.Modified;
                }

                return db.SaveChanges() > 0 ? 1 : 0;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="orderNum">订单号</param>
        /// <param name="name">姓名</param>
        /// <param name="callPhone">用户手机号</param>
        /// <param name="address">地址</param>
        /// <param name="payStatus">付款状态</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_OrderByZC>>> SearchAsync(int page, int row, string orderNum, string name, string callPhone, string address, PayStatusType? payStatus)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_OrderByZC.Include(x => x.UT_Users).Include(x => x.UT_OrderByZCSelectionNumber);

                if (!string.IsNullOrEmpty(orderNum))
                {
                    query = query.Where(x => x.OrderByZCNum.Contains(orderNum));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (!string.IsNullOrEmpty(callPhone))
                {
                    query = query.Where(x => x.CallPhone.Contains(callPhone));
                }

                if (!string.IsNullOrEmpty(address))
                {
                    query = query.Where(x => x.Address.Contains(address));
                }

                //if (payStatus != null)
                //{
                //    query = query.Where(x => x.PayStatus == payStatus);
                //}

                var result = await query.OrderByDescending(x => x.OrderDate).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_OrderByZC>>(count, result);
            }
        }

        /// <summary>
        /// 查询用户订单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="userId">用户</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_OrderByZC>>> GetUserOrderByZCList(int page, int row, Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_OrderByZC.Include(x => x.UT_Users).Include(x => x.UT_OrderByZCSelectionNumber).Where(x => true);

                query = query.Where(x => x.UserId == userId);

                var result = await query.OrderByDescending(x => x.OrderDate).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_OrderByZC>>(count, result);
            }
        }

        /// <summary>
        /// 根据ID获取订单和订单中的选号订单
        /// </summary>
        /// <returns></returns>
        public async Task<UT_OrderByZC> GetEntityAndOrderByZCSelectionNumberByIdAsync(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_OrderByZC.Include(x => x.UT_OrderByZCSelectionNumber).FirstOrDefaultAsync(x => x.ID == ID);
            }
        }
    }
}
