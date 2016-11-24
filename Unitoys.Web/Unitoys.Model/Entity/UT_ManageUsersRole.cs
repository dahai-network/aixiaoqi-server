using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_ManageUsersRole : UT_Entity
    {
        public Guid ManageUserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual UT_ManageUsers UT_ManageUsers  { get; set; }
        public virtual UT_Role UT_Role { get; set; }
    }
}
