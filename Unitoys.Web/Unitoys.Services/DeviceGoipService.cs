using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class DeviceGoipService : BaseService<UT_DeviceGoip>, IDeviceGoipService
    {
        /// <summary>
        /// 分配一个空闲的Goip端口
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<UT_DeviceGoip> GetNotUsedPortAsync(Guid UserId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (db.UT_DeviceGoip.Any(a => a.UserId.Equals(UserId)))
                {
                    return await db.UT_DeviceGoip.Where(a => a.UserId.Equals(UserId)).FirstOrDefaultAsync();
                }
                else
                {
                    var model = db.UT_DeviceGoip.Where(a => a.UserId.Equals("")).FirstOrDefault();

                    model.UserId = UserId;
                    model.Status = DeviceGoipStatus.Disabled;
                    db.UT_DeviceGoip.Attach(model);
                    db.Entry<UT_DeviceGoip>(model).State = System.Data.Entity.EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        return model;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 取消一个正在使用的Goip端口
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>0失败/1成功/2不存在正在使用的端口</returns>
        public async Task<int> CancelUsedPortAsync(Guid UserId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (await db.UT_DeviceGoip.AnyAsync(a => a.UserId == UserId))
                {
                    var model = await db.UT_DeviceGoip.FirstOrDefaultAsync(a => a.UserId == UserId);

                    model.Status = DeviceGoipStatus.Enable;
                    db.UT_DeviceGoip.Attach(model);
                    db.Entry<UT_DeviceGoip>(model).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        return 1;
                    }
                }
                else
                {
                    return 2;
                }
                return 0;
            }
        }

        /// <summary>
        /// 设置使用的设备
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<bool> SetUsedAsync(Guid UserId, string IccId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                if (!await db.UT_DeviceGoip.AnyAsync(a => a.UserId == UserId))
                {
                    UT_DeviceGoip Entity = new UT_DeviceGoip();
                    Entity.Mac = "";
                    Entity.DeviceName = "";
                    Entity.Status = DeviceGoipStatus.Disabled;
                    Entity.Port = 0;
                    Entity.IccId = IccId;
                    Entity.UserId = UserId;
                    Entity.CreateDate = CommonHelper.GetDateTimeInt();
                    Entity.UpdateDate = CommonHelper.GetDateTimeInt();
                    db.UT_DeviceGoip.Add(Entity);
                }
                else
                {
                    var entity = await db.UT_DeviceGoip.FirstOrDefaultAsync(x => x.UserId == UserId);
                    entity.UserId = UserId;
                    entity.Status = DeviceGoipStatus.Disabled;
                    entity.IccId = IccId;

                    db.UT_DeviceGoip.Attach(entity);
                    db.Entry<UT_DeviceGoip>(entity).State = System.Data.Entity.EntityState.Modified;
                }
                return db.SaveChanges() > 0;
            }
        }

        public async Task<KeyValuePair<int, List<UT_DeviceGoip>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate, DeviceGoipStatus? status)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_DeviceGoip.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (createStartDate != null)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate != null)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_DeviceGoip>>(count, result);
            }
        }

        /// <summary>
        /// 获取使用中的Goip和Goip中的用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<UT_DeviceGoip>> GetUsedEntitysAndUserByIdAsync()
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = await db.UT_DeviceGoip.Include(x => x.UT_Users).Where(x => x.Status == DeviceGoipStatus.Disabled && x.UserId.HasValue).ToListAsync();
                return query;
            }
        }

        /// <summary>
        /// 获取使用中的Goip和Goip中的用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<UT_DeviceGoip> GetUsedEntityAndUserAsync(string IccId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = await db.UT_DeviceGoip.Include(x => x.UT_Users).Where(x => x.Status == DeviceGoipStatus.Disabled && x.UserId.HasValue && x.IccId == IccId).FirstOrDefaultAsync();
                return query;
            }
        }

        public async Task<UT_DeviceGoip> CheckUserGoipAsync(Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceGoip.Where(a => a.UserId == userId && a.Status == DeviceGoipStatus.Disabled).SingleOrDefaultAsync();
            }
        }
        public async Task<bool> CheckIccIdExistsByNotUserAsync(Guid userId, string IccId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceGoip.AnyAsync(x => x.IccId == IccId && x.UserId != userId && x.Status == DeviceGoipStatus.Disabled);
            }
        }

    }
}
