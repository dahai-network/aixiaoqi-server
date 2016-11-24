using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_Role : UT_Entity
    {
        public UT_Role()
        {
            this.UT_RolePermission = new HashSet<UT_RolePermission>();
            this.UT_ManageUsersRole = new HashSet<UT_ManageUsersRole>();
        }

        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UT_RolePermission> UT_RolePermission { get; set; }
        public virtual ICollection<UT_ManageUsersRole> UT_ManageUsersRole { get; set; }
    }
}
