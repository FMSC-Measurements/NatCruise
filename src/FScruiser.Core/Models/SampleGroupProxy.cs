using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("SampleGroup_V3")]
    public class SampleGroupProxy
    {
        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("StratumCode")]
        public string StratumCode { get; set; }

        public override string ToString()
        {
            return SampleGroupCode;
        }
    }
}
