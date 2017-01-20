using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class SportController : ApiController
    {
        private ISportService _sportService;
        private ISportTimePeriodService _sportTimePeriodService;
        public SportController(ISportService sportService, ISportTimePeriodService sportTimePeriodService)
        {
            this._sportService = sportService;
            this._sportTimePeriodService = sportTimePeriodService;
        }

        /// <summary>
        /// 添加运动记录
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddHistorys([FromBody]AddSportAndTimePeriodBindingModels model)
        {
            LoggerHelper.Info("AddHistorys" + DateTime.Now.ToString());
            LoggerHelper.Info(System.Web.HttpContext.Current.Request.Form.ToString());
            LoggerHelper.Info("AddHistorys：SerializeObject" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
            LoggerHelper.Info("SerializeObject End");
            LoggerHelper.Info("BodyInputStream:" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());
            LoggerHelper.Info("Content:" + Request.Content.ReadAsStringAsync().Result);


            var currentUser = WebUtil.GetApiUserSession();

            if (model == null)
            {
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "没有任何数据"));
            }
            else
            {
                if (await _sportService.AddSportHistoryAsync(currentUser.ID, model.ToDays, model.YesterDays, model.BeforeYesterDays, model.HistoryDays))
                {
                    return Ok(new { status = 1, msg = "添加成功！" });
                }
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "添加失败！"));
        }

        /// <summary>
        /// 获取运动记录,根据时间
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetTimePeriodByDate(int Date)
        {
            var currentUser = WebUtil.GetApiUserSession();

            //.ToString("yyyy-MM-dd")是为了避免传递的值不正确
            int BeginDateInt = CommonHelper.ConvertDateTimeInt(Convert.ToDateTime(CommonHelper.GetTime(Date.ToString()).ToString("yyyy-MM-dd")));
            int EndDateInt = CommonHelper.ConvertDateTimeInt(Convert.ToDateTime(CommonHelper.GetTime(Date.ToString()).ToString("yyyy-MM-dd")).AddDays(1));

            var result = await _sportTimePeriodService.GetEntitiesAsync(x => x.UserId == currentUser.ID
                && x.StartDateTime >= BeginDateInt
                && x.EndDateTime < EndDateInt
                && x.StepNum > 0);

            var sport = await _sportService.GetEntityAsync(x => x.Date == BeginDateInt && x.UserId == currentUser.ID && x.StepNum > 0);

            var data = from i in result.OrderBy(x => x.StartDateTime)
                       select new
                       {
                           StartDateTime = i.StartDateTime.ToString(),
                           EndDateTime = i.EndDateTime.ToString(),
                           StepNum = i.StepNum.ToString(),
                           Minute = Math.Floor(Convert.ToDouble((i.EndDateTime - i.StartDateTime) / 60)).ToString(),
                           KM = GetSportKm(i.StepNum).ToString(),//公里
                           //卡路里（kcal）＝体重（kg）×距离（公里）×1.036
                           Kcal = GetSportKcal(currentUser, i.StepNum).ToString()//卡路里
                       };

            return Ok(new
            {
                status = 1,
                data = new
                {
                    TotalStepNum = sport == null ? "" : sport.StepNum.ToString(),
                    TimePeriods = data
                }
            });
        }

        /// <summary>
        /// 获取公里数
        /// 步数*步距(当前默认0.75/米)/1000
        /// </summary>
        /// <param name="StepNum">步数</param>
        /// <returns></returns>
        private static double GetSportKm(int StepNum)
        {
            //0.75,为默认步距
            //后续考虑根据身高计算步距公式来进行计算
            return Math.Round(StepNum * 0.683 / 1000, 1);
        }

        /// <summary>
        /// 获取卡路里
        /// 体重(kg)(30或以下默认为30)*公里*1.036
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="StepNum"></param>
        /// <returns></returns>
        private static double GetSportKcal(LoginUserInfo currentUser, int StepNum)
        {
            double km = (StepNum * 0.683 / 1000);
            if (currentUser.Weight > 0 && currentUser.Weight <= 30)
            {
                return Math.Round((double)30 * km * 1.036, 1);
            }
            else
            {
                return Math.Round((double)currentUser.Weight * km * 1.036, 1);
            }
            //return currentUser.Weight * (i.StepNum * 0.75 * 1000) * 1.036.ToString();
        }

        /// <summary>
        /// 获取运动总量
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetSportTotal()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var sport = await _sportService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            if (sport != null && sport.Count() > 0)
            {
                var data = new
                {
                    Date = sport.Count().ToString(),//总天数
                    StepNum = sport.Sum(x => x.StepNum).ToString(),//总步数
                    KM = GetSportKm(sport.Sum(x => x.StepNum)).ToString(),//公里
                    //卡路里（kcal）＝体重（kg）×距离（公里）×1.036
                    Kcal = GetSportKcal(currentUser, sport.Sum(x => x.StepNum)).ToString()//卡路里
                };

                return Ok(new
                {
                    status = 1,
                    data = data
                });
            }
            return Ok(new
            {
                status = 1,
                data = new { }
            });
        }

        /// <summary>
        /// 获取记录的运动日期
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="days">天数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetRecordDate(int startDate, int days = 31)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                return Ok(new { status = 0, msg = "查询失败！" + "用户不能为空！" });
            }

            //.ToString("yyyy-MM-dd")是为了避免传递的值不正确
            int EndDateInt = CommonHelper.ConvertDateTimeInt(Convert.ToDateTime(CommonHelper.GetTime(startDate.ToString()).ToString("yyyy-MM-dd")).AddDays(days));

            var result = await _sportService.GetEntitiesAsync(x => x.UserId == currentUser.ID
                && x.Date >= startDate
                && x.Date <= EndDateInt
                && x.StepNum > 0);

            return Ok(new
            {
                status = 1,
                data = result.OrderBy(x => x.Date).Select(x => new { Date = x.Date.ToString() }).ToArray()
            });
        }

        /// <summary>
        /// 添加运动记录
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [NonAction]
        public async Task<IHttpActionResult> Add([FromBody]AddSportAndTimePeriodBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (await _sportService.AddSportAndTimePeriodAsync(currentUser.ID, model.StepNum, model.StepTime))
            {
                return Ok(new { status = 1, msg = "添加成功！" });
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败, "添加失败"));
        }

    }


    public class AddSportAndTimePeriodBindingModels
    {
        /// <summary>
        /// 今天
        /// </summary>
        public List<int> ToDays { get; set; }
        /// <summary>
        /// 昨天
        /// </summary>
        public List<int> YesterDays { get; set; }
        /// <summary>
        /// 前天
        /// </summary>
        public List<int> BeforeYesterDays { get; set; }
        /// <summary>
        /// 三天前的
        /// </summary>
        public List<int> HistoryDays { get; set; }
    }

}
