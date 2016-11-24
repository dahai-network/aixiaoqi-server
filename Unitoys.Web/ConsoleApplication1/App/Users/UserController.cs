using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;

namespace Unitoys.WebApi
{
    public class UserController : ApiController
    {
        private IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
       

        
    }
}
