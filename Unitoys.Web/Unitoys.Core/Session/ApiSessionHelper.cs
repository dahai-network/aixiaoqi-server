using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.Core
{
    public class ApiSessionHelper
    {
        private const int secondsTimeOut = 60 * 60 * 24 * 12;  //默认过期时间20分钟  单位秒-12天过期

        public LoginUserInfo this[string key]
        {
            get
            {
                return GetSession(key);
            }
            set
            {
                SetSession(key, value);
            }
        }
        private LoginUserInfo GetSession(string key)
        {
            if (key == null)
            {
                return null;
            }
            try
            {

                var loginUserInfo = RedisHelper.Instance.Get<LoginUserInfo>(key);

                if (loginUserInfo == null) return null;

                //距离过期时间还有多少秒 950400
                long l = RedisHelper.Instance.TTL(key);
                if (l < secondsTimeOut * 0.8)
                {
                    RedisHelper.Instance.Expire(key, secondsTimeOut);
                    RedisHelper.Instance.Expire(loginUserInfo.Tel, secondsTimeOut);
                }
                if (RedisHelper.Instance.Get<string>(loginUserInfo.Tel) == key)
                {
                    return loginUserInfo;
                }
                this.Remove(key);
                return null;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("获取token信息失败:" + ex.Message);
                return null;
            }
        }

        private void SetSession(string key, LoginUserInfo value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Key is Null or Epmty");
            }
            try
            {
                //查询这个账号是否有登录过，如果有登陆把之前的token移除
                var oldKey = RedisHelper.Instance.Get<string>(value.Tel);
                if (oldKey != key && oldKey != null)
                {
                    this.Remove(oldKey);
                }
                RedisHelper.Instance.Set<LoginUserInfo>(key, value, secondsTimeOut);
                RedisHelper.Instance.Set<string>(value.Tel, key, secondsTimeOut);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("设置token信息失败:" + ex.Message);
            }
        }
        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            var loginUserInfo = RedisHelper.Instance.Get<LoginUserInfo>(key);
            if (loginUserInfo != null)
            {
                RedisHelper.Instance.Remove(loginUserInfo.Tel);
            }
            return RedisHelper.Instance.Remove(key);
        }
        /// <summary>
        /// 判断Session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long Exists(string key)
        {
            return RedisHelper.Instance.Exists(key);
        }

        public string GetStrSession(string key)
        {
            return RedisHelper.Instance.Get<string>(key);
        }
    }
}
