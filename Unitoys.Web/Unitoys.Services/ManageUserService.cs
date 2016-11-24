using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class ManageUserService:BaseService<UT_ManageUsers>,IManageUserService
    {
        UnitoysEntities db = new UnitoysEntities();

        /// <summary>
        /// 判断管理员用户是否拥有权限
        /// </summary>
        /// <param name="userId">管理员用户ID</param>
        /// <param name="rolesOrPermission">角色或者权限</param>
        /// <returns></returns>
        public bool IsInRole(Guid manageUserId, string[] rolesOrPermission)
        {
            if (rolesOrPermission == null || rolesOrPermission.Length == 0)
            {
                throw new ArgumentNullException("rolesOrPermission");
            }

            int calculateCount = rolesOrPermission.Length;

            using (UnitoysEntities db = new UnitoysEntities())
            {
                //1. 获取ManageUser对象。
                UT_ManageUsers manageUser = db.UT_ManageUsers.Find(manageUserId);

                if (manageUser != null)
                {
                    //2. 获取User的角色列表。
                    List<UT_ManageUsersRole> userRoleList = db.UT_ManageUsersRole.Include(x => x.UT_Role).Where(x => x.ManageUserId == manageUser.ID).ToList();

                    if (userRoleList.Count > 0)
                    {
                        foreach (var userRole in userRoleList)
                        {
                            if (calculateCount == 0)
                            {
                                break;
                            }

                            for (int i = 0; i < rolesOrPermission.Length; i++)
                            {
                                //3. 判断是否拥有角色。
                                if (userRole.UT_Role.Name == rolesOrPermission[i])
                                {
                                    calculateCount--;
                                    break;
                                }
                            }

                            //4. 如果角色匹配不对，则判断角色里的权限进行匹配判断。
                            if (calculateCount > 0)
                            {
                                List<UT_RolePermission> rolePermissionList = db.UT_RolePermission.Include(x => x.UT_Permission).Where(x => x.RoleId == userRole.RoleId).ToList();

                                if (rolePermissionList.Count > 0)
                                {
                                    foreach (var rolePermission in rolePermissionList)
                                    {
                                        if (calculateCount == 0)
                                        {
                                            break;
                                        }

                                        //5. 判断是否拥有权限。
                                        for (int i = 0; i < rolesOrPermission.Length; i++)
                                        {
                                            if (rolePermission.UT_Permission.Name == rolesOrPermission[i])
                                            {
                                                calculateCount--;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return calculateCount == 0;
            }
        }

        public UT_ManageUsers CheckUserLogin(string LoginName, string PassWord)
        {
            PassWord = SecureHelper.MD5(PassWord);
            return db.UT_ManageUsers.Where(a => a.LoginName == LoginName && a.PassWord == PassWord).SingleOrDefault();
        }

        public bool ModifyPassWord(string UserId, string oldPwd, string newPwd)
        {
            UT_ManageUsers user = db.UT_ManageUsers.Find(UserId);
            if (user != null && user.PassWord == SecureHelper.MD5(oldPwd))
            {
                user.PassWord = SecureHelper.MD5(newPwd);

                return db.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }
        
    }
}
