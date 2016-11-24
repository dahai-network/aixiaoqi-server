using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UserWxConfiguration : EntityTypeConfiguration<UT_UsersWx>
    {
        public UserWxConfiguration()
        {
            //微信绑定用户表1对1
            this.HasRequired(t => t.UT_User).WithMany().HasForeignKey(t=>t.UserId);
        }
    }
}
