using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class UT_Entity
    {
        public UT_Entity()
        {
            this.ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
    }
}
