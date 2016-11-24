using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi
{
    /// <summary>
    /// 用户注册
    /// </summary>
    public class Register : ApiController
    {
        IUserService userService;

        public Register(IUserService userService)
        {
            this.userService = userService;
        }
        public HttpResponseMessage Post([FromUri]AuthQuery authQueryint, string LoginName, string PassWord)
        {
            if (String.IsNullOrEmpty(LoginName) || String.IsNullOrEmpty(PassWord) || LoginName.Length < 5 || PassWord.Length < 5)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Type = "Error", Msg = "帐号或者密码不符合规范！" });
            }
            UT_Users model = new UT_Users();
            model.LoginName = LoginName;
            model.PassWord = SecureHelper.MD5(PassWord);
            model.Amount = 0;
            model.CreateDate = DateTime.Now;
            model.GroupId = Guid.Parse("688a3245-2628-4488-bf35-9c029ff80988"); //默认会员组
            model.Lock4 = 0;
            model.Score = 0;
            if (userService.Insert(model))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Type = "Success", Msg = "添加成功" });
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Type = "Error", Msg = "添加失败！" });
        }
         
    }
}
