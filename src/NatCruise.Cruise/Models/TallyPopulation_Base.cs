using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;
using System.Linq;

namespace NatCruise.Cruise.Models
{
    public class TallyPopulation_Base : Model_Base
    {
        [Field("Description")]
        public string TallyDescription { get; set; }

        [Field("HotKey")]
        public string TallyHotKey { get; set; }

        public string StratumCode { get; set; }

        [Field("StratumMethod")]
        public string Method { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        public string SampleGroupCode { get; set; }

        [Field("sgMinKPI")]
        public int MinKPI { get; set; }

        [Field("sgMaxKPI")]
        public int MaxKPI { get; set; }

        public string SpeciesCode { get; set; }
        public string LiveDead { get; set; }

        public override string ToString()
        {
            return $"St:{StratumCode} Sg:{SampleGroupCode} Sp:{SpeciesCode} LD:{LiveDead}";
        }
    }
}