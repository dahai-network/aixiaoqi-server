using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi.Controllers
{
    public class LoginRecordController:ApiController
    {
        private IUserLoginRecordService _loginRecordService;
        public LoginRecordController(IUserLoginRecordService loginRecordService)
        {
            this._loginRecordService = loginRecordService;
        }

        public async Task<IHttpActionResult> Get(int page, int rows)
        {
            LoginUserInfo model = WebUtil.GetApiUserSession();

            Expression<Func<UT_UserLoginRecord, object>> orderName = a => a.LoginDate.ToString();

            Expression<Func<UT_UserLoginRecord, bool>> exp = a => a.UserId == model.ID;

            //建立分页Task和计算总数Task，同时运行，提高效率。
            var pagerTask = _loginRecordService.GetEntitiesForPagingAsync(page, rows, orderName, "desc", exp);

            var calculateCountTask = _loginRecordService.GetEntitiesCountAsync(exp);

            //等待异步的完成获取结果。
            var pageRowsDb = await pagerTask;
            var totalNum = await calculateCountTask;

             //过滤掉不必要的字段
            var pageRows = from i in pageRowsDb
                           select new
                           {
                               ID = i.ID,
                               LoginName=i.LoginName,
                               Entrance = i.Entrance,
                               LoginDate = CommonHelper.ConvertDateTimeInt(i.LoginDate).ToString(),
                               LoginIp = i.LoginIp,
                               UserId = i.UserId
                           };

            var jsonResult = new { Total = totalNum, CurrentPage = page, Data = pageRows };

            return Ok(new { result = jsonResult });
        }
    }
}
