﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class OrderZCBindingModels
    {
        public string Tel { get; set; }
        public string SmsVerCode { get; set; }
    }
    public class GetUserOrderZCListBindingModel
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string CallPhone { get; set; }
    }
}
