using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.WebApi.Models;

namespace Unitoys.WebApi.Controllers
{
    public class OrderUsageController : ApiController
    {
        private IOrderUsageService _orderUsageService;
        public OrderUsageController(IOrderUsageService orderUsageService)
        {
            this._orderUsageService = orderUsageService;
        }

        /// <summary>
        /// 获取当月流量使用明细
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页数</param>
        /// <param name="row">行数</param>
        /// <param name="month">查询的月份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUsageRecordForMonth(int page, int row, int month = 0)
        {
            var currentUser = WebUtil.GetApiUserSession();

            List<KeyValuePair<string, object>> result = await _orderUsageService.GetOrderItemUsageRecordForMonth(currentUser.ID, page, row, month);

            return Ok(new { status = 1, data = result });
        }

        /// <summary>
        /// 添加流量使用记录
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddUsageRecord([FromBody]AddUsageRecordBindingModel model)
        {
            string errorMsg = "";

            var currentUser = WebUtil.GetApiUserSession();

            if (currentUser == null)
            {
                errorMsg = "用户不能为空！";
            }
            else if(model.UsedFlow == 0)
            {
                errorMsg = "使用流量必须大于0！";
            }
            else
            {
                if (await _orderUsageService.AddOrderItemUsageRecordAsync(currentUser.ID, model.UsedFlow, model.StartDate, model.EndDate))
                {
                    return Ok(new { status = 1, msg = "添加成功！" });
                }
            }
            return Ok(new { status = 0, msg = "添加失败！" + errorMsg });
        }
    }
}
