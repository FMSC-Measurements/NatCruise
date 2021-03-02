using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("Stratum")]
    public class StratumProxy
    {
        //[Field(Name = "Stratum_CN")]
        //public string Stratum_CN { get; set; }

        public string StratumCode { get; set; }

        public string Method { get; set; }

        public override string ToString()
        {
            return StratumCode;
        }
    }
}