using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_ZCSelectionNumber : UT_Entity
    {
        /// <summary>
        /// 众筹订单选号Id
        /// </summary>
        public Guid? OrderByZCSelectionNumberId { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }
        /// <summary>
        /// 乐观并发
        /// </summary>
        [Timestamp]
        public Byte[] RowVersion { get; set; }
       

        public virtual UT_OrderByZCSelectionNumber UT_OrderByZCSelectionNumber { get; set; }
    }
}
