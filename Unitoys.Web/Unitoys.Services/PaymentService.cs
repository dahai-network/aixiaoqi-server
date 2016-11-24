using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Core;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class PaymentService : BaseService<UT_Payment>, IPaymentService
    {
        /// <summary>
        /// 订单支付成功后调用
        /// </summary>
        /// <param name="paymentNum">充值记录编号</param>
        /// <param name="payAmount">支付金额</param>
        /// <returns></returns>
        public async Task<bool> OnAfterPaymentSuccess(string paymentNum, decimal payAmount)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //根据paymentId获取Payment对象。
                UT_Payment payment = await db.UT_Payment.Where(a => a.PaymentNum == paymentNum).SingleOrDefaultAsync();

                if (payment != null)
                {
                    //如果是已付款状态，直接返回true表示已经处理好了。
                    if (payment.Status == 1) return true;

                    //如果支付的金额小于充值金额
                    if (payAmount < payment.Amount)
                    {
                        payment.PaymentConfirmDate = DateTime.Now;
                        payment.Status = -1;
                        payment.Remark = "实际支付金额：" + payAmount;

                        db.UT_Payment.Attach(payment);
                        db.Entry<UT_Payment>(payment).State = System.Data.Entity.EntityState.Modified;
                        if (await db.SaveChangesAsync() <= 0)
                        {
                            LoggerHelper.Error("订单错误支付更新失败", new Exception("订单错误支付更新失败"));
                        }
                        return false;
                    }
                    if (payAmount > payment.Amount)
                    {
                        payment.Remark = "实际支付金额：" + payAmount;
                    }

                    //把付款日期设置为当前，并保存。
                    payment.PaymentConfirmDate = DateTime.Now;
                    payment.Status = 1;

                    db.UT_Payment.Attach(payment);
                    db.Entry<UT_Payment>(payment).State = System.Data.Entity.EntityState.Modified;

                    //根据Payment的UserId获取User，添加充值金额到用户上，并保存。
                    UT_Users user = await db.UT_Users.FindAsync(payment.UserId);
                    user.Amount += payment.Amount;

                    db.UT_Users.Attach(user);
                    db.Entry<UT_Users>(user).State = System.Data.Entity.EntityState.Modified;

                    //建立UserBill记录账单。
                    UT_UserBill userBill = new UT_UserBill();
                    userBill.UserId = payment.UserId;
                    userBill.Amount = payment.Amount;
                    userBill.UserAmount = user.Amount;
                    userBill.CreateDate = CommonHelper.GetDateTimeInt();
                    userBill.LoginName = user.Tel;
                    userBill.BillType = payment.PayOrReceive;
                    userBill.PayType = 0; //充值
                    userBill.Descr = payment.PaymentMethod.GetDescription() + "充值";

                    db.UT_UserBill.Add(userBill);

                    return await db.SaveChangesAsync() > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// 获取包含用户实体的所有充值记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public async Task<KeyValuePair<int, List<UT_Payment>>> GetPaymentsIncludeUser(int page, int row, string paymentNum, string tel, DateTime? createStartDate, DateTime? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Payment.Include(x => x.UT_Users).Where(x => true);

                if (!string.IsNullOrEmpty(paymentNum))
                {
                    query = query.Where(x => x.PaymentNum.Contains(paymentNum));
                }

                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }

                if (createStartDate != null && createStartDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null && createEndDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                Task<List<UT_Payment>> paymentsTask = query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * row).Take(row).ToListAsync();
                Task<int> countTask = GetEntitiesCountAsync(x => true);

                return new KeyValuePair<int, List<UT_Payment>>(await countTask, await paymentsTask);
            }
        }


        public async Task<UT_Payment> Add(Guid id, decimal Amount, PaymentMethodType PaymentMethod)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                UT_Payment payment = new UT_Payment();
                payment.UserId = id;
                payment.PaymentNum = String.Format("9022{0}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));
                payment.PaymentMethod = PaymentMethod;
                payment.PaymentPurpose = "充值余额";
                payment.PayOrReceive = 1; //默认为1：收入||充值。
                payment.Amount = Amount;
                payment.CreateDate = DateTime.Now;
                payment.Status = 0; //默认为0：待付款。
                db.UT_Payment.Add(payment);
                if (await db.SaveChangesAsync() > 0)
                {
                    return payment;
                }
                return null;
            }
        }
    }
}
