using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class UT_UsersGroup : UT_Entity
    {
        public UT_UsersGroup()
        {
            this.UT_Users = new HashSet<UT_Users>();
        }

        public string GroupName { get; set; }
        public int Level { get; set; }

        public virtual ICollection<UT_Users> UT_Users { get; set; }
    }
}
