using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IManageUserService : IBaseService<UT_ManageUsers>
    {
        /// <summary>
        /// 判断管理员用户是否拥有权限
        /// </summary>
        /// <param name="userId">管理员用户ID</param>
        /// <param name="rolesOrPermission">角色或者权限</param>
        /// <returns></returns>
        bool IsInRole(Guid manageUserId, string[] rolesOrPermission);
        /// <summary>
        /// 验证用户登录，并且返回用户数据
        /// </summary>
        /// <param name="LoginName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <returns></returns>
        UT_ManageUsers CheckUserLogin(string LoginName, string PassWord);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        bool ModifyPassWord(string UserId, string oldPwd, string newPwd);


    }
}
