using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;
using System.Diagnostics;

namespace Unitoys.Services
{
    public class UserService : BaseService<UT_Users>, IUserService
    {
        UnitoysEntities db = new UnitoysEntities();

        public async Task<bool> RegisterAsync(UT_Users user, UT_SMSConfirmation smsConfirmation)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                db.UT_Users.Add(user);

                smsConfirmation.IsConfirmed = true;
                smsConfirmation.ConfirmDate = DateTime.Now;

                db.UT_SMSConfirmation.Attach(smsConfirmation);
                db.Entry<UT_SMSConfirmation>(smsConfirmation).State = EntityState.Modified;

                return await db.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> ForgotPasswordAsync(UT_Users user, UT_SMSConfirmation smsConfirmation)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                smsConfirmation.IsConfirmed = true;
                smsConfirmation.ConfirmDate = DateTime.Now;

                db.UT_Users.Attach(user);
                db.Entry<UT_Users>(user).State = EntityState.Modified;

                db.UT_SMSConfirmation.Attach(smsConfirmation);
                db.Entry<UT_SMSConfirmation>(smsConfirmation).State = EntityState.Modified;

                return await db.SaveChangesAsync() > 0;
            }
        }

        public async Task<KeyValuePair<int, List<UT_Users>>> SearchAsync(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate, int? status)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Users.Where(x => true);

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.Tel.Contains(tel));
                }

                if (createStartDate != null && createStartDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null && createEndDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                if (status != null && status != -1)
                {
                    query = query.Where(x => x.Status == status);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).AsNoTracking().ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Users>>(count, result);
            }
        }

        public async Task<UT_Users> CheckUserLoginTelAsync(string Tel, string passWord)
        {
            passWord = SecureHelper.MD5(passWord);
            return await db.UT_Users.Where(a => a.Tel == Tel && a.PassWord == passWord).SingleOrDefaultAsync();
        }

        public bool ModifyPassWord(Guid ID, string oldPwd, string newPwd)
        {
            UT_Users user = db.UT_Users.Find(ID);
            if (user != null && user.PassWord == SecureHelper.MD5(oldPwd))
            {
                user.PassWord = SecureHelper.MD5(newPwd);

                return db.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ModifyUserInfoAndUserShape(Guid userId, string nickName, int? sex, int? birthday, double? height, double? weight, int? movingTarget)
        {
            //修改用户资料
            UT_Users user = await db.UT_Users.FindAsync(userId);
            if (!string.IsNullOrEmpty(nickName))
                user.NickName = nickName;
            if (sex != null)
                user.Sex = (int)sex;
            if (birthday != null)
                user.Birthday = (int)birthday;

            //新增体形资料
            UT_UserShape userShapeEntity = null;
            if (!await db.UT_UserShape.AnyAsync(x =>
                x.UserId == userId))
            {
                userShapeEntity = new UT_UserShape();

                userShapeEntity.UserId = userId;
                userShapeEntity.CreateDate = DateTime.Now;
                db.UT_UserShape.Add(userShapeEntity);
            }
            else
            {
                userShapeEntity = await db.UT_UserShape.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync(x => x.UserId == userId);
                db.UT_UserShape.Attach(userShapeEntity);
                db.Entry<UT_UserShape>(userShapeEntity).State = EntityState.Modified;
            }
            if (height != null)
                userShapeEntity.Height = (double)height;
            if (weight != null)
                userShapeEntity.Weight = (double)weight;
            if (movingTarget != null)
                userShapeEntity.MovingTarget = (int)movingTarget;
            db.UT_Users.Attach(user);
            db.Entry<UT_Users>(user).State = EntityState.Modified;



            return db.SaveChanges() > 0;

        }

        public bool CheckTelExist(string tel)
        {
            return db.UT_Users.Any(a => a.Tel == tel);
        }


        public async Task<UT_Users> GetEntityByOpenIdAsync(string openId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var result = await db.UT_UsersWx.Include(a => a.UT_User).Where(a => a.OpenId == openId).FirstOrDefaultAsync();
                if (result != null)
                {
                    return result.UT_User;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取余额和订单总可通话最长秒数
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1/失败->0通话秒数</returns>
        public async Task<int> GetAmountAndOrderMaximumPhoneCallTime(Guid id)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var user = await db.UT_Users.SingleOrDefaultAsync(x => x.ID == id);
                if (user != null)
                {
                    //判断被叫号码的费率。TODO
                    int maximumPhoneCallTime = 0;

                    if (user.Amount > 0)//只计算可拨打的分钟
                        maximumPhoneCallTime = Convert.ToInt32((int)(user.Amount / UTConfig.SiteConfig.CallDirectPricePerMinutes) * 60);

                    int dtInt = CommonHelper.GetDateTimeInt();

                    //订单剩余分钟数
                    //获取当前用户有效订单-已激活+已付款+剩余通话时间大于0+在有效时间内
                    var orderList = await db.UT_Order.Where(x => x.UserId == user.ID
                            && x.OrderStatus == OrderStatusType.Used
                            && x.PayStatus == PayStatusType.YesPayment
                            && x.EffectiveDate.HasValue
                            && x.EffectiveDate.Value + (x.ExpireDays * 86400) > dtInt
                            && x.RemainingCallMinutes > 0).ToListAsync();

                    if (orderList != null && orderList.Count() > 0)
                    {
                        maximumPhoneCallTime += orderList.Sum(x => x.RemainingCallMinutes) * 60;
                    }
                    return maximumPhoneCallTime;
                }
            }
            return -1;
        }
    }
}
