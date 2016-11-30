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
                var query = db.UT_OrderByZC.Include(x => x.UT_OrderByZCSelectionNumber);

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
        /// <param name="CallPhone">联系号码</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_OrderByZC>>> GetUserOrderByZCList(int page, int row, Guid userId, string CallPhone)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_OrderByZC.Include("UT_OrderByZCSelectionNumber.UT_ZCSelectionNumber").Where(x => true);

                if (!string.IsNullOrEmpty(CallPhone))
                {
                    query = query.Where(x => x.CallPhone.Contains(CallPhone));
                }

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
                return await db.UT_OrderByZC.Include("UT_OrderByZCSelectionNumber.UT_ZCSelectionNumber").FirstOrDefaultAsync(x => x.ID == ID);
            }
        }
    }
}
