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
    public class DeviceBraceletConnectRecordService : BaseService<UT_DeviceBraceletConnectRecord>, IDeviceBraceletConnectRecordService
    {
        public async Task<KeyValuePair<int, List<UT_DeviceBraceletConnectRecord>>> SearchAsync(int page, int rows, string sort, string order, string iMEI, string tel, int? createStartDate, int? createEndDate, bool? isOnLine)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_DeviceBraceletConnectRecord.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (!string.IsNullOrEmpty(iMEI))
                {
                    query = query.Where(x => x.IMEI.Contains(iMEI));
                }
                if (createStartDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate >= createStartDate);
                }

                if (createEndDate.HasValue)
                {
                    query = query.Where(x => x.CreateDate <= createEndDate);
                }
                if (isOnLine.HasValue)
                {
                    query = query.Where(x => x.DisconnectDate.HasValue == !isOnLine);
                }

                if (!string.IsNullOrEmpty(sort))
                {
                    var sortArray = sort.Split(',');
                    var orderArray = order.Split(',');

                    for (int i = 0; i < sortArray.Length; i++)
                    {
                        string sortVal = sortArray[i];
                        if (orderArray[i] == "desc")
                        {
                            switch (sortVal)
                            {
                                case "ConnectDuration":
                                    query = query.OrderByDescending(x => x.DisconnectDate.HasValue ? x.ConnectDate - x.DisconnectDate : null);
                                    break;
                                default:
                                    throw new Exception("意外的排序字段");
                                    break;
                            }

                        }
                        else if (orderArray[i] == "asc")
                        {
                            switch (sortVal)
                            {
                                case "ConnectDuration":
                                    query = query.OrderBy(x => x.DisconnectDate.HasValue ? x.ConnectDate - x.DisconnectDate : null);
                                    break;
                                default:
                                    throw new Exception("意外的排序字段");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreateDate);
                    //query = query.OrderByDescending(x => new { x.ConnectDate });
                }

                var result = await query.Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_DeviceBraceletConnectRecord>>(count, result);
            }
        }

        public async Task<bool> CheckUserIdExist(Guid UserId)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBraceletConnectRecord.AnyAsync(a => a.UserId == UserId);
            }
        }
        public async Task<bool> CheckIMEIByNotUserExist(Guid UserId, string IMEI)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                return await db.UT_DeviceBraceletConnectRecord.AnyAsync(a => a.IMEI == IMEI && a.UserId != UserId);
            }
        }
    }
}
