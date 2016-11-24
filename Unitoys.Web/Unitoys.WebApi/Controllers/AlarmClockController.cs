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
    public class AlarmClockController : ApiController
    {
        private IAlarmClockService _alarmClockService;
        public AlarmClockController(IAlarmClockService alarmClockService)
        {
            this._alarmClockService = alarmClockService;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody]AddAlarmClockBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(model.Time))
            {
                return Ok(new { status = 0, msg = "时间不能为空！" });
            }
            if (!model.TimeRange.HasValue)
            {
                return Ok(new { status = 0, msg = "时间范围不能为空！" });
            }
            if (!model.Status.HasValue)
            {
                return Ok(new { status = 0, msg = "状态不能为空！" });
            }
            if (!IsRepeat(model.Repeat))
            {
                return Ok(new { status = 0, msg = "重复参数错误！" });
            }

            var count = await _alarmClockService.GetEntitiesCountAsync(x => x.UserId == currentUser.ID);
            if (count >= 3)
            {
                return Ok(new { status = 0, msg = "目前仅支持最高3个闹钟！" });
            }

            UT_AlarmClock alarmClock = new UT_AlarmClock()
            {
                UserId = currentUser.ID,
                CreateDate = CommonHelper.GetDateTimeInt(),
                UpdateDate = CommonHelper.GetDateTimeInt(),
                TimeRange = model.TimeRange.Value,
                Time = model.Time,
                Repeat = model.Repeat,
                Tag = model.Tag,
                Status = model.Status.Value
            };

            if (await _alarmClockService.InsertAsync(alarmClock))
            {
                return Ok(new { status = 1, msg = "添加成功！", data = new { AlarmClockId = alarmClock.ID } });
            }
            return Ok(new { status = 0, msg = "添加失败！" });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Update([FromBody]AddAlarmClockBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (model.ID == Guid.Empty)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }
            if (string.IsNullOrEmpty(model.Time))
            {
                return Ok(new { status = 0, msg = "时间不能为空！" });
            }
            if (!model.TimeRange.HasValue)
            {
                return Ok(new { status = 0, msg = "时间范围不能为空！" });
            }
            if (!model.Status.HasValue)
            {
                return Ok(new { status = 0, msg = "状态不能为空！" });
            }
            if (!IsRepeat(model.Repeat))
            {
                return Ok(new { status = 0, msg = "重复参数错误！" });
            }

            UT_AlarmClock alarmClock = await _alarmClockService.GetEntityByIdAsync(model.ID);
            if (alarmClock.UserId != currentUser.ID)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }

            alarmClock.TimeRange = model.TimeRange.Value;
            alarmClock.Time = model.Time;
            alarmClock.Repeat = model.Repeat;
            alarmClock.Tag = model.Tag;
            alarmClock.Status = model.Status.Value;

            if (await _alarmClockService.UpdateAsync(alarmClock))
            {
                return Ok(new { status = 1, msg = "更新成功！" });
            }
            return Ok(new { status = 0, msg = "更新失败！" });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatus([FromBody]AddAlarmClockBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (model.ID == Guid.Empty)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }
            else if (!model.Status.HasValue)
            {
                return Ok(new { status = 0, msg = "状态不能为空！" });
            }
            UT_AlarmClock alarmClock = await _alarmClockService.GetEntityByIdAsync(model.ID);
            if (alarmClock.UserId != currentUser.ID)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }

            //alarmClock.TimeRange = model.TimeRange;
            //alarmClock.Time = model.Time.Value;
            //alarmClock.Repeat = model.Repeat;
            //alarmClock.Tag = model.Tag;
            alarmClock.Status = model.Status.Value;

            if (await _alarmClockService.UpdateAsync(alarmClock))
            {
                return Ok(new { status = 1, msg = "更新成功！" });
            }
            return Ok(new { status = 0, msg = "更新失败！" });
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var dataResult = await _alarmClockService.GetEntitiesAsync(x => x.UserId == currentUser.ID);

            var data = from i in dataResult.OrderByDescending(x => x.UpdateDate)
                       select new
                       {
                           AlarmClockId = i.ID,
                           TimeRange = ((int)i.TimeRange).ToString(),
                           Time = i.Time,
                           Repeat = i.Repeat,
                           Tag = i.Tag,
                           Status = ((int)i.Status).ToString(),
                       };

            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 查询已启用闹钟数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetByDisabledNum()
        {
            var currentUser = WebUtil.GetApiUserSession();

            var totalRows = _alarmClockService.GetEntitiesCountAsync(x => x.UserId == currentUser.ID && x.Status == AlarmClockStatus.Enable);

            return Ok(new { status = 1, data = new { totalRows = await totalRows + "" } });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Delete([FromBody]AddAlarmClockBindingModel model)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (model.ID == Guid.Empty)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }
            UT_AlarmClock alarmClock = await _alarmClockService.GetEntityByIdAsync(model.ID);
            if (alarmClock.UserId != currentUser.ID)
            {
                return Ok(new { status = 0, msg = "参数错误！" });
            }

            //alarmClock.TimeRange = model.TimeRange;
            //alarmClock.Time = model.Time.Value;
            //alarmClock.Repeat = model.Repeat;
            //alarmClock.Tag = model.Tag;

            if (await _alarmClockService.DeleteAsync(alarmClock))
            {
                return Ok(new { status = 1, msg = "删除成功！" });
            }
            return Ok(new { status = 0, msg = "删除失败！" });
        }

        private bool IsRepeat(string repeat)
        {
            if (string.IsNullOrEmpty(repeat))
                return true;
            return new System.Text.RegularExpressions.Regex(@"^([0-6],)+[0-6]$|^[0-6]$").IsMatch(repeat);
        }
    }
}
