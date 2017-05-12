using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class PushContentController : ApiController
    {
        private IPushContentService _pushContentService;

        public PushContentController(IPushContentService pushContentService)
        {
            this._pushContentService = pushContentService;
        }

        /// <summary>
        /// 查询所有推送内容
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get()
        {
            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var dataResult = await _pushContentService.GetAll();


            var data = from i in dataResult.OrderByDescending(x => x.CreateDate)
                       select new
                       {
                           ID = i.ID,
                           Title = i.Title,
                           Image = i.Image.GetCompleteUrl(),
                       };

            return Ok(new
            {
                status = 1,
                data =  //dic,
                new { list = data }
            });
        }

        [NoAuthenticate]
        /// <summary>
        /// 随机4000
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> Getadasdasdzczxczxc()
        {
            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var dataResult = await _pushContentService.GetAll();

            List<string> sb = new List<string>();
            for (int i = 0; i < 4000; i++)
            {
                string pwd = "0001" + GetRandomCardPwd();
                while (true)
                {
                    if (!sb.Contains(pwd))
                    {
                        break;
                    }
                    else
                    {
                        pwd = "0001" + GetRandomCardPwd();
                    }
                }
                sb.Add(pwd);
            }
            var result = sb.GroupBy(x => x);

            return sb;
            //return Ok(new
            //{
            //    status = 1,
            //    data = sb.ToString()
            //});
        }


        private string GetRandomCardPwd()
        {
            string[] s2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            string cardPwd = string.Empty;

            Random rdm = new Random(GetRandomSeed());

            //Random rdm2 = new Random(GetRandomSeed());
            //int rngNum2 = rdm.Next(1000000000, 99999999);

            for (int i = 0; i < 6; i++)
            {
                int rngNum = rdm.Next(0, 9);

                cardPwd += s2[rngNum];
            }

            return cardPwd;
        }

        /// <summary>
        /// 获取随机种子
        /// </summary>
        /// <returns></returns>
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}
