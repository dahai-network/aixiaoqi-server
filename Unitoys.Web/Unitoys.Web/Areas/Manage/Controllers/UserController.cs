using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.Core.Security;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_User)]
    public class UserController : BaseController
    {
        private IUserService _userService;
        private IPaymentService _paymentService;
        private IOrderService _orderService;
        private ISMSConfirmationService _smsConfirmationService;
        private IDeviceBraceletConnectRecordService _deviceBraceletConnectRecordService;

        public UserController() { }

        public UserController(IUserService userService, IPaymentService paymentService, IOrderService orderService, ISMSConfirmationService smsConfirmationService, IDeviceBraceletConnectRecordService deviceBraceletConnectRecordService)
        {
            this._userService = userService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._smsConfirmationService = smsConfirmationService;
            this._deviceBraceletConnectRecordService = deviceBraceletConnectRecordService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate, int? status)
        {
            var pageRowsDb = await _userService.SearchAsync(page, rows, tel, createStartDate, createEndDate, status);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_Users>
                           select new
                           {
                               ID = i.ID,
                               Tel = i.Tel,
                               Email = i.Email,
                               TrueName = i.TrueName,
                               QQ = i.QQ,
                               CreateDate = i.CreateDate.ToString(),
                               Status = i.Status,
                               Score = i.Score,
                               Sex = i.Sex,
                               Amount = i.Amount,
                               Remark = i.Remark
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSelectList()
        {
            Expression<Func<UT_Users, bool>> exp = a => 1 == 1;

            var pageRowsDb = await _userService.GetEntitiesAsync(exp);

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb
                           select new
                           {
                               ID = i.ID,
                               Tel = i.Tel
                           };

            var jsonResult = new { pageRows };

            return Json(pageRows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="passWord"></param>
        /// <param name="trueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Add_User)]
        public async Task<ActionResult> Add(string passWord, string tel, string email = "", string trueName = "")
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (passWord.Trim() == "" || passWord.Length < 6)
            {
                result.Success = false;
                result.Msg = "密码不符合规范！";
            }
            else if (!ValidateHelper.IsMobile(tel))
            {
                result.Success = false;
                result.Msg = "电话号码不正确！";
            }
            else if (_userService.CheckTelExist(tel))
            {
                result.Success = false;
                result.Msg = "该号码已绑定其他用户！";
            }
            else
            {

                UT_Users user = new UT_Users();
                user.PassWord = SecureHelper.MD5(passWord);
                user.Tel = tel;
                user.Email = string.IsNullOrEmpty(email) ? "" : email;
                user.TrueName = string.IsNullOrEmpty(trueName) ? "" : trueName;
                user.Score = 0;
                user.Amount = 0;
                user.GroupId = Guid.Parse("688a3245-2628-4488-bf35-9c029ff80988");
                user.Status = 0;
                user.CreateDate = DateTime.Now;
                user.UserHead = "/Unitoys/2015/12/1512291755292460937.png";

                switch (Unitoys.WebApi.Controllers.PhoneServerByMySqlServices.SetSip_Buddies(user.Tel))
                {
                    case 2:
                        result.Success = true;
                        result.Msg = "系统繁忙_请重试！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    //return Ok(new StatusCodeRes(StatusCodeType.系统繁忙_请重试));
                    case 0:
                        result.Success = true;
                        result.Msg = "注册失败_请重试！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    //return Ok(new StatusCodeRes(StatusCodeType.注册失败_请重试));
                }

                if (await _userService.InsertAsync(user))
                {
                    //默认运动目标8000
                    if (await _userService.ModifyUserInfoAndUserShape(user.ID, null, null, null, null, null, 8000))
                    {

                    }
                    result.Success = true;
                    result.Msg = "添加成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "操作失败！";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> Lock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {

                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                if (user.Status != 1)
                {
                    user.Status = 1;
                    if (await _userService.UpdateAsync(user))
                    {
                        string userToken = WebUtil.GetApiKeyByTel(user.Tel);
                        if (!string.IsNullOrEmpty(userToken))
                            WebUtil.RemoveApiUserSession(userToken);
                        result.Success = true;
                        result.Msg = "锁定操作成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "锁定操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该用户已经是锁定状态！";
                }

            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解除用户锁定状态
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> uLock(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                if (user.Status != 0)
                {
                    user.Status = 0;
                    if (await _userService.UpdateAsync(user))
                    {
                        result.Success = true;
                        result.Msg = "更新成功！";
                    }
                    else
                    {
                        result.Success = false;
                        result.Msg = "操作失败！";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Msg = "该用户已经是正常状态！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> recharge(Guid? ID, decimal price)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                if (await _userService.Recharge(ID.Value, price))
                {
                    result.Success = true;
                    result.Msg = "更新成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "操作失败！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_User)]
        public async Task<ActionResult> setRemark(Guid? ID, string remark)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID.HasValue)
            {
                UT_Users user = await _userService.GetEntityByIdAsync(ID.Value);
                user.Remark = remark;
                if (await _userService.UpdateAsync(user))
                {
                    result.Success = true;
                    result.Msg = "更新成功！";
                }
                else
                {
                    result.Success = false;
                    result.Msg = "操作失败！";
                }

            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取图标的配置参数
        /// </summary>
        /// <param name="createStartDate">开始时间</param>
        /// <param name="createEndDate">结束时间</param>
        /// <returns></returns>
        public async Task<ActionResult> GetOptionByBarChart(DateTime? createStartDate, DateTime? createEndDate)
        {
            var listUser = (await _userService.GetEntitiesAsync(x =>
                (
                createStartDate.HasValue ? x.CreateDate >= createStartDate.Value : true)
                && (createEndDate.HasValue ? x.CreateDate <= createEndDate.Value : true)
                )).OrderByDescending(x => x.CreateDate);

            var listPayment = (await _paymentService.GetEntitiesAsync(x =>
                (
                createStartDate.HasValue ? x.CreateDate >= createStartDate.Value : true)
                && (createEndDate.HasValue ? x.CreateDate <= createEndDate.Value : true)
                && x.Status == 1
                )).OrderByDescending(x => x.CreateDate);

            var createStartDateInt = createStartDate.HasValue ? CommonHelper.ConvertDateTimeInt(createStartDate.Value) : 0;
            var createEndDateInt = createEndDate.HasValue ? CommonHelper.ConvertDateTimeInt(createEndDate.Value) : 0;

            var listOrderYesPayment = (await this._orderService.GetEntitiesAsync(x =>
               (
               createStartDate.HasValue ? x.OrderDate >= createStartDateInt : true)
               && (createEndDate.HasValue ? x.OrderDate <= createEndDateInt : true)
               && x.PayStatus == PayStatusType.YesPayment
               )).OrderByDescending(x => x.OrderDate);

            var listOrderActivation = (await _orderService.GetEntitiesAsync(x =>
              (
              x.ActivationDate.HasValue
              && createStartDate.HasValue ? x.ActivationDate >= createStartDateInt : true)
              && (createEndDate.HasValue ? x.ActivationDate <= createEndDateInt : true)
              )).OrderByDescending(x => x.ActivationDate);

            var listSmsConfirmation = (await _smsConfirmationService.GetEntitiesAsync(x =>
               (
               createStartDate.HasValue ? x.CreateDate >= createStartDate.Value : true)
               && (createEndDate.HasValue ? x.CreateDate <= createEndDate.Value : true)
               )).OrderByDescending(x => x.CreateDate);

            var listDeviceBraceletConnectRecord = (await _deviceBraceletConnectRecordService.GetEntitiesAsync(x =>
              (
              createStartDate.HasValue ? x.ConnectDate >= createStartDateInt : true)
              && (createEndDate.HasValue ? x.ConnectDate <= createEndDateInt : true)
              )).Distinct(new DeviceBraceletConnectRecordNoComparer()).OrderByDescending(x => x.CreateDate);

            var listDeviceBraceletConnectRecordOnLineTime = (await _deviceBraceletConnectRecordService.GetEntitiesAsync(x =>
              (
              createStartDate.HasValue ? x.DisconnectDate >= createStartDateInt : true)
              && (createEndDate.HasValue ? x.DisconnectDate <= createEndDateInt : true)
              )).OrderByDescending(x => x.CreateDate);

            var opUser = GetChartOption(
                "注册用户量",
                listUser.GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
                new List<ChartSeriesModel>(){
                    new ChartSeriesModel(){
                        name = "注册用户量",
                        type = "bar",
                        data = listUser.GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
                    }
                });

            var opPayment = GetChartOption(
                "充值金额",
                listPayment.GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
                new List<ChartSeriesModel>(){
                    new ChartSeriesModel(){
                        name = "充值金额",
                        type = "bar",
                        data = listPayment.GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Sum(c=>c.Amount) + "").ToList()
                    }
                });

            var opOrderYesPayment = GetChartOption(
               "订单付款量",
               listOrderYesPayment.GroupBy(x => CommonHelper.GetTime(x.OrderDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
               new List<ChartSeriesModel>(){
                    new ChartSeriesModel(){
                        name = "订单付款量",
                        type = "bar",
                        data = listOrderYesPayment.GroupBy(x => CommonHelper.GetTime(x.OrderDate + "").ToString("yyyy-MM-dd")).Select(x => x.Count()).ToList()
                    }
                });

            var opOrderActivation = GetChartOption(
               "订单激活量",
               listOrderActivation.GroupBy(x => CommonHelper.GetTime(x.ActivationDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
               new List<ChartSeriesModel>(){
                    new ChartSeriesModel(){
                        name = "订单激活量",
                        type = "bar",
                        data = listOrderActivation.GroupBy(x => CommonHelper.GetTime(x.ActivationDate + "").ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
                    }
                });

            var opOrderSmsConfirmation = GetChartOption(
               "验证短信量",
               listSmsConfirmation.GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
               new List<ChartSeriesModel>(){
                    new ChartSeriesModel(){
                        name = "注册",
                        type = "bar",
                        data = listSmsConfirmation.Where(x=>x.Type==1).GroupBy(x =>x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
                    }
                    ,new ChartSeriesModel(){
                        name = "忘记密码",
                        type = "bar",
                        data = listSmsConfirmation.Where(x=>x.Type==2).GroupBy(x =>x.CreateDate.ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
                    }
                });

            //存在的客户端类型
            var clientTypeDistinctList = listDeviceBraceletConnectRecord.Select(x => x.ClientType).Distinct();

            #region 设备用户注册量

            var listOpDeviceBraceletUserRegSuccess = new List<ChartSeriesModel>();
            foreach (var item in clientTypeDistinctList)
            {
                var clientTypeDescr = (item.HasValue ? item.Value.ToString() : "无识别") + "-";

                listOpDeviceBraceletUserRegSuccess.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "注册",
                    type = "bar",
                    data = listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Where(a => a.ClientType == item).Select(a => a.UserId).Distinct().Count() + "").ToList()
                });
                listOpDeviceBraceletUserRegSuccess.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "注册成功",
                    type = "bar",
                    data = listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Where(a => a.ClientType == item && a.RegSuccessDate.HasValue).Select(a => a.UserId).Distinct().Count() + "").ToList()
                });
            }

            var opDeviceBraceletUserRegSuccess = GetChartOption(
             "卡-注册人数",
             listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
             listOpDeviceBraceletUserRegSuccess);

            #endregion

            #region 设备注册量

            var listOpDeviceBraceletRegCount = new List<ChartSeriesModel>();
            foreach (var item in clientTypeDistinctList)
            {
                var clientTypeDescr = (item.HasValue ? item.Value.ToString() : "无识别") + "-";

                listOpDeviceBraceletRegCount.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "注册成功",
                    type = "bar",
                    data = listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Where(a => a.ClientType == item && a.RegSuccessDate.HasValue).Count() + "").ToList()
                });
                listOpDeviceBraceletRegCount.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "注册失败",
                    type = "bar",
                    data = listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Where(a => a.ClientType == item && !a.RegSuccessDate.HasValue).Count() + "").ToList()
                });
            }

            var opDeviceBraceletRegCount = GetChartOption(
            "卡-注册量",
            listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
            listOpDeviceBraceletRegCount);

            #endregion

            #region 卡掉线次数

            var listOpDeviceBraceletDisconnectDateCount = new List<ChartSeriesModel>();
            foreach (var item in clientTypeDistinctList)
            {
                var clientTypeDescr = (item.HasValue ? item.Value.ToString() : "无识别") + "-";

                listOpDeviceBraceletDisconnectDateCount.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "掉线次数",
                    type = "bar",
                    data = listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Where(a => a.ClientType == item == a.DisconnectDate.HasValue).Count() + "").ToList()
                });
            }

            var opDeviceBraceletDisconnectDateCount = GetChartOption(
           "卡-掉线次数",
           listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
           listOpDeviceBraceletDisconnectDateCount);

            #endregion

            #region 卡平均在线时间

            var listOpDeviceBraceletOnLineTime = new List<ChartSeriesModel>();
            foreach (var item in clientTypeDistinctList)
            {
                var clientTypeDescr = (item.HasValue ? item.Value.ToString() : "无识别") + "-";

                listOpDeviceBraceletOnLineTime.Add(new ChartSeriesModel()
                {
                    name = clientTypeDescr + "平均在线",
                    type = "bar",
                    data = listDeviceBraceletConnectRecordOnLineTime.GroupBy(x => CommonHelper.GetTime(x.DisconnectDate + "").ToString("yyyy-MM-dd")).Select(x =>
                        Convert.ToInt32(x.Where(a => a.ClientType == item == a.DisconnectDate.HasValue).GroupBy(a => a.SessionId).Average(a => a.OrderByDescending(b => b.DisconnectDate).FirstOrDefault().DisconnectDate - a.OrderBy(b => b.ConnectDate).FirstOrDefault().ConnectDate)) / 60
                        ).ToList()
                });
            }

            var opDeviceBraceletOnLineTime = GetChartOption(
           "卡-单次在线时长平均值",
           listDeviceBraceletConnectRecordOnLineTime.GroupBy(x => CommonHelper.GetTime(x.DisconnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
           listOpDeviceBraceletOnLineTime);

            #endregion

            //var opDeviceBraceletRegCount = GetChartOption(
            //"设备注册量",
            //listDeviceBraceletConnectRecord.GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Key).ToList(),
            //new List<ChartSeriesModel>(){
            //        new ChartSeriesModel(){
            //            name = "注册成功",
            //            type = "bar",
            //            data = listDeviceBraceletConnectRecord.Where(x=>x.RegSuccessDate.HasValue).GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
            //        },new ChartSeriesModel(){
            //            name = "注册失败",
            //            type = "bar",
            //            data = listDeviceBraceletConnectRecord.Where(x=>!x.RegSuccessDate.HasValue).GroupBy(x => CommonHelper.GetTime(x.ConnectDate + "").ToString("yyyy-MM-dd")).Select(x => x.Count() + "").ToList()
            //        }
            //    });

            return Json(new
            {
                opUser = opUser,
                opPayment = opPayment,
                opOrderYesPayment = opOrderYesPayment,
                opOrderActivation = opOrderActivation,
                opOrderSmsConfirmation = opOrderSmsConfirmation,
                opDeviceBraceletUserRegSuccess = opDeviceBraceletUserRegSuccess,
                opDeviceBraceletRegCount = opDeviceBraceletRegCount,
                opDeviceBraceletDisconnectDateCount = opDeviceBraceletDisconnectDateCount,
                opDeviceBraceletOnLineTime = opDeviceBraceletOnLineTime

            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取报表配置项
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="names"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private dynamic GetChartOption(string title, List<string> xAxisNames, List<ChartSeriesModel> seriesList, string formatter = "")
        {
            var option = new
            {
                title = new { text = title },
                tooltip = new { trigger = "axis", formatter = formatter },
                legend = new { data = title },
                xAxis = new { data = xAxisNames },
                yAxis = new { },
                series = seriesList
            };
            return option;
        }
        /// <summary>
        /// 人类可识别的时间大小
        /// </summary>
        /// <param name="seconds">总秒数</param>
        /// <returns></returns>
        private string GetHumanTime(int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, seconds);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (ts.Days > 0)
            {
                sb.Append((int)ts.TotalDays + "天");
            }
            if (ts.Hours > 0)
            {
                sb.Append(ts.Hours + "小时");
            }
            if (ts.Minutes > 0)
            {
                sb.Append(ts.Minutes + "分");
            }
            if (ts.Seconds > 0)
            {
                sb.Append(ts.Seconds + "秒");
            }
            return sb.ToString();
        }
    }
    public class ChartSeriesModel
    {
        public string name { get; set; }
        public string type { get; set; }
        public dynamic data { get; set; }
        //public List<double> data { get; set; }
    }

    // <summary>
    /// 去"重复"时候的比较器(只要ProductNo相同，即认为是相同记录)
    /// </summary>
    public class DeviceBraceletConnectRecordNoComparer : IEqualityComparer<UT_DeviceBraceletConnectRecord>
    {
        public bool Equals(UT_DeviceBraceletConnectRecord p1, UT_DeviceBraceletConnectRecord p2)
        {
            if (p1 == null)
                return p2 == null;
            return p1.SessionId == p2.SessionId;
        }

        public int GetHashCode(UT_DeviceBraceletConnectRecord p)
        {
            if (p == null)
                return 0;
            return p.SessionId.GetHashCode();
        }
    }
}
