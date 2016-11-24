using RestSharp.Contrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class CdnRequest
    {
        const string CDN_SERVICE_BASE_ADDRESS = "cdn.aliyuncs.com";

        public string Format = "JSON";

        public string Version = "2014-11-11";

        public string AccessKeyId = "5Ed6MLmrrukPtr26";

        public string Signature;

        public string SignatureMethod = "HMAC-SHA1";

        public string TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        public string SignatureVersion = "1.0";

        public string SignatureNonce = Guid.NewGuid().ToString();

        private HttpMethod _httpMethod;

        private string AccessKeySecret = "kiNiyHw9rGyo5nQMu6KLjI7MlbOX1t";

        private Dictionary<string, string> _parameters;
        public CdnRequest(HttpMethod httpMethod, Dictionary<string, string> parameters)
        {
            _httpMethod = httpMethod;
            _parameters = parameters;
        }
        private void BuildParameters()
        {
            _parameters.Add("Format", Format.ToString().ToUpper());
            _parameters.Add("Version", Version);
            _parameters.Add("AccessKeyId", AccessKeyId);
            _parameters.Add("SignatureVersion", SignatureVersion);
            _parameters.Add("SignatureMethod", SignatureMethod);
            _parameters.Add("SignatureNonce", SignatureNonce);
            _parameters.Add("TimeStamp", TimeStamp);
        }
        public void ComputeSignature()
        {
            BuildParameters();
            var canonicalizedQueryString = string.Join("&",
                _parameters.OrderBy(x => x.Key)
                .Select(x => PercentEncode(x.Key) + "=" + PercentEncode(x.Value)));

            var stringToSign = _httpMethod.ToString().ToUpper() + "&%2F&" + PercentEncode(canonicalizedQueryString);

            var keyBytes = Encoding.UTF8.GetBytes(AccessKeySecret + "&");
            var hmac = new HMACSHA1(keyBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            Signature = Convert.ToBase64String(hashBytes);
            _parameters.Add("Signature", Signature);
        }
        private string PercentEncode(string value)
        {
            return UpperCaseUrlEncode(value)
                .Replace("+", "%20")
                .Replace("*", "%2A")
                .Replace("%7E", "~");
        }
        private static string UpperCaseUrlEncode(string s)
        {
            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }
        public string GetUrl()
        {
            ComputeSignature();
            return CDN_SERVICE_BASE_ADDRESS + "?" +
                string.Join("&", _parameters.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value)));
        }
        public static async Task<bool> RefreshObjectCaches(string objectPath)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "Action", "RefreshObjectCaches" },
                { "ObjectPath", objectPath }
            };
            var request = new CdnRequest(HttpMethod.Get, parameters);
            var url = request.GetUrl();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return true;
            }
        }
    }
}
