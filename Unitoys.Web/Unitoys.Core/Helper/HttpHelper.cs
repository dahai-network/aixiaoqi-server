using System;
using System.Web;
using System.Collections;
using System.Web.Caching;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Unitoys.Core
{
    public class HttpHelper
    {
        public static readonly HttpHelper Instance = new HttpHelper();

        private static readonly HttpClient _httpClient;

        private const string BASE_ADDRESS = "http://apitest.unitoys.com";
        static HttpHelper()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri(BASE_ADDRESS) };
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            //帮HttpClient热身
            _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = new HttpMethod("HEAD"),
                RequestUri = new Uri(BASE_ADDRESS + "/")
            })
                .Result.EnsureSuccessStatusCode();
        }

        public async Task<string> PostAsync(string url,Dictionary<string, string> dt)
        {
            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(dt));

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }
    }
}