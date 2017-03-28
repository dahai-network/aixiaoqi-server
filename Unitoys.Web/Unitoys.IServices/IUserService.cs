using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IUserService : IBaseService<UT_Users>
    {
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="user">用户Entity</param>
        /// <param name="smsConfirmation">手机验证Entity</param>
        /// <returns></returns>
        Task<bool> ForgotPasswordAsync(UT_Users user, UT_SMSConfirmation smsConfirmation);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user">用户Entity</param>
        /// <param name="smsConfirmation">手机验证Entity</param>
        /// <returns></returns>
        Task<bool> RegisterAsync(UT_Users user, UT_SMSConfirmation smsConfirmation);
        /// <summary>
        /// 验证手机号码登录，并且返回用户数据
        /// </summary>
        /// <param name="LoginName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <returns></returns>        
        Task<UT_Users> CheckUserLoginTelAsync(string tel, string passWord);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">页数</param>
        /// <param name="userName">用户名</param>
        /// <param name="tel">手机号</param>
        /// <param name="createStartDate">创建开始时间</param>
        /// <param name="createEndDate">创建结束时间</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_Users>>> SearchAsync(int page, int rows, string tel, DateTime? createStartDate, DateTime? createEndDate, int? status);
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        bool ModifyPassWord(Guid ID, string oldPwd, string newPwd);
        /// <summary>
        /// 更新用户基本信息和体形(传什么更新什么)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="sex"></param>
        /// <param name="birthday"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="movingTarget"></param>
        /// <returns></returns>
        Task<bool> ModifyUserInfoAndUserShape(Guid userId, string nickName, int? sex, int? birthday, double? height, double? weight, int? movingTarget);
        /// <summary>
        /// 检查手机号码是否已经存在
        /// </summary>
        /// <param name="tel">手机号码</param>
        /// <returns>true：存在，false：不存在</returns>
        bool CheckTelExist(string tel);
        /// <summary>
        /// 通过微信openid获取用户信息
        /// </summary>
        /// <param name="openId">微信用户唯一的openid</param>
        /// <returns></returns>
        Task<UT_Users> GetEntityByOpenIdAsync(string openId);
        /// <summary>
        /// 获取余额和订单总可通话最长秒数
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1/失败->0通话秒数</returns>
        Task<int> GetAmountAndOrderMaximumPhoneCallTime(Guid id);
        /// <summary>
        /// 直接充值
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        Task<bool> Recharge(Guid userId, decimal price);
    }
}
