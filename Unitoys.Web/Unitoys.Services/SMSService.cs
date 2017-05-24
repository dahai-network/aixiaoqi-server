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
        /// <param name="page">页码(0为全部)</param>
        /// <param name="row">页数(0为全部)</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">用户手机号</param>
        /// <returns>多个联系手机号的最后一条往来信息</returns>
        public async Task<IEnumerable<UT_SMS>> GetLastSMSByUserContactTelAsync(int page, int row, Guid UserId, string Tel, int? beginSMSTime)
        {
            var query = db.UT_SMS.Where(x => x.UserId == UserId && (x.Fm == Tel || x.To == Tel)).GroupBy(x => x.Fm == Tel ? x.To : x.Fm).Select(x => x.OrderByDescending(e => e.SMSTime).FirstOrDefault()).OrderByDescending(x => x.SMSTime);
            if (beginSMSTime.HasValue)
            {
                query = query.Where(x => x.SMSTime > beginSMSTime).OrderByDescending(x => x.SMSTime);
            }
            if (page != 0 && row != 0)
            {
                query = query.Skip((page - 1) * row).Take(row).OrderByDescending(x => x.SMSTime);
            }
            return await query
                .ToListAsync();
        }

        /// <summary>
        /// 根据用户和来往手机号获取信息
        /// </summary>
        /// <param name="page">页码(0为全部)</param>
        /// <param name="row">页数(0为全部)</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="Tel">用户手机号</param>
        /// <param name="ContactTel">联系手机号</param>
        /// <returns>用户和来往手机号短信</returns>
        public async Task<KeyValuePair<int, IEnumerable<UT_SMS>>> GetByUserAndTelAsync(int page, int row, Guid UserId, string Tel, string ContactTel, int? beginSMSTime)
        {
            var query = GetUserAndContactTel(UserId, Tel, ContactTel, beginSMSTime);

            List<UT_SMS> list;

            var count = await query.CountAsync();

            if (page != 0 && row != 0)
            {
                query = query.Skip((page - 1) * row).Take(row);
                //query = query.Skip((page - 1) * row).Take(row);
            }
            list = await query.ToListAsync();

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


            return new KeyValuePair<int, IEnumerable<UT_SMS>>(count, list);
        }

        /// <summary>
        /// 获取用户和联系手机号的短信
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Tel"></param>
        /// <param name="ContactTel"></param>
        /// <returns></returns>
        private IQueryable<UT_SMS> GetUserAndContactTel(Guid UserId, string Tel, string ContactTel, int? beginSMSTime)
        {
            //判断TEL避免用户查看自己手机号的信息
            var query = db.UT_SMS.Where(x => x.UserId == UserId &&
                            ((x.Fm == ContactTel && x.To == Tel) || (x.To == ContactTel && x.Fm == Tel)));
            if (beginSMSTime.HasValue)
            {
                query = query.Where(x => x.SMSTime > beginSMSTime);
            }

            return query.OrderByDescending(x => x.SMSTime);
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

        /// <summary>
        /// 批量删除多条
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="ids">多个id</param>
        /// <returns></returns>
        public async Task<bool> DeletesAsync(Guid userId, Guid[] ids)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var listEntity = await db.UT_SMS.Where(x => x.UserId == userId && ids.Contains(x.ID)).ToListAsync();

                //与删除数量不一致
                //if (listEntity.Count == ids.Length)
                //{

                //}

                foreach (var entity in listEntity)
                {
                    db.UT_SMS.Attach(entity);
                    db.Entry<UT_SMS>(entity).State = EntityState.Deleted;
                }

                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 批量删除联系人短信
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <param name="Tel">用户手机号码</param>
        /// <param name="ContactTel">联系人电话</param>
        /// <returns></returns>
        public async Task<bool> DeletesByTelAsync(Guid UserId, string Tel, string ContactTel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var listEntity = await GetUserAndContactTel(UserId, Tel, ContactTel, null).ToListAsync();

                //if (listEntity.Count == ids.Length)
                //{

                //}

                foreach (var entity in listEntity)
                {
                    db.UT_SMS.Attach(entity);
                    db.Entry<UT_SMS>(entity).State = EntityState.Deleted;
                }

                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 批量删除多个联系人短信
        /// </summary>
        /// <param name="UserId">用户</param>
        /// <param name="Tel">用户手机号码</param>
        /// <param name="ContactTel">联系人电话</param>
        /// <returns></returns>
        public async Task<bool> DeletesByTelsAsync(Guid UserId, string Tel, string[] ContactTels)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var listEntity = new List<UT_SMS>();

                foreach (var ContactTel in ContactTels)
                {
                    listEntity.AddRange(await GetUserAndContactTel(UserId, Tel, ContactTel, null).ToListAsync());
                }
                //if (listEntity.Count == ids.Length)
                //{

                //}

                foreach (var entity in listEntity)
                {
                    db.UT_SMS.Attach(entity);
                    db.Entry<UT_SMS>(entity).State = EntityState.Deleted;
                }

                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 获取短信实体,并允许出现重复的tid,获取首行
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<UT_SMS> GetEntityFirstOrDefaultAsync(int tid, string to, SMSStatusType status)
        {
            return await db.UT_SMS.FirstOrDefaultAsync(x => x.TId == tid && x.To == to && x.Status == status);
        }

    }
}
