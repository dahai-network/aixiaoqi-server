using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unitoys.Model
{
    public class AuthQuery
    {
        /// <summary>
        /// 合作名称，通过这个获取私钥
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// 时间戳，用于加密协议，防止重复提交和攻击
        /// </summary>
        public string Expires { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 用户登录信息
        /// </summary>
        public string Token { get; set; }
    }
}