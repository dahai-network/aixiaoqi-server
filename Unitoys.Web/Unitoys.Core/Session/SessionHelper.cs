using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.Core
{
    public class SessionHelper
    {
        private const int secondsTimeOut = 60 * 600;  //默认过期时间20分钟  单位秒

        
        public LoginUserInfo this[string key]
        {
            get
            {
                string webCookie = WebHelper.GetCookie(key);
                if (webCookie == "")
                {
                    return null;
                }
                key = key + "_" + SecureHelper.AESDecrypt(webCookie);

                try
                {
                    //距离过期时间还有多少秒
                    long l = RedisHelper.Instance.TTL(key);
                    if (l >= 0)
                    {
                        RedisHelper.Instance.Expire(key, secondsTimeOut);
                    }
                    return RedisHelper.Instance.Get<LoginUserInfo>(key);
                }
                catch(Exception e)
                {
                    LoggerHelper.Error("Redis数据库访问出错！"+e.Message,e);
                    return null;
                }
                
            }
            set
            {
                SetSession(key, value);
            }
        }
        public void SetSession(string key, LoginUserInfo value)
        {

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Key is Null or Epmty");
            }
            WebHelper.SetCookie(key, SecureHelper.AESEncrypt(value.ID.ToString()));
            key = key + "_" + value.ID;

            try
            {

                RedisHelper.Instance.Set<LoginUserInfo>(key, value, secondsTimeOut);
            }
            catch
            {
                LoggerHelper.Error("Redis数据库访问出错！");
            }
            
        }
        /// <summary>
        /// 查看改帐号是否有在其他地方登录
        /// </summary>
        /// <returns></returns>
        public bool isRepeatLogin(string key, Guid ID)
        {
            key = key + "_" + ID.ToString();

            LoginUserInfo user = RedisHelper.Instance.Get<LoginUserInfo>(key);
            if (user != null && !user.LoginIp.Equals(WebHelper.GetIP()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            var rs = RedisHelper.Instance.Remove(key + "_" + SecureHelper.AESDecrypt(WebHelper.GetCookie(key)));
            WebHelper.DeleteCookie(key);
            return rs;
        }
             

    }
}
