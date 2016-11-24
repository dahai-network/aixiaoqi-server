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
    public class CallTransferNumService : BaseService<UT_CallTransferNum>, ICallTransferNumService
    {
        /// <summary>
        /// 获取可用的大号
        /// </summary>
        /// <returns></returns>
        public async Task<UT_CallTransferNum> GetUsableModel(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //判断是否已经获取过
                if (await db.UT_CallTransferNum.AnyAsync(a => a.UserId == ID && a.Status == StatusType.Disabled))
                {
                    var model = db.UT_CallTransferNum.Where(a => a.UserId == ID && a.Status == StatusType.Disabled).FirstOrDefault();

                    return model;
                }
                else
                {
                    UT_CallTransferNum model = await db.UT_CallTransferNum.Where(a => a.Status == StatusType.Enable).FirstOrDefaultAsync();
                    if (model != null)
                    {
                        model.Status = StatusType.Disabled;
                        model.UserId = ID;
                        db.UT_CallTransferNum.Attach(model);
                        db.Entry<UT_CallTransferNum>(model).State = System.Data.Entity.EntityState.Modified;
                        if (await db.SaveChangesAsync() > 0)
                        {
                            return model;
                        }

                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 取消用户空闲大号资源
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyToUsable(Guid ID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var model = await db.UT_CallTransferNum.Where(a => a.UserId == ID && a.Status == StatusType.Disabled).FirstOrDefaultAsync();
                //判空
                if (model != null)
                {
                    model.Status = StatusType.Enable;
                    model.UserId = null;

                    db.UT_CallTransferNum.Attach(model);
                    db.Entry<UT_CallTransferNum>(model).State = System.Data.Entity.EntityState.Modified;
                }
                return db.SaveChanges() > 0;
            }
        }

        public async Task<KeyValuePair<int, List<UT_CallTransferNum>>> SearchAsync(int page, int rows, string telNum, DateTime? createStartDate, DateTime? createEndDate, StatusType? status)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_CallTransferNum.Include(x => x.UT_Users).Where(x => true);

                if (!string.IsNullOrEmpty(telNum))
                {
                    query = query.Where(x => x.TelNum.Contains(telNum));
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

                return new KeyValuePair<int, List<UT_CallTransferNum>>(count, result);
            }
        }

        /// <summary>
        /// 批量异步插入Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(List<UT_CallTransferNum> entityList)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                foreach (var entity in entityList)
                {
                    db.UT_CallTransferNum.Add(entity);
                }
                return await db.SaveChangesAsync() > 0;
            }
        }


        /// <summary>
        /// 获取正在使用的大号
        /// </summary>
        /// <returns></returns>
        public async Task<UT_CallTransferNum> GetDisabledModel(Guid userID)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_CallTransferNum.FirstOrDefaultAsync(a => a.UserId == userID && a.Status == StatusType.Disabled);
            }
        }
    }
}
