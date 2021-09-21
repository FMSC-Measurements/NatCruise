using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Models
{
    public class TreeField
    {
        public string Field { get; set; }

        public string Heading { get; set; }

        public string DbType { get; set; }

        public override string ToString()
        {
            return Heading;
        }
    }
}
