using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("SampleGroup")]
    public class SampleGroup
    {
        public string SampleGroupCode { get; set; }
        public long SamplingFrequency { get; set; }
        public long InsuranceFrequency { get; set; }
        public long KZ { get; set; }
        public float BigBAF { get; set; }
        public float SmallFPS { get; set; }
        public string Description { get; set; }

        //[Field(Name = "SampleSelectorType")]
        //public string SampleSelectorType { get; set; }

        //[Field(Name = "SampleSelectorState")]
        //public string SampleSelectorState { get; set; }

        public long MinKPI { get; set; }
        public virtual long MaxKPI { get; set; }
        public string StratumCode { get; set; }
        public string Method { get; set; }
    }
}