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
    public class SportService : BaseService<UT_Sport>, ISportService
    {
        public async Task<KeyValuePair<int, List<UT_Sport>>> SearchAsync(int page, int rows, string tel, int? createStartDate, int? createEndDate)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_Sport.Include(x => x.UT_Users).Where(x => true);
                if (!string.IsNullOrEmpty(tel))
                {
                    query = query.Where(x => x.UT_Users.Tel.Contains(tel));
                }
                if (createStartDate != null)
                {
                    query = query.Where(x => x.Date >= createStartDate);
                }

                if (createEndDate != null)
                {
                    query = query.Where(x => x.Date <= createEndDate);
                }

                var result = await query.OrderByDescending(x => x.Date).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_Sport>>(count, result);
            }
        }


        public async Task<bool> AddSportAndTimePeriodAsync(Guid userId, int stepNum, int stepTime)
        {
            int LastRecordTotalStepNum = 0; //最后一次记录的总步数

            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1.保存当天运动总数据
                int stepDateInt = CommonHelper.ConvertDateTimeInt(Convert.ToDateTime(CommonHelper.GetTime(stepTime.ToString()).ToString("yyyy-MM-dd")));
                int EndDateInt = CommonHelper.ConvertDateTimeInt(Convert.ToDateTime(CommonHelper.GetTime(stepTime.ToString()).ToString("yyyy-MM-dd")).AddDays(1));

                var sportEntity = await db.UT_Sport.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == stepDateInt);//当天总运动

                //不存在当天运动数据则新增,否则更新
                if (sportEntity == null)
                {
                    sportEntity = new UT_Sport();
                    sportEntity.UserId = userId;
                    sportEntity.Date = stepDateInt;
                    sportEntity.CreateDate = CommonHelper.GetDateTimeInt();
                    sportEntity.StepNum = stepNum;

                    db.UT_Sport.Add(sportEntity);
                }
                else
                {
                    LastRecordTotalStepNum = sportEntity.StepNum;

                    sportEntity.StepNum = stepNum;

                    db.UT_Sport.Attach(sportEntity);
                    db.Entry<UT_Sport>(sportEntity).State = EntityState.Modified;
                }

                //2.保存当天不同时间段运动数据
                var sportTimePeriodEntity = await db.UT_SportTimePeriod
                    .Where(x => x.UserId == userId
                        && x.StartDateTime >= stepDateInt
                        && x.EndDateTime < EndDateInt)
                    .OrderByDescending(x => x.EndDateTime).FirstOrDefaultAsync();

                //如果还未创建时间段数据或最后一次更新时间已超过一定时间
                if (sportTimePeriodEntity == null ||
                    stepTime - sportTimePeriodEntity.EndDateTime > 600)
                {
                    sportTimePeriodEntity = new UT_SportTimePeriod();
                    sportTimePeriodEntity.StepNum = stepNum - LastRecordTotalStepNum;//传入的步数-已记录的总步数=新时间段步数
                    sportTimePeriodEntity.StartDateTime = stepTime;
                    sportTimePeriodEntity.EndDateTime = stepTime;
                    sportTimePeriodEntity.UserId = userId;

                    db.UT_SportTimePeriod.Add(sportTimePeriodEntity);
                }
                else
                {
                    sportTimePeriodEntity.EndDateTime = stepTime;
                    sportTimePeriodEntity.StepNum += stepNum - LastRecordTotalStepNum;//已有步数+(传入的步数-已记录的总步数)=时间段内的最新步数
                    db.UT_SportTimePeriod.Attach(sportTimePeriodEntity);
                    db.Entry<UT_SportTimePeriod>(sportTimePeriodEntity).State = EntityState.Modified;
                }

                return await db.SaveChangesAsync() > 0;
            }
        }


        public async Task<bool> AddSportHistoryAsync(Guid userId, List<int> ToDays, List<int> YesterDays, List<int> BeforeYesterDays, List<int> HistoryDays)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1.提取有效数据
                List<UT_Sport> sportList = new List<UT_Sport>();
                List<UT_SportTimePeriod> sportTimePeriodThreeList = new List<UT_SportTimePeriod>();
                List<UT_SportTimePeriod> sportTimePeriodHistoryList = new List<UT_SportTimePeriod>();

                if (ToDays != null && ToDays.Count > 0)
                {
                    await AddHistorySport(db, ToDays, userId, DateTime.Today, "Today");
                }

                if (YesterDays != null && YesterDays.Count > 0)
                {
                    await AddHistorySport(db, YesterDays, userId, DateTime.Today.AddDays(-1), "YesterDays");
                }

                if (BeforeYesterDays != null && BeforeYesterDays.Count > 0)
                {
                    await AddHistorySport(db, BeforeYesterDays, userId, DateTime.Today.AddDays(-2), "BeforeYesterDays");
                }

                //历史记录为一整天的步数，所以额外处理
                if (HistoryDays != null && HistoryDays.Count > 0)
                {
                    for (int i = 0; i < HistoryDays.Count; i++)
                    {
                        int StartDateTime = CommonHelper.ConvertDateTimeInt(DateTime.Today.AddDays(-3 - i));
                        int EndDateTime = CommonHelper.ConvertDateTimeInt(DateTime.Today.AddDays(-3 - i).AddHours(24).AddSeconds(-1));
                        int StepNum = HistoryDays[i];

                        UT_Sport sport = new UT_Sport();
                        sport.Date = CommonHelper.ConvertDateTimeInt(DateTime.Today.AddDays(-3 - i));
                        sport.StepNum = StepNum;
                        sport.UserId = userId;
                        sport.CreateDate = CommonHelper.GetDateTimeInt();

                        UT_SportTimePeriod sportTimePeriod = new UT_SportTimePeriod();
                        sportTimePeriod.StartDateTime = StartDateTime;
                        sportTimePeriod.EndDateTime = EndDateTime;
                        sportTimePeriod.StepNum = StepNum;
                        sportTimePeriod.UserId = userId;
                        sportTimePeriod.CreateDate = CommonHelper.GetDateTimeInt();

                        //如果存在已上传时间段包含在历史数据里
                        //则 将此次历史数据的开始时间戳设为最后更新时间戳的时间,并且-已记录总步数
                        //否则直接新增
                        var entity = await db.UT_SportTimePeriod.OrderByDescending(x => x.EndDateTime).FirstOrDefaultAsync(x => x.UserId == userId &&
                            x.StartDateTime >= StartDateTime &&
                            x.EndDateTime <= EndDateTime);

                        var sportEntity = await db.UT_Sport.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == sportTimePeriod.StartDateTime);

                        //如果这一天的历史数据存在并且有记录过时间段
                        if (sportEntity != null && entity != null)
                        {
                            //判断已记录过的时间段需小于此次的时间段,防止重复数据
                            //  已记录则不进行任何操作
                            if (entity.EndDateTime < EndDateTime)
                            {
                                //如果历史步数小于记录步数,则是异常的,记录日志
                                if (StepNum < sportEntity.StepNum)
                                {
                                    LoggerHelper.Error(userId + "历史步数小于记录步数" + DateTime.Now);
                                }
                                sportTimePeriod.StepNum -= sportEntity.StepNum;
                                sportTimePeriod.StartDateTime = entity.EndDateTime + 1;
                                db.UT_SportTimePeriod.Add(sportTimePeriod);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            db.UT_SportTimePeriod.Add(sportTimePeriod);
                        }

                        if (sportEntity == null)
                        {
                            db.UT_Sport.Add(sport);
                        }
                        else if (sportEntity.StepNum != StepNum)
                        {
                            sportEntity.StepNum = StepNum;
                            db.UT_Sport.Attach(sportEntity);
                            db.Entry<UT_Sport>(sportEntity).State = EntityState.Modified;
                        }

                    }
                }

                return await db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 新增历史运动记录
        /// </summary>
        /// <param name="daysList">运动日期记录</param>
        /// <param name="userId">用户</param>
        /// <param name="dtDate">日期时间</param>
        /// <param name="sportList">运气日期List</param>
        /// <param name="sportTimePeriodList">运动时间段List</param>
        private async Task<bool> AddHistorySport(UnitoysEntities db, List<int> daysList, Guid userId, DateTime dtDate, string dayType)
        {
            List<UT_SportTimePeriod> sportTimePeriodList = new List<UT_SportTimePeriod>();

            var nowDtInt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            int UpdateStepNum = 0;

            //1.处理时间段数据

            /*  如果历史时间段包括在已记录时间段中
                *  则：  跳过此条数据
                *  否则：新增
             */
            for (int i = 0; i < daysList.Count; i++)
            {
                int StartDateTime = CommonHelper.ConvertDateTimeInt(dtDate.AddHours(i));
                int EndDateTime = CommonHelper.ConvertDateTimeInt(dtDate.AddHours(i + 1).AddSeconds(-1));
                int StepNum = daysList[i];

                //如果结束时间段大于当前时间则是无效的
                if (dayType == "Today")
                {
                    if (EndDateTime > nowDtInt)
                    {
                        break;
                    }
                }

                //判断如果此时间段没上传过，才新增
                var entity = await db.UT_SportTimePeriod.FirstOrDefaultAsync(x => x.UserId == userId &&
                    x.StartDateTime == StartDateTime &&
                    x.EndDateTime == EndDateTime);

                if (entity == null)
                {
                    UT_SportTimePeriod sportTimePeriod = new UT_SportTimePeriod();
                    sportTimePeriod.StartDateTime = CommonHelper.ConvertDateTimeInt(dtDate.AddHours(i));
                    sportTimePeriod.EndDateTime = CommonHelper.ConvertDateTimeInt(dtDate.AddHours(i + 1).AddSeconds(-1));
                    sportTimePeriod.StepNum = daysList[i];
                    sportTimePeriod.UserId = userId;
                    sportTimePeriod.CreateDate = CommonHelper.GetDateTimeInt();

                    sportTimePeriodList.Add(sportTimePeriod);
                }
                //如果是当前时间段则允许更新
                else if (StartDateTime <= nowDtInt && (EndDateTime >= nowDtInt || EndDateTime >= nowDtInt - 3600) && entity.StepNum < StepNum)
                {
                    UpdateStepNum += StepNum - entity.StepNum;
                    entity.StepNum = StepNum;

                    db.UT_SportTimePeriod.Attach(entity);
                    db.Entry<UT_SportTimePeriod>(entity).State = EntityState.Modified;
                }
            }

            //如果不需要新增时间段则表示此天数据不需要进行任何处理
            if (sportTimePeriodList.Count <= 0)
            {
                return false;
            }

            //2.处理日期数据
            int sportDate = CommonHelper.ConvertDateTimeInt(dtDate);

            UT_Sport sport = await db.UT_Sport.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == sportDate);

            if (sport == null)
            {
                sport = new UT_Sport();
                sport.Date = sportDate;
                sport.StepNum = sportTimePeriodList.Sum(x => x.StepNum);// daysList.Sum(x => x);
                sport.UserId = userId;
                sport.CreateDate = CommonHelper.GetDateTimeInt();

                db.UT_Sport.Add(sport);
            }
            else
            {
                sport.StepNum += sportTimePeriodList.Sum(x => x.StepNum) + UpdateStepNum;

                db.UT_Sport.Attach(sport);
                db.Entry<UT_Sport>(sport).State = EntityState.Modified;
            }

            foreach (var entity in sportTimePeriodList)
            {
                db.UT_SportTimePeriod.Add(entity);
            }

            return true;
        }
    }
}
