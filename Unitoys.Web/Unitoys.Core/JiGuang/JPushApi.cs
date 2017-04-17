using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.push.mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core.JiGuang
{
    public class JPushApi
    {
        public static String ALERT = "alert";
        public static String MSG_CONTENT = "Test from C# v3 sdk - msgContent";
        private static String app_key = "203b0b8a6747e85d18779ce0";
        private static String master_secret = "ea2a851deaba1b8eb1272f35";
        //广播所有人
        //设备标签(Tag)
        //设备别名(Alias)
        public void Push_all_alert(string title, Dictionary<string, string> dicExtra)
        {
            JPushClient client = new JPushClient(app_key, master_secret);
            PushPayload payload = PushObject_All_All_alert(title, dicExtra);
            client.SendPush(payload);
        }
        public void Push_all_alias_alert(string alias, string title, string alert, Dictionary<string, string> dicExtra)
        {
            JPushClient client = new JPushClient(app_key, master_secret);
            PushPayload payload = PushObject_all_alias_alert(alias, title, dicExtra);
            payload.notification.setAlert(alert);
            //设置IOS推送环境，生产或开发
            payload.ResetOptionsApnsProduction(UTConfig.SiteConfig.IOSApnsProduction != "0");
            client.SendPush(payload);
        }
        public bool Push_ios_alias_alert(string alias, string title, string alert, Dictionary<string, string> dicExtra)
        {
            try
            {
                JPushClient client = new JPushClient(app_key, master_secret);
                PushPayload payload = PushObject_ios_alias_alert(alias, title, dicExtra);
                payload.notification.setAlert(alert);
                //设置IOS推送环境，生产或开发
                payload.ResetOptionsApnsProduction(UTConfig.SiteConfig.IOSApnsProduction != "0");
                client.SendPush(payload);
                return true;
            }
            catch (APIRequestException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Error response from JPush server. Should review and fix it. ");
                sb.AppendLine("HTTP Status: " + e.Status);
                sb.AppendLine("Error Code: " + e.ErrorCode);
                sb.AppendLine("Error Message: " + e.ErrorMessage);
                //throw new Exception(sb.ToString());
                LoggerHelper.Error(sb.ToString(), e);
            }
            catch (APIConnectionException e)
            {
                LoggerHelper.Error(e.Message, e);
            }
            return false;
        }

        public bool Push_android_alias_message(string alias, string msgContent, string contentType, Dictionary<string, string> dicExtra)
        {
            try
            {
                JPushClient client = new JPushClient(app_key, master_secret);
                PushPayload payload = PushObject_android_alias_message(alias, msgContent, contentType, dicExtra);
                //设置IOS推送环境，生产或开发
                payload.ResetOptionsApnsProduction(UTConfig.SiteConfig.IOSApnsProduction != "0");
                client.SendPush(payload);
                return true;
            }
            catch (APIRequestException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Error response from JPush server. Should review and fix it. ");
                sb.AppendLine("HTTP Status: " + e.Status);
                sb.AppendLine("Error Code: " + e.ErrorCode);
                sb.AppendLine("Error Message: " + e.ErrorMessage);
                //throw new Exception(sb.ToString());
                LoggerHelper.Error(sb.ToString(), e);
            }
            catch (APIConnectionException e)
            {
                LoggerHelper.Error(e.Message, e);
            }
            return false;
        }
        public bool Push_all_alias_message(string alias, string msgContent, string contentType, Dictionary<string, string> dicExtra)
        {
            try
            {
                JPushClient client = new JPushClient(app_key, master_secret);
                PushPayload payload = PushObject_all_alias_message(alias, msgContent, contentType, dicExtra);
                //设置IOS推送环境，生产或开发
                payload.ResetOptionsApnsProduction(UTConfig.SiteConfig.IOSApnsProduction != "0");
                client.SendPush(payload);
                return true;
            }
            catch (APIRequestException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Error response from JPush server. Should review and fix it. ");
                sb.AppendLine("HTTP Status: " + e.Status);
                sb.AppendLine("Error Code: " + e.ErrorCode);
                sb.AppendLine("Error Message: " + e.ErrorMessage);
                //throw new Exception(sb.ToString());
                LoggerHelper.Error(sb.ToString(), e);
            }
            catch (APIConnectionException e)
            {
                LoggerHelper.Error(e.Message, e);
            }
            return false;
        }

        /// <summary>
        /// 发送所有平台所有人
        /// </summary>
        /// <returns></returns>
        private PushPayload PushObject_All_All_alert(string title, Dictionary<string, string> dicExtra)
        {

            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.all();

            //通知
            var notification = new Notification().setAlert(ALERT);
            notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification();
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            notification.AndroidNotification.setTitle(title);

            foreach (var item in dicExtra)
            {
                notification.AndroidNotification.AddExtra(item.Key, item.Value);
                notification.IosNotification.AddExtra(item.Key, item.Value);
            }
            pushPayload.notification = notification;

            //消息内容
            //var message = Message.content(MSG_CONTENT)
            //                  .AddExtras("key1", "value1")
            //                  .AddExtras("key2", 222)
            //                  .AddExtras("key3", false);
            return pushPayload;
        }

        /// <summary>
        /// 发送所有平台通知根据别名
        /// </summary>
        /// <returns></returns>
        private static PushPayload PushObject_all_alias_alert(string alias, string title, Dictionary<string, string> dicExtra)
        {

            PushPayload pushPayload_alias = new PushPayload();
            pushPayload_alias.platform = Platform.all();
            pushPayload_alias.audience = Audience.s_alias(alias);

            //通知
            var notification = new Notification().setAlert(ALERT);
            notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification();
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            notification.AndroidNotification.setTitle(title);
            foreach (var item in dicExtra)
            {
                notification.AndroidNotification.AddExtra(item.Key, item.Value);
                notification.IosNotification.AddExtra(item.Key, item.Value);
            }
            pushPayload_alias.notification = notification;

            //消息内容
            //var message = Message.content(MSG_CONTENT)
            //                  .AddExtras("key1", "value1")
            //                  .AddExtras("key2", 222)
            //                  .AddExtras("key3", false);
            return pushPayload_alias;
        }
        /// <summary>
        /// 发送所有平台通知根据别名
        /// </summary>
        /// <returns></returns>
        private static PushPayload PushObject_ios_alias_alert(string alias, string title, Dictionary<string, string> dicExtra)
        {

            PushPayload pushPayload_alias = new PushPayload();
            pushPayload_alias.platform = Platform.ios();
            pushPayload_alias.audience = Audience.s_alias(alias);

            //通知
            var notification = new Notification().setAlert(ALERT);
            //notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification();
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            notification.IosNotification.setSound("default");
            notification.IosNotification.setAlert(title);
            //notification.AndroidNotification.setTitle(title);
            foreach (var item in dicExtra)
            {
                //notification.AndroidNotification.AddExtra(item.Key, item.Value);
                notification.IosNotification.AddExtra(item.Key, item.Value);
            }
            pushPayload_alias.notification = notification;

            //消息内容
            //var message = Message.content(MSG_CONTENT)
            //                  .AddExtras("key1", "value1")
            //                  .AddExtras("key2", 222)
            //                  .AddExtras("key3", false);
            return pushPayload_alias;
        }

        /// <summary>
        /// 发送所有平台自定义消息根据别名
        /// </summary>
        /// <returns></returns>
        private static PushPayload PushObject_all_alias_message(string alias, string msgContent, string contentType, Dictionary<string, string> dicExtra)
        {

            PushPayload pushPayload_alias = new PushPayload();
            pushPayload_alias.platform = Platform.all();
            pushPayload_alias.audience = Audience.s_alias(alias);

            //自定义消息
            pushPayload_alias.message = Message.content(msgContent).setContentType(contentType);

            //通知
            foreach (var item in dicExtra)
            {
                pushPayload_alias.message.AddExtras(item.Key, item.Value);
            }

            //消息内容
            //var message = Message.content(MSG_CONTENT)
            //                  .AddExtras("key1", "value1")
            //                  .AddExtras("key2", 222)
            //                  .AddExtras("key3", false);
            return pushPayload_alias;
        }

        /// <summary>
        /// 发送所有平台自定义消息根据别名
        /// </summary>
        /// <returns></returns>
        private static PushPayload PushObject_android_alias_message(string alias, string msgContent, string contentType, Dictionary<string, string> dicExtra)
        {

            PushPayload pushPayload_alias = new PushPayload();
            pushPayload_alias.platform = Platform.android();
            pushPayload_alias.audience = Audience.s_alias(alias);

            //自定义消息
            pushPayload_alias.message = Message.content(msgContent).setContentType(contentType);

            //通知
            foreach (var item in dicExtra)
            {
                pushPayload_alias.message.AddExtras(item.Key, item.Value);
            }

            //消息内容
            //var message = Message.content(MSG_CONTENT)
            //                  .AddExtras("key1", "value1")
            //                  .AddExtras("key2", 222)
            //                  .AddExtras("key3", false);
            return pushPayload_alias;
        }

        //Registration ID
        //用户分群推送

    }
}
