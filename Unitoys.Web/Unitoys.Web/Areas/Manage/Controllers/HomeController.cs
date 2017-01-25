using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {       
            return View();
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UploadImgAsync()
        {
            HttpPostedFileBase image = Request.Files["Filedata"];

            var resultAsync = await WebUtil.UploadImgAsync(image);

            return Content(resultAsync);

        }


        Unitoys.CiticTelecom.WSApi test = new CiticTelecom.WSApi();
        public async Task<JsonResult> TestWsApi(string method, string imsi, string planName, int noOfDays, int tariff, string currency, int actionCode, string alertType, string authenticationTime)
        {
            string a = select1("tests");
            dynamic model = null;
            switch (method)
            {
                case "PackagePlanAdd":
                    model = await test.PackagePlanAdd(imsi, planName, noOfDays, tariff, currency);
                    break;
                case "PackagePlanActivate":
                    model = await test.PackagePlanActivate(imsi, planName, noOfDays, tariff, currency);
                    break;
                case "PackagePlanQuery":
                    model = await test.PackagePlanQuery(imsi);
                    break;
                case "PackagePlanDelete":
                    model = await test.PackagePlanDelete(imsi, planName);
                    break;
                case "SubscriptionUpdate":
                    model = await test.SubscriptionUpdate(imsi, actionCode);
                    break;
                case "SubscriptionQuery":
                    model = await test.SubscriptionQuery(imsi);
                    break;
                case "KeepAlive":
                    model = await test.KeepAlive();
                    break;
                case "BalanceRemind":
                    model = await test.BalanceRemind(imsi, alertType);
                    break;
                case "DataActivationTimeRemind":
                    model = await test.DataActivationTimeRemind(imsi, authenticationTime);
                    break;
            }

            return Json(new
            {
                success = true,
                data = model
            });
        }
        public static string select1(string content)
        {
            return string.Format(@"
select ST_CODE,ST_VCCONTENT,NO_COUNT,NO_LIMITDATE,ST_TITLE,ST_ONLYID ,CASE NO_TYPE  
        WHEN 1 THEN '驗證碼'          
        WHEN 2 THEN '活動碼'  
        END as NO_TYPE ,ST_VCCONTENT_ENG,ST_VCCONTENT_JP
         from PL_SMSSETTING p
         where UPPER(ST_TITLE)like UPPER('%{0}%') and UPPER(ST_VCCONTENT)like UPPER('%{0}%') and UPPER(ST_VCCONTENT_ENG)like UPPER('%{0}%') and UPPER(ST_VCCONTENT_JP)like UPPER('%{0}%') order by ST_CODE asc", content);
        }
    }
}
