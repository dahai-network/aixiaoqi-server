using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class AddUsersConfigBindingModel
    {
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public UsersConfigStatus? Status { get; set; }
    }
}
