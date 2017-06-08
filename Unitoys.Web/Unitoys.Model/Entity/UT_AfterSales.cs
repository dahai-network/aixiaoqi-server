using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 联系我们
    /// </summary>
    public class UT_AfterSales : UT_Entity
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string AfterSalesNum { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 问题描述
        /// </summary>
        public string ProblemDescr { get; set; }
        public string Pic1 { get; set; }
        public string Pic2 { get; set; }
        public string Pic3 { get; set; }
        public string Pic4 { get; set; }
        public string Pic5 { get; set; }
        public AfterSalesStatus Status { get; set; }
        /// <summary>
        /// 回寄快递单号
        /// </summary>
        public string TrackingNO { get; set; }
        /// <summary>
        /// 产品型号
        /// </summary>
        public DeviceType ProductModel { get; set; }
        /// <summary>
        /// 快递公司
        /// </summary>
        public string ExpressCompany { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditRemark { get; set; }
        public int CreateDate { get; set; }
        /// <summary>
        /// 显示排序
        /// </summary>
        //public int DisplayOrder { get; set; }
    }
    public enum AfterSalesStatus
    {
        //提交申请
        [Description("待审核")]
        Pending = 0,
        //审核
        [Description("审核通过")]
        Pass = 1,
        //审核不通过
        [Description("审核不通过")]
        NotPass = 2,
        //已取消
        [Description("已取消")]
        Cancel = 3,
        //收到快递
        [Description("收到快递")]
        ReceivedCourier = 4,
        //邮寄
        [Description("回寄中")]
        ReturnTo = 5,
        //完成
        [Description("完成")]
        Done = 6,
        //邮寄 = 2,
        //处理 = 3,
        //完成 = 4,
    }
}
