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
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "反馈信息不能为空"));
            }
            else if (string.IsNullOrEmpty(queryModel.Terminal))
            {
                return Ok(new { status = 0, msg = "Terminal不能为空！" });
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "Terminal不能为空"));
            }
            else if (string.IsNullOrEmpty(queryModel.Mail))
            {
                return Ok(new { status = 0, msg = "邮箱不能为空！" });
                return Ok(new StatusCodeRes(StatusCodeType.必填参数为空, "邮箱不能为空"));
            }
            else if (!ValidateHelper.IsEmail(queryModel.Mail))
            {
                return Ok(new { status = 0, msg = "邮箱格式不正确！" });
                return Ok(new StatusCodeRes(StatusCodeType.参数错误, "邮箱格式不正确"));
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
            return Ok(new StatusCodeRes(StatusCodeType.失败, "意见反馈信息添加失败"));
        }


        /// <summary>
        /// 空中升级
        /// </summary>
        /// <param name="TerminalCode">终端</param>
        /// <returns></returns>
        [HttpGet]
        [NoLogin]
        public async Task<IHttpActionResult> Upgrade([FromUri]PublicUpgradeBindingModel model)
        {
            if (Convert.ToDouble(GetReplaceNotFirst(".", model.Version)) < Convert.ToDouble(GetReplaceNotFirst(".", UTConfig.SiteConfig.IosVersion)))
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        Version = UTConfig.SiteConfig.IosVersion,// "版本号",
                        Mandatory = UTConfig.SiteConfig.IosUpgradeMandatory,// "是否强制",
                        Descr = UTConfig.SiteConfig.IosUpgradeDescr.Replace("\\n", "\n"),// "1、更新蓝牙绑定流程\n2、更新蓝牙连接稳定性\n3、优化短信功能\n4、新增爱小器国际卡手机激活功能",//"升级内容",
                        Url = "",//"升级地址url（地址可为空）",
                        TerminalCode = model.TerminalCode + ""//终端标识
                    }
                });
            }
            else
            {
                return Ok(new
                {
                    status = 1,
                    msg = "已是最新版本",
                    data = new
                    {

                    }
                });

            }
        }

        /// <summary>
        /// 替换字符串，除第一个匹配项
        /// </summary>
        /// <param name="replaceStr">需要替换的内容</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private string GetReplaceNotFirst(string replaceStr, string value)
        {
            return value.Substring(0, value.IndexOf(replaceStr) + 1) + (value.Substring(value.IndexOf(replaceStr) + 1).Replace(replaceStr, ""));
        }
    }
}
