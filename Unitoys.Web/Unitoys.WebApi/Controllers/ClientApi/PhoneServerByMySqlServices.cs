using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;

namespace Unitoys.WebApi.Controllers
{
    public class PhoneServerByMySqlServices
    {
        /// <summary>
        /// 设置Sip Buddies
        /// </summary>
        /// <param name="callTransferNum">大号</param>
        /// <param name="userTel">用户电话</param>
        /// <param name="isOpen">是否为打开功能</param>
        /// <returns>0失败1成功2系统异常</returns>
        public static int SetSip_Buddies(string userTel)
        {
            try
            {
                var secret = SecureHelper.MD5(SecureHelper.MD5(SecureHelper.MD5(UTConfig.SiteConfig.PublicKey + userTel) + "voipcc2015"));

                var result = MySqlDBHelper.ExecuteNonQuery(string.Format(@"Replace INTO sip_buddies (NAME, defaultuser, secret, context,canreinvite, HOST, nat, qualify, TYPE)  VALUES ('{0}', '{0}', '{1}', 'unitoysapp','no', 'dynamic', 'yes', 'yes', 'friend')", userTel, secret));

                if (result <= 0)
                {
                    LoggerHelper.Error(string.Format("SetSip_Buddies失败，参数，userTel:{0}", userTel));
                }
                //result > 1//代表删除后新增
                return result > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(string.Format("SetSip_Buddies失败，参数，userTel:{0}", userTel), ex);
                return 2;
            }
        }

        /// <summary>
        /// 设置重写电话的规则
        /// </summary>
        /// <param name="callTransferNum">大号</param>
        /// <param name="userTel">用户电话</param>
        /// <param name="isOpen">是否为打开功能</param>
        /// <returns>0失败1成功2系统异常</returns>
        internal static int SetNameChange(string callTransferNum, string userTel, bool isOpen)
        {
            try
            {
                //数据行
                var countTel = MySqlDBHelper.ExecuteScalar("select count(id) from numchange where newnum='" + userTel + "'");
                if (countTel == null)
                {
                    LoggerHelper.Error("countTel is SystemError", new Exception("numchange>countTel上出现系统错误"));
                    return 0;
                }
                int intCountTel = Convert.ToInt32(countTel);

                //打开功能,更新信息
                int result = 0;
                if (isOpen)
                {
                    if (intCountTel > 0)
                    {
                        result = MySqlDBHelper.ExecuteNonQuery(string.Format(@"
                            update numchange 
                                    set oldnum='{0}'
                            where newnum='{1}'", callTransferNum, userTel));
                    }
                    else
                    {
                        result = MySqlDBHelper.ExecuteNonQuery(string.Format(@"
                            insert into numchange(oldnum,newnum) values('{0}','{1}')", callTransferNum, userTel));
                    }
                }
                else
                {
                    if (intCountTel > 0)
                    {
                        //低程度避免数据过期问题，直接以SQL处理
                        //rewriterulesoutcallee字段删除当前info
                        //考虑只有单条数据的情况
                        result = MySqlDBHelper.ExecuteNonQuery(string.Format(@"delete from numchange where oldnum='{0}' and newnum='{1}'", callTransferNum, userTel));
                    }
                    else
                    {
                        return 1;
                    }
                }
                if (result <= 0)
                {
                    LoggerHelper.Error(string.Format("SetNameChange失败，参数，callTransferNum:{0},userTel:{1},isOpen:{2}", callTransferNum, userTel, isOpen));
                }
                return result > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(string.Format("SetNameChange失败，参数，callTransferNum:{0},userTel:{1},isOpen:{2}", callTransferNum, userTel, isOpen), ex);
                return 2;
            }
        }

        /// <summary>
        /// 获取用户是否在线
        /// </summary>
        /// <param name="callTransferNum">大号</param>
        /// <param name="userTel">用户电话</param>
        /// <param name="isOpen">是否为打开功能</param>
        /// <returns>0失败1成功2系统异常</returns>
        internal static int GetIsOnline(string userTel)
        {
            try
            {
                var result = MySqlDBHelper.ExecuteScalar(string.Format(@"select ipaddr from sip_buddies where defaultuser='{0}'", userTel));

                return result == null || string.IsNullOrEmpty(result.ToString()) ? 0 : 1;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(string.Format("GetIsOnline失败，参数，userTel:{0}", userTel), ex);
                return 2;
            }
        }

    }
}
