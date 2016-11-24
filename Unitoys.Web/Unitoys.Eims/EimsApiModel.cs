using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Eims
{
    /// <summary>
    /// 短信Tid模型
    /// </summary>
    public class SMSTaskModel
    {
        public SMSTaskModel()
        {

        }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int tid { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string devname { get; set; }
        /// <summary>
        /// 发送端口
        /// 一个或多个（逗号，短横线连接）发送端口（从1开始）
        /// </summary>
        public string port { get; set; }
        /// <summary>
        /// sim卡的iccid
        /// 一个或多个（逗号分隔）发送sim卡的iccid
        /// </summary>
        public string iccid { get; set; }
        /// <summary>
        /// 短信接收者号码
        /// 一个或多个（逗号连接）短信接收者号码
        /// </summary>
        public string to { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string sms { get; set; }
        /// <summary>
        /// 编码集（utf8|base64）
        /// </summary>
        public string chs { get; set; }
        /// <summary>
        /// 指定短信发送时的编码
        /// 0：不指定
        /// 1：USC2
        /// 2: 7bit
        /// </summary>
        public int? coding { get; set; }
        /// <summary>
        /// 短消息服务中心号码
        /// </summary>
        public string smsc { get; set; }
        /// <summary>
        /// 指定任务内短信发送间隔时间范围（毫秒）
        /// </summary>
        public int? intvl { get; set; }
        /// <summary>
        /// 等待短信发送结果的超时时间（秒）
        /// 默认：30
        /// </summary>
        public int? tmo { get; set; }
        /// <summary>
        /// 是否开启短信成功状态报告, 1：开启, 0：关闭
        /// 默认：0
        /// </summary>
        public int? sdr { get; set; }
        /// <summary>
        /// 是否开启失败详情记录，1：开启，0：关闭。
        /// 默认：1
        /// </summary>
        public int? fdr { get; set; }
        /// <summary>
        /// 是否开启短信送达报告, 1：开启, 0：关闭
        /// 默认：0
        /// </summary>
        public int? dr { get; set; }
        /// <summary>
        /// 状态报告周期（秒），0：不开启，>0:开启
        /// 默认：60
        /// 注意：仅控制单个任务产生报告的周期
        /// </summary>
        public int? sr_prd { get; set; }
        /// <summary>
        /// 单次状态报告短信数, 小于1将使用缺省值
        /// 默认：10
        /// 注意：仅控制单个任务产生报告的已处理短信数
        /// </summary>
        public int? sr_cnt { get; set; }
    }

    /// <summary>
    /// 转化短信ModelBase
    /// </summary>
    public class TranslateEimsSMSModelBase
    {
        public int code { get; set; }
        public string reason { get; set; }
    }

    public class TranslateEimsATUSSDModel
    {
        public int code { get; set; }
        public string reason { get; set; }
        /// <summary>
        /// 类型
        /// 以下为类型对应时返回的参数
        /// at-status/ussd-status：status
        /// at-report/ussd-report：rpt_num,rpts
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 任务的状态码及原因描述
        /// 如果任务的响应码不是0，表明该任务没有被EIMS接受
        /// 0：OK
        /// 1：Invalid User
        /// 2：Invalid Port
        /// 3：USSD Expected
        /// 4：Pending USSD
        /// 5：SIM Unregistered
        /// 6：Timeout
        /// 7：Server Error
        /// 8：SMS expected
        /// 9：TO expected
        /// 10：Pending Transaction
        /// 11：TID Expected
        /// 12：FROM Expected
        /// 13：IccidNoFound
        /// </summary>
        public string status { get; set; }
        public int rpt_num { get; set; }
        /// <summary>
        /// json数据中报告本身也是一个数组
        /// [0]: 短信发送设备和端口（devname.1A, devname 2B,…），字符串
        /// [1]: 短信发送sim卡的iccid，字符串
        /// [2]: EIMS收到报告时的时间戳
        /// [3]: 报告内容：“ utf-8的BASE64编码”
        /// </summary>
        public List<List<string>> rpts { get; set; }
    }

    #region 1.SMS
    /// <summary>
    /// 发送SMS,响应转化
    /// </summary>
    public class TranslateEimsSMSSendModel : TranslateEimsSMSModelBase
    {
        /// <summary>
        /// 类型
        /// 以下为类型对应时返回的参数
        /// task-status：status
        /// status-report：rpt_num,rpts
        /// </summary>
        public string type { get; set; }
        public TaskStatu[] status { get; set; }
    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public class TaskStatu
    {
        public int tid { get; set; }
        /// <summary>
        /// 任务的状态码及原因描述
        /// 如果任务的响应码不是0，表明该任务没有被EIMS接受
        /// 0：OK
        /// 1：Invalid User
        /// 2：Invalid Port
        /// 3：USSD Expected
        /// 4：Pending USSD
        /// 5：SIM Unregistered
        /// 6：Timeout
        /// 7：Server Error
        /// 8：SMS expected
        /// 9：TO expected
        /// 10：Pending Transaction
        /// 11：TID Expected
        /// 12：FROM Expected
        /// 13：IccidNoFound
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// 查询SMS状态报告,响应转化
    /// </summary>
    public class TranslateEimsSMSReportQueryModel:TranslateEimsSMSModelBase
    {
        /// <summary>
        /// 类型
        /// 以下为类型对应时返回的参数
        /// task-status：status
        /// status-report：rpt_num,rpts
        /// </summary>
        public string type { get; set; }
        public int rpt_num { get; set; }
        public SMSReportModel rpts { get; set; }
    }
    /// <summary>
    /// 报告
    /// </summary>
    public class SMSReportModel
    {
        /// <summary>
        /// 关联的任务id
        /// </summary>
        public int tid { get; set; }
        /// <summary>
        /// 发送失败的短信数
        /// </summary>
        public int failed { get; set; }
        /// <summary>
        /// 未发送短信数
        /// </summary>
        public int unsent { get; set; }
        /// <summary>
        /// 成功详情记录（一个号码一个描述）
        /// 成功描述本身也是数组，依次为：
        /// [0]: 号码索引（基于群发号码） 整型
        /// [1]: 号码，字符串
        /// [2]: 短信发送设备和端口（devname.1A, devname 2B,…），字符串
        /// [3]: 短信发送sim卡的iccid，字符串
        /// [4]: 短信发送时UTC时间戳，整型
        /// </summary>
        public List<List<string>> sdr { get; set; }
        /// <summary>
        /// 失败详情记录（一个号码一个描述）
        /// 失败描述本身也是数组，依次为：
        /// [0]: 号码索引（基于群发号码） 整型
        /// [1]: 号码，字符串
        /// [2]: 短信发送设备和端口（devname.1A, devname 2B,…），字符串
        /// [3]: 短信发送sim卡的iccid，字符串
        /// [4]: 短信发送时UTC时间戳，整型
        /// [5]: 程序原因，code+描述 【参见1.0API】
        /// [6]: 运营商原因，code+描述。当程序原因为发送失败时有效
        /// </summary>
        public List<List<string>> fdr { get; set; }
    }

    /// <summary>
    /// 查询SMS,响应转化
    /// </summary>
    public class TranslateEimsSMSQueryModel : TranslateEimsSMSModelBase
    {
        /// <summary>
        /// 同步源标识
        /// EIMS每次运行产生一个新的ssrc
        /// 所以该值发生变化，重新查询
        /// </summary>
        public string ssrc { get; set; }
        /// <summary>
        /// 查询到的短信数目
        /// </summary>
        public int sms_num { get; set; }
        /// <summary>
        /// 查询到的短信内容
        /// 短信本身也是一个数组（为节约网络流量），依次为： 
        /// [0]: 标志是否为送达报告，0：普通短信，1：送达报告
        /// [1]: 短信发送设备和端口（devname.1A, devname 2B,…），字符串
        /// [2]: 短信发送sim卡的iccid，字符串
        /// [3]: EIMS收到短信/送达报告时的时间戳
        /// [4]: 发件人（如果是送达报告，则是消息服务中心号码）
        /// [5]: 收件人（如果是送达报告，则是原短信的收件人）
        /// [6]: 短信内容：
        /// 送达报告：“code scts”, code为0表示成功送达，utf-8
        /// 普通短信： utf-8的BASE64编码
        /// </summary>
        public List<List<string>> data { get; set; }
    }

    #endregion

}
