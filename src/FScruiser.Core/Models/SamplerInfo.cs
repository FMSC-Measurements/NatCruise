namespace FScruiser.Models
{
    public class SamplerInfo
    {
        public string StratumCode {get; set;}
        public string SampleGroupCode { get; set; }

        public string Method { get; set; }
        //public bool UseExternalSampler { get; set; }
        public int SamplingFrequency { get; set; }
        public int InsuranceFrequency { get; set; }
        public int KZ { get; set; }
    }
} 