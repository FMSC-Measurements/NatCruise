using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class LogError : Error_Base
    {
        [Field("LogID")]
        public string LogID { get; set; }

        [Field("LogNumber")]
        public int LogNumber { get; set; }
    }
}
