//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Unitoys.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UT_UserBill
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string Amount { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UserOldAmount { get; set; }
        public string UserNewAmount { get; set; }
        public Nullable<int> BillType { get; set; }
        public Nullable<int> PayType { get; set; }
    }
}
