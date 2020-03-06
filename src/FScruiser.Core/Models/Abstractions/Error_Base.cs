using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public abstract class Error_Base
    {
        [Field("Field")]
        public string Field { get; set; }

        [Field("Level")]
        public string Level { get; set; }

        [Field("Message")]
        public string Message { get; set; }

        [Field("IsResolved")]
        public bool IsResolved { get; set; }

        [Field("Resolution")]
        public string Resolution { get; set; }

        [Field("ResolutionInitials")]
        public string ResolutionInitials { get; set; }
    }
}
