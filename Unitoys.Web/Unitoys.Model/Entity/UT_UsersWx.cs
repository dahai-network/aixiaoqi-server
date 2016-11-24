using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 微信绑定用户
    /// </summary>
    public class UT_UsersWx : UT_Entity
    {
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
        public int Sex { get; set; }
        public Guid UserId { get; set; }
        public virtual UT_Users UT_User { get; set; }
    }
}
