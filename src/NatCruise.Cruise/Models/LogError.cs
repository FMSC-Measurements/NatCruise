using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    public class LogError : Model_Base
    {
        public string LogID { get; set; }

        public int LogNumber { get; set; }
    }
}