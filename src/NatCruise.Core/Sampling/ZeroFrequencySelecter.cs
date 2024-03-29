﻿using FMSC.Sampling;
using System;

namespace NatCruise.Sampling
{
    public class ZeroFrequencySelecter : IFrequencyBasedSelecter
    {
        public ZeroFrequencySelecter(string stratumCode, string sampleGroupCode)
        {
            StratumCode = stratumCode;
            SampleGroupCode = sampleGroupCode;
        }

        public int Frequency
        {
            get => 0;
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
            return SampleResult.C;
        }
    }
}