using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Unitoys.Services
{
    public class SMSService : BaseService<UT_SMS>, ISMSService
    {
        UnitoysEntities db = new UnitoysEntities();
        public bool SendSMS(Guid ID, string ToNum)
        {
            var user = db.UT_Users.Find(ID);


            throw new NotImplementedException();
        }


        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(List<UT_SMS> entityList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                foreach (var entity in entityList)
                {
                    db.UT_SMS.Add(entity);
                }
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="tel">用户手机号</param>
        /// <param name="to">接收号码</param>
        /// <param name="beginSMSTime">时间开始</param>
        /// <param name="endSMSTime">时间结束</param>
        /// <param name="smsStatus">短信状态</param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_SMS>>> SearchAsync(int page, int row, string tel, string to, int? beginSMSTime, int? endSMSTime, SMSStatusType? smsStatus)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_SMS.Include(x => x.UT_Users);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (!string.IsNullOrEmpty(to))
                {
                    query = query.Where(x => x.To.Contains(to));
                }
                if (beginSMSTime.HasValue)
                {
                    query = query.Where(x => x.SMSTime >= beginSMSTime);
                }
                if (endSMSTime.HasValue)
                {
                    query = query.Where(x => x.SMSTime <= endSMSTime);
                }
                if (smsStatus != null)
                {
                    query = query.Where(x => x.Status == smsStatus);
                }

                var result = await query.OrderByDescending(x => x.SMSTime).Skip((page - 1) * row).Take(row).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_SMS>>(count, result);
            }
        }

        /// <summary>
        /// 获取用户联系手机号的最后一条往来信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">用户手机号</param>
        /// <returns>多个联系手机号的最后一条往来信息</returns>
        public async Task<IEnumerable<UT_SMS>> GetLastSMSByUserContactTelAsync(int page, int row, Guid UserId, string Tel)
        {
            return await db.UT_SMS.Where(x => x.UserId == UserId && (x.Fm == Tel || x.To == Tel))
                .GroupBy(x => x.Fm == Tel ? x.To : x.Fm).Select(x => x)
                .Select(x => x.OrderByDescending(e => e.SMSTime).FirstOrDefault())
                .OrderByDescending(x => x.SMSTime)
                .Skip((page - 1) * row).Take(row)
                .ToListAsync();
        }

        /// <summary>
        /// 根据用户和来往手机号获取信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="row">页数</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">用户手机号</param>
        /// <param name="ContactTel">联系手机号</param>
        /// <returns>用户和来往手机号短信</returns>
        public async Task<IEnumerable<UT_SMS>> GetByUserAndTelAsync(int page, int row, Guid UserId, string Tel, string ContactTel)
        {
            //判断TEL避免用户查看自己手机号的信息
            var list = await db.UT_SMS.Where(x => x.UserId == UserId &&
                ((x.Fm == ContactTel && x.To == Tel) || (x.To == ContactTel && x.Fm == Tel)))
                .OrderByDescending(x => x.SMSTime)
                .Skip((page - 1) * row).Take(row)
                .ToListAsync();

            //批量更新为已读
            var updateList = list.Where(x => x.IsRead == false).ToList();
            foreach (var item in updateList)
            {
                item.IsRead = true;
                db.UT_SMS.Attach(item);
                db.Entry<UT_SMS>(item).State = EntityState.Modified;
            }
            if (updateList.Count() > 0)
            {
                await db.SaveChangesAsync();
            }


            return list;
        }

        /// <summary>
        /// 根据用户获取最大的收件时间
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns>最大的收件时间</returns>
        public async Task<int> GetMaxNotSendTimeByUserAsync(Guid UserId)
        {
            var list = await db.UT_SMS.Where(x => x.UserId == UserId && x.IsSend == false).MaxAsync(x => x.SMSTime);

            return list;
        }
    }
}
