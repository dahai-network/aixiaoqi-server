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
    public class OrderByZCConfirmationService : BaseService<UT_OrderByZCConfirmation>, IOrderByZCConfirmationService
    {
        /// <summary>
        /// 绑定众筹订单验证成功号码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="callPhone">联系电话</param>
        /// <returns>0失败/1成功/2已绑定</returns>
        public async Task<int> Bind(Guid userId, string tel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var anyResult = await db.UT_OrderByZCConfirmation.AnyAsync(x => x.UserId == userId && x.Tel == tel);

                //不存在
                if (anyResult)
                {
                    return 2;
                }

                UT_OrderByZCConfirmation confirmation = new UT_OrderByZCConfirmation();
                confirmation.UserId = userId;
                confirmation.Tel = tel;
                confirmation.CreateDate = CommonHelper.GetDateTimeInt();

                db.UT_OrderByZCConfirmation.Add(confirmation);

                return db.SaveChanges() > 0 ? 1 : 0;
            }
        }

        /// <summary>
        /// 验证用户是否绑定
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tel">联系电话</param>
        /// <returns></returns>
        public async Task<bool> CheckUserTelExist(Guid userId, string tel)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var anyResult = await db.UT_OrderByZCConfirmation.AnyAsync(x => x.UserId == userId && x.Tel == tel);
                var anyUsersResult = await db.UT_Users.AnyAsync(x => x.ID == userId && x.Tel == tel);
                return anyResult == false ? anyUsersResult : anyResult;
            }
        }
    }
}
