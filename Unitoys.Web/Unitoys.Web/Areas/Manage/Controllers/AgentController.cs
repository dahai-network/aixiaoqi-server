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
    [RequireRolesOrPermissions(UnitoysPermissionStore.Can_View_Agent)]
    public class AgentController : BaseController
    {
        private IAgentService _agentService;

        public AgentController() { }

        public AgentController(IAgentService agentService)
        {
            this._agentService = agentService;
        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetList(int page, int rows, string companyName, DateTime? createStartDate, DateTime? createEndDate)
        {
            int? beginTimeInt = null;
            int? endTimeInt = null;
            if (createStartDate.HasValue)
            {
                beginTimeInt = CommonHelper.ConvertDateTimeInt(createStartDate.Value);
            }
            if (endTimeInt.HasValue)
            {
                endTimeInt = CommonHelper.ConvertDateTimeInt(createEndDate.Value);
            }
            var pageRowsDb = await _agentService.SearchAsync(page, rows, companyName, beginTimeInt, endTimeInt);

            int totalNum = pageRowsDb.Key;

            //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb.Value
                           select new
                           {
                               ID = i.ID,
                               CompanyName = i.CompanyName,
                               Area = i.Area,
                               RegisteredAddress = i.RegisteredAddress,
                               CorporateRepresentative = i.CorporateRepresentative,
                               CompanyRegistrationTime = i.CompanyRegistrationTime.ToString(),
                               CorporationNature = i.CorporationNature,
                               MarketPersonnelNum = i.MarketPersonnelNum,
                               SalerPersonnelNum = i.SalerPersonnelNum,
                               AfterSalesPersonalNum = i.AfterSalesPersonalNum,
                               ScopeBusiness = i.ScopeBusiness,
                               SalesArea = i.SalesArea,
                               AnnualSales = i.AnnualSales,
                               MainBusiness = i.MainBusiness,
                               MainClientCategory = i.MainClientCategory,
                               Contact = i.Contact,
                               ContactMobilePhone = i.ContactMobilePhone,
                               EMail = i.EMail,
                               QQWeChat = i.QQWeChat,
                               Tel = i.Tel,
                               FAX = i.FAX,
                               CompanyWebSite = i.CompanyWebSite,
                               IsAgentOtherProducts = i.IsAgentOtherProducts,
                               OtherProducts = i.OtherProducts,
                               DevelopMarketAssumptions = i.DevelopMarketAssumptions,
                               CompanyChannelsResources = i.CompanyChannelsResources,
                               CooperationSuggestion = i.CooperationSuggestion,
                               Status = i.Status,
                               CreateDate = i.CreateDate,
                           };

            var jsonResult = new { total = totalNum, rows = pageRows };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Modify_Agent)]
        public async Task<ActionResult> Update(UT_Agent model)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            UT_Agent entity = await _agentService.GetEntityByIdAsync(model.ID);
            entity.Status = model.Status;
           
            if (await _agentService.UpdateAsync(entity))
            {
                result.Success = true;
                result.Msg = "更新成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "更新失败！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [RequireRolesOrPermissions(UnitoysPermissionStore.Can_Delete_Agent)]
        public async Task<ActionResult> Delete(Guid? ID)
        {
            JsonAjaxResult result = new JsonAjaxResult();

            if (ID.HasValue)
            {
                UT_Agent modal = await _agentService.GetEntityByIdAsync(ID.Value);
                await _agentService.DeleteAsync(modal);
                result.Success = true;
                result.Msg = "删除成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "参数错误！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
