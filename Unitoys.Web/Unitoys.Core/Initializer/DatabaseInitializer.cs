using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core.Security;
using Unitoys.Model;

namespace Unitoys.Core.Initializer
{
    public class DatabaseInitializer
    {
        public static void Initialize(bool isReGenerateSystemPermission = true)
        {                     
            //预热程序，
            using (var dbContext = new UnitoysEntities())
            {
                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<EdmSchemaError>());

                if (isReGenerateSystemPermission)
                {
                    InitializePermission();
                }
                
            }
        }

        /// <summary>
        /// 初始化角色&权限
        /// </summary>
        public static void InitializePermission()
        {
            using(var dbContext = new UnitoysEntities())
            {
                using(var dbTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //生成系统默认角色&权限。

                        //1. 首先删除所有角色、权限及用户跟角色、角色跟权限之间的关系。
                        List<UT_ManageUsersRole> manageUsersRoleList = dbContext.UT_ManageUsersRole.ToList();
                        List<UT_RolePermission> rolePermissionList = dbContext.UT_RolePermission.ToList();
                        List<UT_Role> roleList = dbContext.UT_Role.ToList();
                        List<UT_Permission> permissionList = dbContext.UT_Permission.ToList();

                        manageUsersRoleList.ForEach(x =>
                        {
                            dbContext.UT_ManageUsersRole.Attach(x);
                            dbContext.Entry<UT_ManageUsersRole>(x).State = EntityState.Deleted;
                        });

                        rolePermissionList.ForEach(x =>
                        {
                            dbContext.UT_RolePermission.Attach(x);
                            dbContext.Entry<UT_RolePermission>(x).State = EntityState.Deleted;
                        });

                        roleList.ForEach(x =>
                        {
                            dbContext.UT_Role.Attach(x);
                            dbContext.Entry<UT_Role>(x).State = EntityState.Deleted;
                        });

                        permissionList.ForEach(x =>
                        {
                            dbContext.UT_Permission.Attach(x);
                            dbContext.Entry<UT_Permission>(x).State = EntityState.Deleted;
                        });

                        dbContext.SaveChanges();

                        //2. 生成系统默认角色&权限。

                        //2.1 添加默认角色管理员。
                        UT_Role adminRole = new UT_Role()
                        {
                            CreateDate = DateTime.Now,
                            Name = "System_Admin",
                            Description = "管理员"
                        };
                        dbContext.UT_Role.Add(adminRole);

                        //2.2 添加默认权限。
                        foreach (Tuple<string, string, int> prop in UnitoysPermissionStore.Properties)
                        {
                            UT_Permission permission = new UT_Permission()
                            {
                                CreateDate = DateTime.Now,
                                Name = prop.Item1,
                                Description = prop.Item2,
                                DisplayOrder = prop.Item3
                            };
                            dbContext.UT_Permission.Add(permission);
                        }

                        dbContext.SaveChanges();

                        //3. 生成管理员拥有所有权限。                        
                        List<UT_Permission> latestPermissionList = dbContext.UT_Permission.ToList();

                        foreach (UT_Permission permission in latestPermissionList)
                        {
                            UT_RolePermission rolePermission = new UT_RolePermission()
                            {
                                CreateDate = DateTime.Now,
                                RoleId = adminRole.ID,
                                PermissionId = permission.ID
                            };
                            dbContext.UT_RolePermission.Add(rolePermission);
                        }

                        dbContext.SaveChanges();

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();                        
                        throw;
                    }
                    
                }                
            }
        }
    }
}
