using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;

namespace Unitoys.WebApi.Controllers
{
    public class LoginController : ApiController
    {
        private IUserService _userService;
        private IUserLoginRecordService _userLoginInfoService;
        private IUserShapeService _userShapeService;
        private IDeviceBraceletService _deviceBraceletService;
        private IUsersConfigService _usersConfigService;
        public LoginController(IUserService userService, IUserLoginRecordService userLoginInfoService, IUserShapeService userShapeService, IDeviceBraceletService deviceBraceletService, IUsersConfigService usersConfigService)
        {
            this._userService = userService;
            this._userLoginInfoService = userLoginInfoService;
            this._userShapeService = userShapeService;
            this._deviceBraceletService = deviceBraceletService;
            this._usersConfigService = usersConfigService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="authQueryint">授权参数</param>
        /// <param name="Tel">手机号码</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<IHttpActionResult> CheckLogin([FromBody]QueryLoginUser queryModel)
        {

            var errorMsg = "";

            if (!ValidateHelper.IsMobile(queryModel.Tel))
            {
                errorMsg = "手机号码格式不正确！";
            }
            else if (queryModel.PassWord.Length < 6 || queryModel.PassWord.Length > 12)
            {
                errorMsg = "密码长度必须在6~12位之间！";
            }
            else if (!_userService.CheckTelExist(queryModel.Tel))
            {
                errorMsg = "帐号不存在，请先注册！";
            }
            else
            {
                //获取用户信息
                UT_Users user = await _userService.CheckUserLoginTelAsync(queryModel.Tel, queryModel.PassWord);

                if (user != null)
                {
                    if (user.Status == 0)
                    {
                        //异步添加登录记录，同时保存登录信息。
                        UT_UserLoginRecord ulr = new UT_UserLoginRecord();
                        ulr.UserId = user.ID;
                        ulr.LoginName = user.Tel;
                        ulr.LoginIp = WebHelper.GetIP(Request);
                        ulr.Entrance = queryModel.LoginTerminal;
                        ulr.LoginDate = DateTime.Now;
                        //建立Task进行异步。
                        var insertTaskAsync = _userLoginInfoService.InsertAsync(ulr);

                        //获取体形数据
                        var userShapeAsync = _userShapeService.GetUserShapeAsync(user.ID);

                        //手环设备
                        var deviceBraceletAsync =  _deviceBraceletService.GetEntityAsync(x => x.UserId == user.ID);

                        //用户配置
                        var dataResult = await _usersConfigService.GetEntitiesAsync(x => x.UserId == user.ID);
                        var notificaCall = dataResult.FirstOrDefault(x => x.Name == "NotificaCall");
                        var notificaSMS = dataResult.FirstOrDefault(x => x.Name == "NotificaSMS");
                        var notificaWeChat = dataResult.FirstOrDefault(x => x.Name == "NotificaWeChat");
                        var notificaQQ = dataResult.FirstOrDefault(x => x.Name == "NotificaQQ");
                        var liftWristLight = dataResult.FirstOrDefault(x => x.Name == "LiftWristLight");

                        //等待异步的完成。
                        await insertTaskAsync;
                        var deviceBracelet =await deviceBraceletAsync;
                        var userShape = await userShapeAsync;

                        double? Weight = null;
                        if (userShape != null)
                        {
                            Weight = userShape.Weight;
                        }

                        LoginUserInfo model = new LoginUserInfo() { ID = user.ID, Tel = user.Tel, Weight = Weight };
                        //生成token，保存登录信息。
                        var token = CommonHelper.RandomLoginToken();
                        WebUtil.SetApiUserSession(token, model);

                        return Ok(new
                        {
                            status = 1,
                            msg = "登录成功",
                            data = new
                            {
                                NickName = user.NickName,
                                Email = user.Email,
                                UserHead = user.UserHead.GetUserHeadCompleteUrl(),
                                Tel = user.Tel,
                                TrueName = user.TrueName,
                                Age = user.Age.ToString(),
                                Birthday = user.Birthday.ToString(),
                                Sex = user.Sex.ToString(),
                                Height = userShape == null ? "" : userShape.Height.ToString(),
                                Weight = userShape == null ? "" : userShape.Weight.ToString(),
                                MovingTarget = userShape == null ? "" : userShape.MovingTarget.ToString(),
                                BraceletIMEI = deviceBracelet == null ? "" : deviceBracelet.IMEI.ToString(),
                                BraceletVersion = deviceBracelet == null ? "" : deviceBracelet.Version.ToString(),
                                NotificaCall = notificaCall == null ? "0" : ((int)notificaCall.Status) + "",
                                NotificaSMS = notificaSMS == null ? "0" : ((int)notificaSMS.Status) + "",
                                NotificaWeChat = notificaWeChat == null ? "0" : ((int)notificaWeChat.Status) + "",
                                NotificaQQ = notificaQQ == null ? "0" : ((int)notificaQQ.Status) + "",
                                LiftWristLight = liftWristLight == null ? "0" : ((int)liftWristLight.Status) + "",
                                Token = token
                            }
                        });
                    }
                    else
                    {
                        errorMsg = "您的帐号已被锁定！";
                    }
                }
                else
                {
                    errorMsg = "密码不正确！";
                }
            }
            return Ok(new { status = 0, msg = errorMsg });

        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="authQueryint"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Logout()
        {
            LoginUserInfo model = WebUtil.GetApiUserSession();

            if (model != null)
            {
                WebUtil.RemoveApiUserSession();

                return Ok(new { status = 1, Msg = "退出成功！" });
            }
            else
            {
                return Ok(new { status = 0, Msg = "退出失败！" });
            }
        }
        /// <summary>
        /// 判断是token是否过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NoLogin]
        public IHttpActionResult Get()
        {
            LoginUserInfo model = WebUtil.GetApiUserSession();
            if (model != null)
            {
                return Ok(new
                {
                    status = 1,
                    msg = "已登录！",
                    data = new
                    {
                        id = model.ID,
                        tel = model.Tel,
                        trueName = model.TrueName,
                        updateConfigTime = UTConfig.SiteConfig.UpdateConfigTime
                    }
                });
            }
            else
            {
                return Ok(new { status = 0, msg = "未登录！" });
            }
        }
    }

    public class QueryLoginUser
    {
        public string Tel { get; set; }
        public string PassWord { get; set; }
        public string LoginTerminal { get; set; }
    }
}
