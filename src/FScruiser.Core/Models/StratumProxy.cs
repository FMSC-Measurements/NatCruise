using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("Stratum")]
    public class StratumProxy
    {
        //[Field(Name = "Stratum_CN")]
        //public string Stratum_CN { get; set; }

        [Field(Name = "Code")]
        public string Code { get; set; }

        [Field(Name = "Method")]
        public string Method { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
