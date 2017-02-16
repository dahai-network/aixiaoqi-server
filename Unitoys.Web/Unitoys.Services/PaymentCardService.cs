using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using System.Data.Entity;
using Unitoys.Core;

namespace Unitoys.Services
{
    public class PaymentCardService : BaseService<UT_PaymentCard>, IPaymentCardService
    {
        public async Task<KeyValuePair<int, List<UT_PaymentCard>>> SearchAsync(int page, int rows, string cardNum, DateTime? createStartDate, DateTime? createEndDate, PaymentCardStatusType? status)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_PaymentCard.Include(x => x.UT_Users).Include(x => x.UT_ManageUsers).Where(x => true);

                if (!string.IsNullOrEmpty(cardNum))
                {
                    query = query.Where(x => x.CardNum.Contains(cardNum));
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }

                if (createStartDate != null && createStartDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null && createEndDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_PaymentCard>>(count, result);
            }
        }


        public async Task<bool> AddGenerateCard(Guid ManageUserId, int Qty, int Price)
        {
            List<UT_PaymentCard> entityList = new List<UT_PaymentCard>();

            var LastEffectiveDate = CommonHelper.ConvertDateTimeInt(DateTime.Today.AddMonths(6));

            for (int i = 0; i < Qty; i++)
            {
                entityList.Add(new UT_PaymentCard()
                {
                    CardNum = DateTime.Now.ToString("yyMMdd") + GetRandomCard(),
                    CardPwd = GetRandomCardPwd() + "",
                    ManageUserId = ManageUserId,
                    CreateDate = DateTime.Now,
                    Price = Price,
                    LastEffectiveDate = LastEffectiveDate,
                    Status = PaymentCardStatusType.Enable
                });
            }
            return await InsertAsync(entityList);
        }

        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(List<UT_PaymentCard> entityList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                foreach (var entity in entityList)
                {
                    db.UT_PaymentCard.Add(entity);
                }
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 充值卡充值
        /// </summary>
        /// <returns>0失败/1成功/2状态不等于未使用/3已超过最晚有效时间</returns>
        public async Task<int> Recharge(Guid userId, string cardPwd, UT_PaymentCard outModel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //根据cardNum，cardPwd获取PaymentCard对象。
                UT_PaymentCard entity = await db.UT_PaymentCard.SingleOrDefaultAsync(a => a.CardPwd == cardPwd);

                if (entity != null)
                {
                    //如果已超过最晚有效时间
                    if (entity.LastEffectiveDate <= CommonHelper.GetDateTimeInt()) return 3;

                    //如果是已使用状态，直接返回false表示已经充值失败。
                    if (entity.Status != PaymentCardStatusType.Enable) return 2;

                    //把付款日期设置为当前，并保存。
                    entity.PaymentDate = CommonHelper.GetDateTimeInt();
                    entity.Status = PaymentCardStatusType.Disabled;
                    entity.UserId = userId;

                    db.UT_PaymentCard.Attach(entity);
                    db.Entry<UT_PaymentCard>(entity).State = System.Data.Entity.EntityState.Modified;

                    //根据PaymentCard的UserId获取User，添加充值金额到用户上，并保存。
                    UT_Users user = await db.UT_Users.FindAsync(entity.UserId);
                    user.Amount += entity.Price;

                    db.UT_Users.Attach(user);
                    db.Entry<UT_Users>(user).State = System.Data.Entity.EntityState.Modified;

                    //建立UserBill记录账单。
                    UT_UserBill userBill = new UT_UserBill();
                    userBill.UserId = (Guid)entity.UserId;
                    userBill.Amount = entity.Price;
                    userBill.UserAmount = user.Amount;
                    userBill.CreateDate = CommonHelper.GetDateTimeInt();
                    userBill.LoginName = user.Tel;
                    userBill.BillType = 1;
                    userBill.PayType = 0; //充值
                    userBill.Descr = "充值卡充值";

                    db.UT_UserBill.Add(userBill);

                    outModel.CardNum = entity.CardNum;
                    outModel.Price = entity.Price;

                    return await db.SaveChangesAsync() > 0 ? 1 : 0;
                }
                return 0;
            }
        }


        private int GetRandomCard()
        {
            Random rdm = new Random(GetRandomSeed());
            int rngNum = rdm.Next(100000, 999999);
            return rngNum;
        }

        private string GetRandomCardPwd()
        {
            //string[] s1 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            //string[] s2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            string[] s1 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N"
                              //,"O"
                              ,"P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string[] s2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            string cardPwd = string.Empty;

            Random rdm = new Random(GetRandomSeed());

            //Random rdm2 = new Random(GetRandomSeed());
            //int rngNum2 = rdm.Next(1000000000, 99999999);

            for (int i = 0; i < 16; i++)
            {
                int rngNum = rdm.Next(0, 13);

                //如果随机数大于9则插入字母
                if (rngNum > 9)
                {
                    cardPwd += s1[rdm.Next(0, 25)];
                }
                //插入数字
                else
                {
                    cardPwd += s2[rngNum];
                }

            }

            return cardPwd;
        }

        /// <summary>
        /// 获取随机种子
        /// </summary>
        /// <returns></returns>
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
