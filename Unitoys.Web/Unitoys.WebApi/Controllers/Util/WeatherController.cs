using Jayrock.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Unitoys.WebApi.Controllers.Util
{
    public class WeatherController:ApiController
    {

        [HttpGet]
        public IHttpActionResult Get(string city)
        {
            using(HttpClient client = new HttpClient())
            {
               
                try
                {
                    var result = client.GetStringAsync(String.Format("https://api.thinkpage.cn/v3/weather/daily.json?location={0}&language=zh-Hans&unit=c&aqi=city&key=TJ2XLU03PI", city)).Result;

                    JsonReader jsonRead = new JsonTextReader(new StringReader(result));
                    JsonObject jsonobj = new JsonObject();

                    jsonobj.Import(jsonRead);
                    JsonArray ja = jsonobj["results"] as JsonArray;

                    JsonObject cityJson = ja[0] as JsonObject;

                    JsonObject location = cityJson["location"] as JsonObject;

                    JsonArray daily = cityJson["daily"] as JsonArray;

                    return Ok(new
                    {
                        status = 1,
                        msg = "success",
                        data = new
                        {
                            city_name = location["name"],
                            last_update = cityJson["last_update"],
                            future = daily
                        }
                    });
                }
                catch
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "error"
                    });
                }
                
            }            
        }
    }
}
