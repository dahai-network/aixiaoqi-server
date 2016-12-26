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
    public class GiftCardService : BaseService<UT_GiftCard>, IGiftCardService
    {
        public async Task<KeyValuePair<int, List<UT_GiftCard>>> SearchAsync(int page, int rows, string cardNum, DateTime? createStartDate, DateTime? createEndDate, GiftCardStatusType? status)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_GiftCard.Include(x => x.UT_Users).Include(x => x.UT_ManageUsers).Where(x => true);

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

                return new KeyValuePair<int, List<UT_GiftCard>>(count, result);
            }
        }


        public async Task<bool> AddGenerateCard(Guid ManageUserId, int Qty)
        {
            List<UT_GiftCard> entityList = new List<UT_GiftCard>();

            var LastEffectiveDate = CommonHelper.ConvertDateTimeInt(DateTime.Today.AddMonths(3));

            for (int i = 0; i < Qty; i++)
            {
                entityList.Add(new UT_GiftCard()
                {
                    CardNum = DateTime.Now.ToString("yyMMdd") + GetRandomCard(),
                    CardPwd = GetRandomCardPwd() + "",
                    ManageUserId = ManageUserId,
                    CreateDate = DateTime.Now,
                    LastEffectiveDate = LastEffectiveDate,
                    Status = GiftCardStatusType.Enable
                });
            }
            return await InsertAsync(entityList);
        }

        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(List<UT_GiftCard> entityList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                foreach (var entity in entityList)
                {
                    db.UT_GiftCard.Add(entity);
                }
                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 礼包卡绑定
        /// </summary>
        /// <returns>0失败/1成功/2状态不等于未使用/3已超过最晚有效时间/4套餐不存在/5已绑定礼包卡</returns>
        public async Task<int> Bind(Guid userId, string cardPwd, UT_GiftCard outModel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //根据cardNum，cardPwd获取GiftCard对象。
                UT_GiftCard entity = await db.UT_GiftCard.SingleOrDefaultAsync(a => a.CardPwd == cardPwd);

                if (await db.UT_GiftCard.AnyAsync(x => x.UserId == userId))
                {
                    return 5;
                };

                if (entity != null)
                {
                    //如果已超过最晚有效时间
                    if (entity.LastEffectiveDate <= CommonHelper.GetDateTimeInt()) return 3;

                    //如果是已使用状态，直接返回false表示已经充值失败。
                    if (entity.Status != GiftCardStatusType.Enable) return 2;

                    //把付款日期设置为当前，并保存。
                    entity.BindDate = CommonHelper.GetDateTimeInt();
                    entity.Status = GiftCardStatusType.Disabled;
                    entity.UserId = userId;

                    db.UT_GiftCard.Attach(entity);
                    db.Entry<UT_GiftCard>(entity).State = System.Data.Entity.EntityState.Modified;

                    //根据GiftCard的UserId获取User，添加充值金额到用户上，并保存。
                    //UT_Users user = await db.UT_Users.FindAsync(entity.UserId);
                    //user.Amount += entity.Price;

                    //db.UT_Users.Attach(user);
                    //db.Entry<UT_Users>(user).State = System.Data.Entity.EntityState.Modified;

                    //建立UserBill记录账单。
                    //UT_UserBill userBill = new UT_UserBill();
                    //userBill.UserId = (Guid)entity.UserId;
                    //userBill.Amount = entity.Price;
                    //userBill.UserAmount = user.Amount;
                    //userBill.CreateDate = CommonHelper.GetDateTimeInt();
                    //userBill.LoginName = user.Tel;
                    //userBill.BillType = 1;
                    //userBill.PayType = 0; //充值
                    //userBill.Descr = "充值卡充值";

                    //db.UT_UserBill.Add(userBill);

                    outModel.CardNum = entity.CardNum;
                    //outModel.Price = entity.Price;


                    //添加礼包卡包含的订单
                    int quantity = 1;
                    Guid[] pkArray = new Guid[3] { 
                        new Guid("E343A6CD-D3BD-408C-A385-06AA0F2E1B7D"),//大王卡免费体验 
                        new Guid("4820E5E6-1AB3-47D7-B707-22D150149955"), //双卡双待
                        new Guid("6E9D162F-EA8D-4625-86F1-3ABA1860891B") };//200分钟通话时长
                    for (int i = 0; i < pkArray.Length; i++)
                    {
                        Guid packageId = pkArray[i];
                        UT_Package package = await db.UT_Package.FindAsync(packageId);
                        if (package != null)
                        {
                            //1. 先添加Order实体。
                            UT_Order order = new UT_Order();
                            order.UserId = userId;
                            order.OrderNum = String.Format("8022{0}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));
                            order.PackageId = packageId;
                            order.PackageName = package.PackageName;
                            order.PackageCategory = package.Category;
                            order.OrderDate = CommonHelper.GetDateTimeInt();
                            order.PayStatus = 0; //添加时付款状态默认为0：未付款。
                            order.Quantity = quantity;
                            order.UnitPrice = package.Price;
                            order.TotalPrice = package.Price * quantity;
                            order.Flow = package.Flow * quantity;
                            order.OrderStatus = 0; //添加时订单状态默认为0：未激活。
                            order.ExpireDays = package.ExpireDays;
                            order.RemainingCallMinutes = package.CallMinutes;
                            order.PackageFeatures = package.Features;
                            order.PackageDetails = package.Details;
                            order.PaymentMethod = PaymentMethodType.Gift;
                            order.PayDate = CommonHelper.GetDateTimeInt();
                            order.PayStatus = PayStatusType.YesPayment;
                            order.Remark = "礼包卡" + entity.CardNum + "绑定";

                            //双卡双待,通话时长,购买成功后默认激活
                            if (i == 1 || i == 2)
                            {
                                order.ActivationDate = CommonHelper.GetDateTimeInt();
                                order.OrderStatus = OrderStatusType.Used;
                                order.EffectiveDate = CommonHelper.GetDateTimeInt();
                            }
                            db.UT_Order.Add(order);
                        }
                        else
                        {
                            return 4;
                        }
                    }


                    return await db.SaveChangesAsync() == pkArray.Length + 1 ? 1 : 0;
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
