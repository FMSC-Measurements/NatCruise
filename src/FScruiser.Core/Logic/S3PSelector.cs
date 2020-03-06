using FMSC.Sampling;
using System;

namespace FScruiser.Logic
{
    public class S3PSelector : ISampleSelector
    {
        protected BlockSelecter BlockSelecter { get; }

        protected ThreePSelecter ThreePSelecter { get; }

        public string BlockState => BlockSelecter.BlockState;

        public int Count => BlockSelecter.Count;

        public int ITreeFrequency => 0;

        public bool IsSelectingITrees => false;

        public int InsuranceCounter => BlockSelecter.InsuranceCounter;

        public int InsuranceIndex => BlockSelecter.InsuranceIndex;

        public string StratumCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SampleGroupCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public S3PSelector(int freq, int kz)
        {
            BlockSelecter = new BlockSelecter(freq, 0);
            ThreePSelecter = new ThreePSelecter(kz, 0);
        }

        public S3PSelector(int freq, int kz, int count, string blockState)
        {
            BlockSelecter = new BlockSelecter(freq, 0, blockState, count, 0, 0);
            ThreePSelecter = new ThreePSelecter(kz, 0);
        }

        public SampleResult Sample()
        {
            return BlockSelecter.Sample();
        }

        public SampleResult Sample(int kpi, out int rand)
        {
            return ThreePSelecter.Sample(kpi, out rand);
        }
    }
}
