using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.Eims;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    /// <summary>
    /// 大号资源API
    /// </summary>
    public class CallTransferNumController : ApiController
    {
        private ICallTransferNumService _callTransferNumService;
        private IOrderService _orderService;
        private IDeviceGoipService _deviceGoipService;
        public CallTransferNumController(ICallTransferNumService callTransferNumService, IOrderService orderService, IDeviceGoipService deviceGoipService)
        {
            this._callTransferNumService = callTransferNumService;
            this._orderService = orderService;
            this._deviceGoipService = deviceGoipService;
        }


        /// <summary>
        /// 呼叫转移
        /// 开启呼叫转移与短信功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> OpenCallTransferAndSMS(CallTransferBindingModel model)
        {
            if (string.IsNullOrEmpty(model.Iccid))
            {
                return Ok(new { status = 0, msg = "数据错误" });
            }
            var currentUser = WebUtil.GetApiUserSession();

            if (await _deviceGoipService.CheckIccIdExistsByNotUserAsync(currentUser.ID, model.Iccid))
            {
                return Ok(new { status = 0, msg = "该卡已存在绑定" });
            }

            //判断当前用户是否有激活套餐，有的话才可以开启呼叫转移
            if (await _orderService.IsStatusUsed(currentUser.ID))
            {
                var entity = await _callTransferNumService.GetUsableModel(currentUser.ID);
                if (entity == null)
                {
                    return Ok(new { status = 0, msg = "暂无可用呼叫转移资源" });
                }

                //设置呼叫转移
                string Type = "at";
                string Chs = "utf8";
                string Content = "";//命令

                //判断手机运营商
                switch (MobileCarriers(currentUser.Tel))
                {
                    //todo 考虑失败时更新用户已存在大号资源
                    case 0:
                        LoggerHelper.Error("api>CallTransfer>MobileCarriers Result 0," + currentUser.Tel);
                        break;
                    case 1:
                    case 2:
                        Content = string.Format("at+ccfc=0,3,{0}", entity.TelNum);
                        break;
                    case 3:
                        Content = string.Format("*{0} {1}", "72", entity.TelNum);
                        break;
                    case 4:
                        return Ok(new { status = 0, msg = "未识别出号码运营商" });
                        break;
                    default:
                        break;
                }

                EimsApi api = new EimsApi();
                var result = await api.ATUSSDSend(Type, null, null, model.Iccid, Content, Chs);
                if (result != null && result.status.Length > 0)
                {
                    //成功
                    if (result.status.Contains('0'))
                    {
                        int setResult = PhoneServerByMySqlServices.SetNameChange(entity.TelNum, currentUser.Tel, true);
                        if (setResult == 2)
                        {
                            return Ok(new { status = 0, msg = "系统繁忙，请重试" });
                        }
                        //设置使用Goip
                        if (!await _deviceGoipService.SetUsedAsync(currentUser.ID, model.Iccid))
                        {
                            return Ok(new { status = 0, msg = "设备繁忙，请重试" });
                        }
                        return Ok(new { status = 1, msg = "开启成功" });//呼叫转移成功
                    }
                    else
                    {
                        return Ok(new { status = 0, msg = "开启失败" + result.status });//呼叫转移失败
                    }
                }
                return Ok(new { status = 0, msg = "开启失败" });//呼叫转移调用失败
            }
            else
            {
                return Ok(new { status = 0, msg = "您没有激活任何套餐，无法开启电话接听！" });
            }
        }

        /// <summary>
        /// 取消呼叫转移
        /// 关闭呼叫转移与短信功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ClosedCallTransferAndSMS()
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            string IccId = null;

            var goipEntity = await _deviceGoipService.CheckUserGoipAsync(currentUser.ID);

            if (goipEntity == null || string.IsNullOrEmpty(goipEntity.IccId))
            {
                return Ok(new
                {
                    status = 0,
                    msg = "未开启此功能"//未开启呼叫转移与短信功能
                });
            }
            else
            {
                IccId = goipEntity.IccId;
            }

            var TransferNumModel = await _callTransferNumService.GetDisabledModel(currentUser.ID);

            if (TransferNumModel != null)
            {
                //设置取消呼叫转移
                string Type = "at";
                string Chs = "utf8";
                string Content = "";//命令
                switch (MobileCarriers(currentUser.Tel))
                {
                    case 0:
                        LoggerHelper.Error("api>CancelCallTransfer>MobileCarriers Result 0," + currentUser.Tel);
                        return Ok(new { status = 0, msg = "不是有效的手机号" });
                    case 1:
                    case 2:
                        Content = "at+ccfc=0,4";
                        break;
                    case 3:
                        Content = string.Format("*{0}", "720");
                        break;
                    case 4:
                        return Ok(new { status = 0, msg = "未识别出号码运营商" });
                    default:
                        break;
                }

                EimsApi api = new EimsApi();
                var result = await api.ATUSSDSend(Type, null, null, IccId, Content, Chs);
                if (result != null && result.status.Length > 0)
                {
                    //成功
                    if (result.status.Contains('0'))
                    {
                        int setResult = PhoneServerByMySqlServices.SetNameChange(TransferNumModel.TelNum, currentUser.Tel, false);
                        if (setResult == 2)
                        {
                            return Ok(new { status = 0, msg = "系统繁忙，请重试" });
                        }

                        //取消设备
                        int cancelResult = await _deviceGoipService.CancelUsedPortAsync(currentUser.ID);
                        if (cancelResult == 0)
                        {
                            return Ok(new { status = 0, msg = "设备处理失败" });
                        };

                        var entity = await _callTransferNumService.ModifyToUsable(currentUser.ID);

                        return Ok(new { status = 1, msg = "关闭成功" });
                    }
                    //失败
                    else
                    {
                        return Ok(new { status = 0, msg = "关闭失败" + result.status });
                    }
                }
                return Ok(new { status = 0, msg = "关闭失败" });//取消呼叫转移调用失败
            }
            else
            {
                errorMsg = "关闭失败！";
            }
            return Ok(new { status = 0, msg = errorMsg });
        }



        /// <summary>
        /// 获取一个空闲大号资源
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = WebUtil.GetApiUserSession();
            //判断当前用户是否有激活套餐，有的话才可以开启呼叫转移
            if (await _orderService.IsStatusUsed(currentUser.ID))
            {
                var model = await _callTransferNumService.GetUsableModel(currentUser.ID);

                return Ok(new { status = 1, data = model });
            }
            else
            {
                return Ok(new { status = 0, msg = "您没有激活任何套餐，无法开启电话接听！" });
            }
        }

        /// <summary>
        /// 取消用户空闲大号资源
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<IHttpActionResult> ModifyByStatus()
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            var model = await _callTransferNumService.ModifyToUsable(currentUser.ID);
            if (model)
            {
                errorMsg = "取消成功！";
            }
            else
            {
                errorMsg = "取消失败！";
            }
            return Ok(new { status = 1, msg = errorMsg });
        }

        /// <summary>
        /// 手机运营商
        /// </summary>
        /// <param name="tel"></param>
        /// <returns>
        /// 0：非有效手机号码
        /// 1：移动
        /// 2：联通
        /// 3：电信
        /// 4：无法识别
        /// </returns>
        private int MobileCarriers(string mobile)
        {
            var yd = new string[] { "134", "135", "136", "137", "138", "139", "147", "150", "151", "152", "157", "158", "159", "182", "183", "184", "187", "188" };/*移动*/
            var lt = new string[] { "130", "131", "132", "145", "155", "156", "185", "186" };/*联通*/
            var dx = new string[] { "133", "153", "180", "181", "189" }; /*电信*/

            if (!ValidateHelper.IsMobile(mobile))
            {
                return 0;
            };

            string subMobile = mobile.Substring(0, 3);
            if (yd.Contains(subMobile))
            {
                return 1;
            }
            else if (lt.Contains(subMobile))
            {
                return 2;
            }
            else if (dx.Contains(subMobile))
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
}
