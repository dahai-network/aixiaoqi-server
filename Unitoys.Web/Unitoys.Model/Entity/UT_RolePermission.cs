using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_RolePermission : UT_Entity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual UT_Role UT_Role { get; set; }
        public virtual UT_Permission UT_Permission { get; set; }
    }
}
