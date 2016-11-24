using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Web.Areas.wx.Controllers
{
    [AllowAnonymous]
    public class SpeakRecordController : Controller
    {
        private IUserService _userService;
        private ISpeakRecordService _speakRecordService;
        public SpeakRecordController(IUserService userService,ISpeakRecordService speakRecordService)
        {
            this._speakRecordService = speakRecordService;
            this._userService = userService;
        }
        /// <summary>
        /// 通话账单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {

            var openid = WebHelper.GetCookie("openid");

            UT_Users user = await _userService.GetEntityByOpenIdAsync(openid);

            var speakRecords = await _speakRecordService.GetEntitiesAsync(x => x.UserId == user.ID &&x.CallSessionTime > 0);

            return View(speakRecords);
        }
	}
}