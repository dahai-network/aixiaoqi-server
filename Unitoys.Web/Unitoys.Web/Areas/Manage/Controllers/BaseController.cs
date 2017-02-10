using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unitoys.Core;
using Unitoys.Model;

namespace Unitoys.Web.Areas.Manage.Controllers
{
    /// <summary>
    /// 所有需要进行登录、权限控制的控制器基类
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //错误记录
            LoggerHelper.Error("错误日志",filterContext.Exception);

            // 当自定义显示错误 mode = On，显示友好错误页面
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                this.View("Error").ExecuteResult(this.ControllerContext);
            }
        }

    }
}
