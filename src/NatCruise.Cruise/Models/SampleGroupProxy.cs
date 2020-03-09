using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("SampleGroup_V3")]
    public class SampleGroupProxy
    {
        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("StratumCode")]
        public string StratumCode { get; set; }

        public override string ToString()
        {
            return SampleGroupCode;
        }
    }
}