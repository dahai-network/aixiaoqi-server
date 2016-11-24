using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_Permission : UT_Entity
    {
        public UT_Permission()
        {
            this.UT_RolePermission = new HashSet<UT_RolePermission>();
        }

        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public ICollection<UT_RolePermission> UT_RolePermission { get; set; }
    }
}
