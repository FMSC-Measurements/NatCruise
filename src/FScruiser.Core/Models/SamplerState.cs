using FMSC.Sampling;
using FScruiser.Logic;

namespace FScruiser.Models
{
    public class SamplerState
    {
        public SamplerState()
        {
        }

        public SamplerState(ISampleSelector sampler)
        {
            SampleSelectorType = sampler.GetType().Name;
            StratumCode = sampler.StratumCode;
            SampleGroupCode = sampler.SampleGroupCode;
            Counter = sampler.Count;
            InsuranceCounter = sampler.InsuranceCounter;
            InsuranceIndex = sampler.InsuranceIndex;

            switch (sampler)
            {
                case SystematicSelecter s:
                    {
                        SystematicIndex = s.HitIndex;
                        break;
                    }
                case BlockSelecter b:
                    {
                        BlockState = b.BlockState;
                        break;
                    }
                case ThreePSelecter s:
                    {
                        break;
                    }
                case S3PSelector s:
                    {
                        BlockState = s.BlockState;
                        break;
                    }
            }
        }


        public string StratumCode { get; set; }
        public string SampleGroupCode { get; set; }

        public string SampleSelectorType { get; set; }
        public string BlockState { get; set; }
        public int SystematicIndex { get; set; }
        public int Counter { get; set; }
        public int InsuranceIndex { get; set; }
        public int InsuranceCounter { get; set; }
    }
}