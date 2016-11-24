using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Runtime.Serialization;
using System.Net.Http.Headers;
using Unitoys.Core;

namespace Unitoys.WebApi.Controllers.Util
{
    public class TranslateController:ApiController
    {

        private const string appId = "20160229000013981";

        private const string secretKey = "tPbBRYWUq6BPi_eVl_Ee";
        /// <summary>
        /// 百度翻译
        /// </summary>
        /// <param name="q"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get(string q, string from ="auto", string to ="auto")
        {
            Random rd = new Random();

            string salt = rd.Next(10000).ToString();

            string sign = SecureHelper.MD5(String.Format("{0}{1}{2}{3}",appId,q,salt,secretKey));

            using(HttpClient client = new HttpClient())
            {
                var responseMsg = await client.GetAsync(String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?appid={0}&salt={1}&sign={2}&q={3}&from={4}&to={5}", appId, salt, sign,q, from, to));

                if (responseMsg.IsSuccessStatusCode)
                {
                    TranslateModel model = await responseMsg.Content.ReadAsAsync<TranslateModel>();

                    return Ok(new { status = 1, msg = "成功", data = model });
                }

                return Ok(new { status = 0, msg = "翻译失败" });
            }            
        }
    }

    public class TranslateModel
    {
        public string from { get; set; }
        public string to { get; set; }
        public List<Dictionary<string, string>> trans_result { get; set; }
    }
}
