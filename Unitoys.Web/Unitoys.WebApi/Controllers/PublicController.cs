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
    public class PublicController : ApiController
    {
        private IFeedbackService _feedbackService;
        public PublicController(IFeedbackService feedbackService)
        {
            this._feedbackService = feedbackService;
        }

        /// <summary>
        /// 添加意见反馈信息
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> AddFeedback([FromBody]AddFeedbackBindingModel queryModel)
        {
            var currentUser = WebUtil.GetApiUserSession();
            if (string.IsNullOrEmpty(queryModel.Info))
            {
                return Ok(new { status = 0, msg = "反馈信息不能为空！" });
            }
            else if (string.IsNullOrEmpty(queryModel.Terminal))
            {
                return Ok(new { status = 0, msg = "Terminal不能为空！" });
            }
            else if (string.IsNullOrEmpty(queryModel.Mail))
            {
                return Ok(new { status = 0, msg = "邮箱不能为空！" });
            }
            else if (!ValidateHelper.IsEmail(queryModel.Mail))
            {
                return Ok(new { status = 0, msg = "邮箱格式不正确！" });
            }
            UT_Feedback feedback = new UT_Feedback()
            {
                //UserId = currentUser.ID,
                CreateDate = DateTime.Now,
                Model = queryModel.Model,
                Version = queryModel.Version,
                Info = queryModel.Info,
                Entrance = queryModel.Terminal,
                Mail = queryModel.Mail,
            };

            if (await _feedbackService.InsertAsync(feedback))
            {
                return Ok(new { status = 1, msg = "意见反馈信息添加成功！" });
            }
            return Ok(new { status = 0, msg = "意见反馈信息添加失败！" });
        }


        /// <summary>
        /// 空中升级
        /// </summary>
        /// <param name="TerminalCode">终端</param>
        /// <returns></returns>
        [HttpGet]
        [NoLogin]
        public async Task<IHttpActionResult> Upgrade(int TerminalCode)
        {
            return Ok(new
            {
                status = 1,
                data = new
                {
                    Version = UTConfig.SiteConfig.IosVersion,// "版本号",
                    Mandatory = "0",// "是否强制",
                    Descr = "1.更新时间更新/r/n2.优化传输速度",//"升级内容",
                    Url = "",//"升级地址url（地址可为空）",
                    TerminalCode = TerminalCode + ""//终端标识
                }
            });
        }
    }
}
