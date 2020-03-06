using FMSC.Sampling;

namespace FScruiser.Logic
{
    public class HundredPCTSelector : IFrequencyBasedSelecter
    {
        public int Frequency
        {
            get => 1;
        }

        public int Count { get; protected set; }

        public int ITreeFrequency => 0;

        public bool IsSelectingITrees => false;

        public int InsuranceCounter => 0;

        public int InsuranceIndex => 0;

        public string StratumCode { get; set; }
        public string SampleGroupCode { get; set; }

        public SampleResult Sample()
        {
            Count++;
            return SampleResult.M;
        }
    }
}