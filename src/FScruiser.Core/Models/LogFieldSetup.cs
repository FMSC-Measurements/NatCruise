using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("LogFieldSetup_V3")]
    public class LogFieldSetup
    {
        [Field("Field")]
        public string Field { get; set; }

        //public int FieldOrder { get; set; }
        [Field("Heading")]
        public string Heading { get; set; }

    }
}
