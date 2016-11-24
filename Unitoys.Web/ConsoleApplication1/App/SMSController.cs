using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Web.Models;

namespace Unitoys.Web.ApiControllersApp
{
    public class SMSController : ApiController
    {
        private ISMSService smsService;

        public SMSController() { }
        public SMSController(ISMSService smsService)
        {
            this.smsService = smsService;
        }
        /// <summary>
        /// 根据短信ID获取
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public HttpResponseMessage Get([FromUri]AuthQuery authQueryint,Guid SMSID)
        {
            UT_SMS modal = smsService.GetEntityById(SMSID);
            if (modal != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    type = "success",
                    data = new
                    {
                        ID = modal.ID,
                        LoginName = modal.LoginName,
                        FromNum = modal.FromNum,
                        ToNum = modal.ToNum,
                        SendTime = modal.SendDate.ToString(),
                        SMSContent = modal.SMSContent,
                        MsgType = modal.MsgType,
                        IsRead = modal.IsRead
                    }
                });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    type = "Error",
                    Msg = "查找的内容不存在！"
                });
            }
        }

        // POST api/sms
        public void Post([FromBody]string value)
        {
        }

        // PUT api/sms/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/sms/5
        public void Delete(int id)
        {
        }
    }
}
