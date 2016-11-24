using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UnitoysAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IManageUserService _manageUserService;

        private string[] rolesOrPermissionsName = null;

        private Dictionary<string, string[]> cacheDic = new Dictionary<string, string[]>();

        private int statusCode;

        private LoginUserInfo currentUserInfo;

        public UnitoysAuthorizeAttribute() { }

        public UnitoysAuthorizeAttribute(IManageUserService manageUserService)
        {
            this._manageUserService = manageUserService;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            currentUserInfo = currentUserInfo ?? WebUtil.GetManageUserSession();

            //1. 首先判断用户是否登录。
            if (CurrentUserInfo != null)
            {
                var controllerDescriptor = filterContext.ActionDescriptor.ControllerDescriptor;
                var actionDescriptor = filterContext.ActionDescriptor;

                //2. 记录访问的ControllerName和ActionName作为CacheKey。
                var cacheKey = controllerDescriptor.ControllerName + "." + actionDescriptor.ActionName;

                //持久化对象，每次要初始化。
                rolesOrPermissionsName = null;

                //3. 获取访问需要的权限，先判断CacheDictionary里是否存在，不存在则获取并添加到CacheDictionary里。
                if (!cacheDic.ContainsKey(cacheKey))
                {
                    //4. 首先判断Action上是否有定义RequireRolesOrPermissionsAttribute。
                    var attrs = actionDescriptor.GetCustomAttributes(typeof(RequireRolesOrPermissionsAttribute), false);
                    if (attrs.Length == 1)
                    {
                        rolesOrPermissionsName = ((RequireRolesOrPermissionsAttribute)attrs[0]).RolesOrPermissionsName;
                    }
                    else
                    {
                        //5. 如果Action找不到则判断Controller上是否有定义RequireRolesOrPermissionsAttribute。
                        attrs = controllerDescriptor.GetCustomAttributes(typeof(RequireRolesOrPermissionsAttribute), false);
                        if (attrs.Length == 1)
                        {
                            rolesOrPermissionsName = ((RequireRolesOrPermissionsAttribute)attrs[0]).RolesOrPermissionsName;
                        }
                    }
                    if (rolesOrPermissionsName != null)
                    {
                        cacheDic[cacheKey] = rolesOrPermissionsName;
                    }
                }
                else
                {
                    rolesOrPermissionsName = cacheDic[cacheKey];
                }
            }
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }

            //1. 判断是否已经登录，若没有登录则跳转到登录界面。
            if(CurrentUserInfo == null)
            {
                return false;
            }

            if (rolesOrPermissionsName != null)
            {
                //2. 判断当前用户是否拥有访问权限。
                bool isUserInRoleOrHasPermission = _manageUserService.IsInRole(CurrentUserInfo.ID, rolesOrPermissionsName);
                
                //3. 如果没有权限则返回403错误。
                if (!isUserInRoleOrHasPermission)
                {
                    statusCode = 403;
                    return false;
                }
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            bool isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();

            switch (statusCode)
            {
                case 403:
                    if(isAjaxRequest)
                    {
                        filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                    }
                    else
                    {
                        ViewResult result = new ViewResult() { ViewName = "AccessDenied" };
                        filterContext.Result = result;
                    }                    
                    break;
                default:                    
                    filterContext.Result = new RedirectResult("/Manage/Login");
                    break;
            }
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        private LoginUserInfo CurrentUserInfo
        {
            get
            {
                return this.currentUserInfo;
            }
            set
            {
                this.currentUserInfo = value;
            }
        }
    }
}
