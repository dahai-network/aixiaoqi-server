using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi
{
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
            msg = _status.ToString();
        }
    }

    public enum StatusCodeType
    {
        #region 公共状态码
        失败 = 0,
        找不到该用户 = 9900,
        手机号码格式不正确 = 9901,
        密码长度必须在6到20位之间 = 9902,
        验证码无效 = 9903,
        验证码错误 = 9904,
        此验证码已经过期_请重新发送验证码 = 9905,
        系统繁忙_请重试 = 9906,


        #endregion

        #region 用户

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
        一分钟内不能再次发送_x秒以后可以再次发送 = 1015,//返回值在data中
        #endregion
    }
}
