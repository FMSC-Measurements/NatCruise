using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using System.Linq;

namespace FScruiser.Models
{
    [Table("Stratum")]
    public class Stratum
    {
        [Field(Name = "Code")]
        public string Code { get; set; }

        [Field(Name = "Description")]
        public string Description { get; set; }

        [Field(Name = "Method")]
        public string Method { get; set; }

        [Field(Name = "Hotkey")]
        public string Hotkey { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);
    }
}