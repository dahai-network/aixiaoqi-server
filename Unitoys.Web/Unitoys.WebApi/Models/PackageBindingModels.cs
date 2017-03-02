using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.WebApi.Models
{
    public class GetPackageBindingModels
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        /// <summary>
        /// 套餐类型
        /// </summary>
        public CategoryType? Category { get; set; }

        /// <summary>
        /// 是否流量套餐
        /// </summary>
        public bool? IsCategoryFlow { get; set; }
        /// <summary>
        /// 是否通话套餐
        /// </summary>
        public bool? IsCategoryCall { get; set; }
        /// <summary>
        /// 是否双卡双待套餐
        /// </summary>
        public bool? IsCategoryDualSimStandby { get; set; }
        /// <summary>
        /// 是否大王卡套餐
        /// </summary>
        public bool? IsCategoryKingCard { get; set; }
    }

    

}
