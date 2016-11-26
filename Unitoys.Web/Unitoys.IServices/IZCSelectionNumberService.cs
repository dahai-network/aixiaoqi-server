using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.IServices
{
    public interface IZCSelectionNumberService : IBaseService<UT_ZCSelectionNumber>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="provinceName">省</param>
        /// <param name="cityName">市</param>
        /// <param name="mobileNumber">手机号码</param>
        /// <param name="userTel">用户手机号</param>
        /// <returns></returns>
        Task<KeyValuePair<int, List<UT_ZCSelectionNumber>>> SearchAsync(int page, int rows, string provinceName, string cityName, string mobileNumber, string userTel);
    }
}
