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
    public class FeedbackController : ApiController
    {
        private IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
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
        public async Task<IHttpActionResult> AddFeedback([FromBody]AddFeedbackBindingModel queryModel)
        {
            var currentUser = WebUtil.GetApiUserSession();

            if (string.IsNullOrEmpty(queryModel.Info))
            {
                //return Ok(new { status = 0, msg = "反馈信息不能为空！" });
                return Ok(new StatusCodeRes(StatusCodeType.反馈信息不能为空));
            }

            if (queryModel.Info.Length < 10)
            {
                return Ok(new StatusCodeRes(StatusCodeType.反馈信息不能少于10个字));
            }

            UT_Feedback feedback = new UT_Feedback()
            {
                UserId = currentUser.ID,
                CreateDate = DateTime.Now,
                Model = queryModel.Model,
                Version = queryModel.Version,
                Info = System.Web.HttpUtility.UrlDecode(queryModel.Info, Encoding.UTF8),
                Entrance = queryModel.Terminal
            };

            if (await _feedbackService.InsertAsync(feedback))
            {
                return Ok(new { status = 1, msg = "意见反馈信息添加成功！" });
            }
            return Ok(new StatusCodeRes(StatusCodeType.失败));
        }

    }
}
