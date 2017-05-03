using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_OperationRecord)]
    public class OperationRecordController : BaseController
    {
        private IOperationRecordService _operationRecordService;

        public OperationRecordController(IOperationRecordService OperationRecordService)
        {
            this._operationRecordService = OperationRecordService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogIndex()
        {
            return View();
        }
        /// <summary>
        /// 获取操作记录列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetList(int page, int rows, string url, string parameter, string managerLoginName, DateTime? createStartDate, DateTime? createEndDate)
        {
            int? createStartDateInt = null;
            int? createEndDateInt = null;
            if (createStartDate.HasValue)
            {
                createStartDateInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (createEndDate.HasValue)
            {
                createEndDateInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }
            var pageRowsDb = await _operationRecordService.SearchAsync(page, rows, url, parameter, managerLoginName, createStartDateInt, createEndDateInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value as List<UT_OperationRecord>
                           select new
                           {
                               ID = i.ID,
                               Url = i.Url,
                               Parameter = i.Parameter,
                               Data = i.Data,
                               Response = i.Response,
                               CreateDate = i.CreateDate.ToString(),
                               CreateManageUsersName = i.UT_ManageUsers.LoginName,
                               Remark = i.Remark,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解除用户锁定状态
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <param name="TrueName"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_OperationRecord)]
        public async Task<ActionResult> SetRemark(Guid ID, string Remark)
        {
            JsonAjaxResult result = new JsonAjaxResult();
            if (ID != Guid.Empty)
            {
                UT_OperationRecord entity = await _operationRecordService.GetEntityByIdAsync(ID);
                if (!string.IsNullOrEmpty(Remark))
                {
                    entity.Remark = Remark;
                    if (await _operationRecordService.UpdateAsync(entity))
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
                    result.Msg = "备注不能为空！";
                }
            }
            else
            {
                result.Success = false;
                result.Msg = "请求失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 文件日志搜索功能


        public PartialViewResult PartialLogSearch(string search, string prefixPath, DateTime? createStartDate, DateTime? createEndDate)
        {
            DateTime BeginDT = DateTime.Now;
            DateTime EndDT = DateTime.Now;
            if (createStartDate.HasValue)
            {
                BeginDT = createStartDate.Value;
            }
            if (createEndDate.HasValue)
            {
                EndDT = createEndDate.Value;
            }

            int days = (EndDT - BeginDT).Days;
            DateTime DTSearch = BeginDT;

            LogFileModel model = new LogFileModel();
            StringBuilder sb = new StringBuilder();

            if (days > 0)
            {
                DTSearch = DTSearch.AddDays(-1);
                for (int i = 0; i < days; i++)
                {
                    DTSearch = DTSearch.AddDays(1);
                    sb.AppendLine("日期：" + DTSearch.ToString("yyyyMMdd"));
                    var textArray = GetTextArray("记录时间",
                                                Server.MapPath(
                                                    string.Format("/Log/{3}/{0}/{1}/{2}.txt", DTSearch.ToString("yyyy"), DTSearch.ToString("yyyyMM"), DTSearch.ToString("yyyyMMdd"), prefixPath)
                                                      )
                                                , search);

                    sb.AppendLine(string.Join("记录时间", textArray == null || textArray.Count == 0 ? new List<string>() { DTSearch.ToString("yyyyMMdd") + "无记录" } : textArray));
                }
            }
            else
            {
                sb.AppendLine("日期：" + DTSearch.ToString("yyyyMMdd"));
                var textArray = GetTextArray("记录时间",
                                              Server.MapPath(
                                                  string.Format("/Log/{3}/{0}/{1}/{2}.txt", DTSearch.ToString("yyyy"), DTSearch.ToString("yyyyMM"), DTSearch.ToString("yyyyMMdd"), prefixPath)
                                                    )
                                              , search);
                sb.AppendLine(string.Join("记录时间", textArray == null || textArray.Count == 0 ? new List<string>() { DTSearch.ToString("yyyyMMdd") + "无记录" } : textArray));
            }

            model.Text = sb.ToString();

            return PartialView(model);
        }

        /// <summary>
        /// 获取文本数组
        /// </summary>
        /// <param name="path"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private List<string> GetTextArray(string splitRow, string path, string search)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            //ReadWrite,允许读取被占用的文件
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    // 读一行 
                    //string nextLine = sr.ReadLine();

                    // 全部读完 
                    string restOfStream = sr.ReadToEnd();

                    var textArray = restOfStream.Split(new[] { splitRow }, StringSplitOptions.None).ToList();

                    //搜索
                    if (!string.IsNullOrEmpty(search))
                    {
                        //空格隔开，认为要求是全部符合的
                        var searchText = search.Split(' ').ToList();

                        if (searchText != null && searchText.Count > 0)
                            textArray = textArray.Where(x => searchText.Count(a => x.Contains(a)) == searchText.Count).ToList();
                    }
                    return textArray;
                }
            }

        }

        #endregion
    }
    public class LogFileModel
    {
        public string Text { get; set; }
    }
}
