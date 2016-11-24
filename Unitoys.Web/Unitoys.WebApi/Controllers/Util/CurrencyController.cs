using Jayrock.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Unitoys.WebApi.Controllers.Util
{
    public class CurrencyController:ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get(string fromCurrency, string toCurrency, decimal amount = 0)
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("apikey","139941b18a24230bde60d7e6dd0f5f7a");

                var result = await client.GetStringAsync(String.Format("http://apis.baidu.com/apistore/currencyservice/currency?fromCurrency={0}&toCurrency={1}&amount={2}", fromCurrency, toCurrency, amount));

                CurrencyConvertModel model = JsonConvert.DeserializeObject<CurrencyConvertModel>(result);

                return Ok(new
                {
                    status = 1,
                    msg = "成功",
                    data = model.retData
                });
            }
        }

        /// <summary>
        /// 获取支持的币种
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrencyType()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("apikey", "139941b18a24230bde60d7e6dd0f5f7a");

                var result = await client.GetStringAsync("http://apis.baidu.com/apistore/currencyservice/type");

                CurrencyTypeModel model = JsonConvert.DeserializeObject<CurrencyTypeModel>(result);

                List<string> currencyType_EN = new List<string>();
                List<string> currencyType_CN = new List<string>();

                foreach (var item in model.retData)
                {
                    for (int i = 0; i < model.CURRENCY_EN.Length; i++)
                    {
                        if(item.ToUpper() == model.CURRENCY_EN[i].ToUpper())
                        {                            
                            currencyType_EN.Add(item);
                            currencyType_CN.Add(model.CURRENCY_ZH[i]);
                            break;
                        }
                    }
                }
                return Ok(new { status = 1, msg = "success", data = new { currencytype_en = currencyType_EN, currencytype_cn = currencyType_CN } });
            }
        }

    }

    public class CurrencyConvertModel
    {
        public int errNum { get; set; }
        public string errMsg { get; set; }
        public Dictionary<string, string> retData { get; set; }
    }

    public class CurrencyTypeModel
    {
        public string[] CURRENCY_EN = new string[] { "AED", "AUD", "MOP", "DZD", "OMR", "EGP", "BYR", "BRL", "PLN", "BHD", "BGN", "ISK", "DKK", "RUB", "PHP", "HKD", "COP", "CRC", "KER", "CAD", "CZK", "KHR", "HRK", "QAR", "KWD", "KES", "LAK", "RON", "LBP", "USD", "BUK", "MYR", "MAD", "MXN", "NOK", "ZAR", "EUR", "CNY", "CHF", "JPY", "SEK", "SAR", "LKR", "RSD", "THB", "TZS", "BND", "UGX", "ZMK", "SYP", "NZD", "TRY", "SGD", "TWD", "HUF", "GBP", "JOD", "IQD", "VND", "ILS", "INR", "IDR", "CLP" };
        public string[] CURRENCY_ZH = new string[] { "阿联酋迪拉姆", "澳元", "澳门元", "阿尔及利亚第纳尔", "阿曼里亚尔", "埃及磅", "白俄罗斯卢布", "巴西雷亚尔", "波兰兹罗提", "巴林第纳尔", "保加利亚列弗", "冰岛克朗", "丹麦克朗", "俄罗斯卢布", "菲律宾比索", "港元", "哥伦比亚比索", "哥斯达黎加科朗", "韩元", "加元", "捷克克朗", "柬埔寨瑞尔", "克罗地亚库纳", "卡塔尔里亚尔", "科威特第纳尔", "肯尼亚先令", "老挝基普", "罗马尼亚列伊", "黎巴嫩镑", "美元", "缅甸元", "马来西亚林吉特", "摩洛哥道拉姆", "墨西哥元", "挪威克朗", "南非兰特", "欧元", "人民币元", "瑞士法郎", "日元", "瑞典克朗", "沙特里亚尔", "斯里兰卡卢比", "塞尔维亚第纳尔", "泰铢", "坦桑尼亚先令", "文莱元", "乌干达先令", "新的赞比亚克瓦查", "叙利亚镑", "新西兰元", "新土耳其里拉", "新加坡元", "新台币", "匈牙利福林", "英镑", "约旦第纳尔", "伊拉克第纳尔", "越南盾", "以色列新锡克尔", "印度卢比", "印尼卢比", "智利比索" };
        public int errNum { get; set; }
        public string errMsg { get; set; }
        public string[] retData { get; set; }
    }
}
