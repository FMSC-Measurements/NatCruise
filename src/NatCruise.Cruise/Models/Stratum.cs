using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using System.Linq;

namespace NatCruise.Cruise.Models
{
    [Table("Stratum")]
    public class Stratum
    {
        public string StratumCode { get; set; }

        public string Description { get; set; }

        public string Method { get; set; }

        public string Hotkey { get; set; }

        // TODO use CruiseMethods Look up table to get this value from the db?
        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);
    }
}