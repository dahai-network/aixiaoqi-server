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
    public class CountryController : ApiController
    {
        private ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            this._countryService = countryService;
        }

        [HttpGet]
        /// <summary>
        /// 查询热门国家
        /// </summary>
        /// <param name="pageSize">返回前多少条数据</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetHot(int pageSize)
        {
            Expression<Func<UT_Country, bool>> exp;
            exp = x => x.IsHot == true;
            //排序Expression
            Expression<Func<UT_Country, object>> packageExp = x => new { x.DisplayOrder };

            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var dataResult = _countryService.GetEntitiesForPagingAsync(1, pageSize, packageExp, "asc", exp);


            var data = from i in await dataResult
                       select new
                       {
                           CountryID = i.ID,
                           CountryName = i.CountryName,
                           CountryCode = i.CountryCode,
                           Pic = i.Pic.GetCountryPicCompleteUrl(),
                           LogoPic = i.LogoPic.GetPackageCompleteUrl(),
                           Rate = i.Rate,
                       };
            return Ok(new { status = 1, data = data });
        }

        /// <summary>
        /// 查询所有国家
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get()
        {
            //var packageResult = await _packageService.GetEntitiesAsync(exp);
            //如果查询条件不为空，则根据查询条件查询，反则查询所有。
            var dataResult = await _countryService.GetAll();


            var data = from i in dataResult.OrderBy(x => x.DisplayOrder)
                       select new
                       {
                           CountryID = i.ID,
                           CountryName = i.CountryName,
                           CountryCode = i.CountryCode,
                           Pic = i.Pic.GetCountryPicCompleteUrl(),
                           LogoPic = i.LogoPic.GetPackageCompleteUrl(),
                           Rate = i.Rate,
                           IsHot = i.IsHot,
                           Continents = i.Continents,
                           ContinentsDescr = i.Continents.GetDescription()
                       };

            var dic = data.GroupBy(x => x.ContinentsDescr);
            //var dic = data.GroupBy(x => x.ContinentsDescr).ToDictionary(x => x.Key).ToList();
            //var dic = data.GroupBy(x => x.ContinentsDescr).Select(x => new ContinentsCountryBindingResponseModels() { ContinentsDescr = x.Key, ContinentsCountry = x });

            //var dic = data.GroupBy(x => x.ContinentsDescr).Select(x=>new KeyValuePair<string,dynamic>(x.Key,x));

            return Ok(new
            {
                status = 1,
                data =  //dic,
                new dynamic[]
                {
                    //TODO 洲的显示顺序先写死，由于没有排列洲的规则，先按参考应用来
                    data.Where(x => x.Continents == ContinentsType.Asia).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.Oceania).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.Europe).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.NorthAmerica).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.SouthAmerica).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.Africa).ToArray(),
                    data.Where(x => x.Continents == ContinentsType.Antarctica).ToArray(),
                }
            });
        }

    }
}
