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
    public class EjoinDevSlotService : BaseService<UT_EjoinDevSlot>, IEjoinDevSlotService
    {
        public async Task<KeyValuePair<int, List<UT_EjoinDevSlot>>> SearchAsync(int page, int rows, Guid? EjoinDevId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_EjoinDevSlot.Include(x => x.UT_Users).Where(x => true);

                if (EjoinDevId.HasValue)
                {
                    query = query.Where(x => x.EjoinDevId == EjoinDevId);
                }

                var result = await query.OrderBy(x => x.PortNum).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_EjoinDevSlot>>(count, result);
            }
        }

        /// <summary>
        /// 获取使用中的端口和端口中的用户信息
        /// </summary>
        /// <param name="DevName">设备名</param>
        /// <param name="Port">端口</param>
        /// <returns></returns>
        public async Task<UT_EjoinDevSlot> GetUsedEntityAndUserAsync(string DevName, int Port)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = await db.UT_EjoinDevSlot.Include(x => x.UT_Users)
                    .Where(x =>
                        x.UT_EjoinDev.Name == DevName
                        && x.UT_EjoinDev.RegStatus == RegStatusType.SUCCESS
                        && x.PortNum == Port
                        && (x.Status == DevPortStatus.REGING || x.Status == DevPortStatus.REGSUCCESS || x.Status == DevPortStatus.CALLING || x.Status == DevPortStatus.WARNING)
                        && x.UserId.HasValue

                        ).FirstOrDefaultAsync();
                return query;
            }
        }

        /// <summary>
        /// 获取使用的端口
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UT_EjoinDevSlot> GetUsedAAndEjoinDevsync(Guid userId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_EjoinDevSlot.Include(x => x.UT_EjoinDev)
                    .Where(x =>
                        x.UT_EjoinDev.RegStatus == RegStatusType.SUCCESS
                        && x.UserId == userId
                        && (x.Status == DevPortStatus.REGSUCCESS || x.Status == DevPortStatus.CALLING || x.Status == DevPortStatus.WARNING)).FirstOrDefaultAsync();
            }
        }
    }
}
