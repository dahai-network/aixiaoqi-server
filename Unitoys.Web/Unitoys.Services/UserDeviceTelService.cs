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
    public class UserDeviceTelService : BaseService<UT_UserDeviceTel>, IUserDeviceTelService
    {
        public async Task<KeyValuePair<int, List<UT_UserDeviceTel>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_UserDeviceTel.Include(x => x.UT_Users).Where(x => true);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }

                if (createStartDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_UserDeviceTel>>(count, result);
            }
        }

        public async Task<UT_UserDeviceTel> GetFirst(Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_UserDeviceTel.OrderByDescending(x => x.UpdateDate).FirstOrDefaultAsync(x => x.UserId == userId && x.IsConfirmed == true);
            }
        }

        public async Task<KeyValuePair<bool, string>> CheckConfirmed(Guid userId, string ICCID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var entity = await db.UT_UserDeviceTel.FirstOrDefaultAsync(x => x.UserId == userId && x.ICCID == ICCID && x.IsConfirmed == true);
                if (entity == null)
                    return new KeyValuePair<bool, string>(false, "");

                entity.UpdateDate = Core.CommonHelper.GetDateTimeInt();
                db.UT_UserDeviceTel.Attach(entity);
                db.Entry<UT_UserDeviceTel>(entity).State = EntityState.Modified;

                return new KeyValuePair<bool, string>(true, entity.Tel);
            }
        }

        /// <summary>
        /// 验证设备内号码
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="tel">设备内号码</param>
        /// <param name="ICCID">ICCID</param>
        /// <param name="code">验证码</param>
        /// <returns>
        /// key：0失败/1成功/2无此验证码/3验证码过期/4无此验证手机号
        /// value：设备内号码
        /// </returns>
        public async Task<KeyValuePair<int, string>> Confirmed(Guid userId, string ICCID, string code)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_SMSConfirmation smsConfirmation = await db.UT_SMSConfirmation.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync(x => x.Code == code && x.Type == 4 && !x.IsConfirmed);
                if (smsConfirmation == null)
                    return new KeyValuePair<int, string>(2, "");
                if (DateTime.Now > smsConfirmation.ExpireDate)
                {
                    //验证码过期
                    return new KeyValuePair<int, string>(3, "");
                }

                var entity = await db.UT_UserDeviceTel.FirstOrDefaultAsync(x => x.Tel == smsConfirmation.Tel && x.ICCID == ICCID && x.UserId == userId);
                if (entity == null)
                    return new KeyValuePair<int, string>(4, "");
                entity.IsConfirmed = true;

                db.UT_UserDeviceTel.Attach(entity);
                db.Entry<UT_UserDeviceTel>(entity).State = EntityState.Modified;

                return await db.SaveChangesAsync() > 0 ? new KeyValuePair<int, string>(1, entity.Tel) : new KeyValuePair<int, string>(0, ""); ;
            }
        }
    }
}
