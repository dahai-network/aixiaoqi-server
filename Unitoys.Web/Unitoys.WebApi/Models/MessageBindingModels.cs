using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.WebApi.Models
{
    public class AddMessageBindingModel
    {
        public string Country { get; set; }
        public string Location { get; set; }
        public string Content { get; set; }
    }

    public class AddMessageCommentBindingModel
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; }
    }
}
