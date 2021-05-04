namespace NatCruise.Cruise.Models
{
    public class SamplerInfo
    {
        public string CruiseID { get; set; }
        public string StratumCode { get; set; }
        public string SampleGroupCode { get; set; }
        public string Method { get; set; }
        public int SamplingFrequency { get; set; }
        public int InsuranceFrequency { get; set; }
        public string SampleSelectorType { get; set; }
        public int KZ { get; set; }
    }
}