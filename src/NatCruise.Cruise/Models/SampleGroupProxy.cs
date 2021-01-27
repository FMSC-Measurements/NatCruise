using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("SampleGroup")]
    public class SampleGroupProxy
    {
        public string SampleGroupCode { get; set; }
        public string StratumCode { get; set; }

        public override string ToString()
        {
            return SampleGroupCode;
        }
    }
}