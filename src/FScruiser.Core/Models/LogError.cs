using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace FScruiser.Models
{
    public class LogError : Model_Base
    {
        [Field("LogID")]
        public string LogID { get; set; }

        [Field("LogNumber")]
        public int LogNumber { get; set; }
    }
}