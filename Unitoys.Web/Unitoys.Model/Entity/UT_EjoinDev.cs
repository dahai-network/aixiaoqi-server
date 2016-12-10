using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    /// <summary>
    /// 一正设备账号信息管理
    /// </summary>
    public class UT_EjoinDev : UT_Entity
    {
        public UT_EjoinDev()
        {
            this.UT_EjoinDevSlot = new HashSet<UT_EjoinDevSlot>();
         }
        /// <summary>
        /// 设备名，注册名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最大端口数
        /// </summary>
        public int MaxPort { get; set; }
        /// <summary>
        /// 注册状态
        /// </summary>
        public RegStatusType RegStatus { get; set; }
        /// <summary>
        /// 设备注册IP
        /// </summary>
        public string RegIp { get; set; }
        /// <summary>
        /// 设备软件版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 设备MAC地址
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public ModType ModType { get; set; }

        public virtual ICollection<UT_EjoinDevSlot> UT_EjoinDevSlot { get; set; }
    }
    public enum ModType
    {
        [Description("移动联通设备")]
        GSM = 0,
        [Description("电信设备")]
        WCDMA = 1
    }
    public enum RegStatusType
    {
        [Description("注册成功")]
        SUCCESS = 0,
        [Description("注册失败")]
        ERROR = 1
    }
}
