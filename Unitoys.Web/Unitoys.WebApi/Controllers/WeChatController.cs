using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using Unitoys.Core;
using Unitoys.Core.JiGuang;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;
using XKSocket;

namespace Unitoys.WebApi.Controllers
{
    [NoAuthenticate]
    public class WeChatController : ApiController
    {
        public string token = "aixiaoqi20160914";
        public WeChatController(IPaymentService paymentService)
        {

        }
        static string content = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">随机字符串</param>
        /// <returns></returns>
        public HttpResponseMessage Get(string signature, string timestamp, string nonce, string echostr)
        {
            #region 日志
            content += Environment.NewLine + Environment.NewLine + "微信Get" + DateTime.Now.ToString() + Environment.NewLine;

            content += "微信Get BodyInputStream:" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd() + Environment.NewLine;
            content += "微信Get Content:" + Request.Content.ReadAsStringAsync().Result + Environment.NewLine;
            content += "微信Get QueryString:" + System.Web.HttpContext.Current.Request.QueryString.ToString();

            #endregion

            //校验
            if (CheckSource(signature, timestamp, nonce))
            {
                var result = new StringContent(echostr, UTF8Encoding.UTF8, "text/plain");
                var response = new HttpResponseMessage { Content = result };
                return response;
            }
            else
            {
                var result = new StringContent(content, UTF8Encoding.UTF8, "text/plain");
                var response = new HttpResponseMessage { Content = result };
                return response;
            }

            //DeviceApi c = new DeviceApi();
            //c.GetClient();
            return new HttpResponseMessage();

            //return null;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {

            //content += DateTime.Now.ToString();
            //string signature, string timestamp, string nonce, string echostr
            //content += await Request.Content.ReadAsStringAsync();
            //XmlDocument
            content += Environment.NewLine + Environment.NewLine + "微信Post" + DateTime.Now.ToString() + Environment.NewLine;
            #region 日志
            content += System.Web.HttpContext.Current.Request.Form.ToString() + Environment.NewLine;
            content += "微信Post BodyInputStream:" + new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd() + Environment.NewLine;
            content += "微信Post Content:" + Request.Content.ReadAsStringAsync().Result + Environment.NewLine;
            content += "微信Post QueryString:" + System.Web.HttpContext.Current.Request.QueryString.ToString();
            #endregion

            try
            {

                //接受消息
                XElement root = XElement.Parse(await Request.Content.ReadAsStringAsync());
                string ToUserName = root.Element("ToUserName").Value;
                string FromUserName = root.Element("FromUserName").Value;
                string CreateTime = root.Element("CreateTime").Value;
                string MsgType = root.Element("MsgType").Value;
                string MsgId = root.Element("MsgId").Value;

                string responseMsgXml = "success";
                if (MsgType == "text")
                {
                    string Content = root.Element("Content").Value;

                    //你问我答
                    //被动回复文本消息
                    responseMsgXml = @"<xml>
                <ToUserName><![CDATA[" + FromUserName + @"]]></ToUserName>
                <FromUserName><![CDATA[" + ToUserName + @"]]></FromUserName>
                <CreateTime>" + CommonHelper.ConvertDateTimeInt(DateTime.Now) + @"</CreateTime>
                <MsgType><![CDATA[text]]></MsgType>
                <Content><![CDATA[" + Content + @"]]></Content>
                </xml>";
                }
                else if (MsgType == "image")
                {
                    string PicUrl = root.Element("PicUrl").Value;
                    string MediaId = root.Element("MediaId").Value;

                    //“图”尚往来
                    //被动回复图片消息
                    responseMsgXml = @"<xml>
                <ToUserName><![CDATA[" + FromUserName + @"]]></ToUserName>
                <FromUserName><![CDATA[" + ToUserName + @"]]></FromUserName>
                <CreateTime>" + CommonHelper.ConvertDateTimeInt(DateTime.Now) + @"</CreateTime>
                <MsgType><![CDATA[image]]></MsgType>
                <Image>
                <MediaId><![CDATA[" + MediaId + @"]]></MediaId>
                </Image>
                </xml>";
                }

                var result = new StringContent(responseMsgXml, UTF8Encoding.UTF8, "text/plain");
                var response = new HttpResponseMessage { Content = result };
                return response;
            }
            catch (Exception ex)
            {
                content += ex.Message;
                throw;
            }
            //return new HttpResponseMessage();
        }

        #region 方法

        //检验是否来自微信的签名
        private bool CheckSource(string signature, string timestamp, string nonce)
        {
            var str = string.Empty;
            var parameter = new List<string> { token, timestamp, nonce };
            parameter.Sort();
            var parameterStr = parameter[0] + parameter[1] + parameter[2];
            var tempStr = SecureHelper.SHA1_Hash(parameterStr).Replace("-", "").ToLower();
            if (tempStr == signature)
                return true;
            return false;
        }
        #endregion
    }
}
