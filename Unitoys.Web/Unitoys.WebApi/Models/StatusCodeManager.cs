using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi
{
    /*
     * 
     * 使用示例
     * 
     return Ok(new StatusCodeRes(StatusCodeType.用户不能为空));
     * 
     return Ok(new StatusCodeRes(StatusCodeType.失败,"保存失败 "));
     */
    /// <summary>
    /// 状态吗管理
    /// </summary>
    public struct StatusCodeRes
    {
        public StatusCodeType status;
        public string msg;
        public StatusCodeRes(StatusCodeType _status)
        {
            status = _status;
            msg = _status.ToString().Replace("_", "，") + "！";
        }
        public StatusCodeRes(StatusCodeType _status, string _msg)
        {
            status = _status;
            msg = _msg;
        }
    }

    public enum StatusCodeType
    {
        #region 权限验证
        tokenIncorrect = -999,
        parameterInvalid = -401,
        请求失败_请稍后重试 = -998,
        #endregion

        #region 公共状态码

        失败 = 0,
        成功 = 1,
        找不到该用户 = 9900,
        手机号码格式不正确 = 9901,
        密码长度必须在6到20位之间 = 9902,
        验证码无效 = 9903,
        验证码错误 = 9904,
        此验证码已经过期_请重新发送验证码 = 9905,
        系统繁忙_请重试 = 9906,

        参数错误 = 9940,
        必填参数为空 = 9944,
        用户不能为空 = 9945,

        内部错误 = 9950,
        #endregion

        #region 用户10

        //注册
        您输入的手机号码已注册 = 1001,
        注册失败_请重试 = 1002,

        //登录
        帐号不存在_请先注册 = 1003,
        您的帐号已被锁定 = 1004,
        密码不正确 = 1005,

        //退出登录
        退出失败 = 1006,

        //忘记密码
        手机号未注册 = 1008,

        //发送验证短信
        验证类型错误 = 1009,
        您输入的手机号码已注册2 = 1010,
        您输入的手机号码未注册 = 1011,
        短信服务器异常_请联系客服人员 = 1012,
        您发送的太频繁了 = 1013,
        阿里云短信调用失败 = 1014,
        一分钟内不能再次发送_RemainingSeconds秒以后可以再次发送 = 1015,//返回值在data中

        //获取本次可以通话的最长秒数
        //充值卡充值余额
        请输入16位完整卡密码 = 1016,
        充值失败 = 1018,
        充值卡已被使用或失效 = 1019,
        充值卡已超过最晚可用时间 = 1020,
        充值异常_请重试 = 1021,

        //用户反馈
        反馈信息不能为空 = 1022,
        反馈信息不能少于10个字 = 1023,

        //更新用户基本信息和体形
        昵称不能长于10个字符 = 1024,
        性别错误 = 1025,

        //更新用户头像
        只能选择一张图片 = 1026,
        暂时无法保存头像 = 1028,

        //获取用户消费记录
        //判断TOKEN是否过期
        //上传配置
        //获取用户配置
        //获取指定用户手机号是否在线


        //礼包卡
        请输入16位礼包卡密码 = 1029,
        礼包卡已被使用或失效 = 1030,
        礼包卡已超过最晚可用时间 = 1031,
        套餐不存在 = 1032,
        已绑定礼包卡 = 1033,
        绑定异常_请重试 = 1034,
        #endregion

        #region 订单11

        //创建订单
        数量请在BeginQty至EndQty之间选择 = 1101,
        套餐不可用_请选择其他套餐 = 1103,
        该套餐不允许购买多个 = 1115,

        //套餐激活,激活大王卡套餐
        激活失败_超过最晚激活日期 = 1104,
        激活失败_激活类型异常 = 1105,
        激活套餐失败_可能套餐已过期 = 1106,
        //订单套餐激活本地完成
        //查询用户订单
        //根据ID查询用户订单

        //取消订单
        订单已经被取消 = 1108,
        订单已被使用 = 1109,
        订单不允许取消 = 1110,

        //通过用户余额支付套餐订单
        此订单已经支付成功_不能再支付 = 1111,
        用户余额不足 = 1112,
        支付方式异常 = 1113,
        支付失败 = 1114,

        //激活大王卡套餐

        #endregion

        #region 短信12

        //发送短信,发送重试错误的短信
        手环内的卡未注册成功 = 1201,
        //发送重试错误的短信
        短信已发送成功 = 1202,
        短信处理中 = 1203,

        #endregion

        #region 设备13

        //绑定设备
        设备重复绑定 = 1301,
        此设备已绑定其他用户 = 1302,
        用户已绑定设备 = 1303,

        //版本号更新
        //空中升级
        用户未绑定设备 = 1304,

        //解除绑定
        //查询
        //是否被其他用户绑定
        //空中升级

        #endregion

        #region 闹钟14
        达到最高设定数量 = 1401
        #endregion
    }
}
