﻿using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    // TODO can this be consolidated into TallyPopulation or broken into a separate model
    public class TallyPopulationEx : TallyPopulation
    {
        private int _treeCount;
        private int _sumKPI;

        public int TreeCount
        {
            get { return _treeCount; }
            set { SetProperty(ref _treeCount, value); }
        }

        public int SumKPI
        {
            get { return _sumKPI; }
            set { SetProperty(ref _sumKPI, value); }
        }

        public bool IsClickerTally => SampleSelectorType == CruiseMethods.CLICKER_SAMPLER_TYPE;

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string SampleSelectorType { get; set; }

        // depreciated in favor of using SampleSelectorType with "ClickerSelecter"
        // IsClickerTally will reflect this
        //public bool UseExternalSampler { get; set; }

        public int Frequency { get; set; }

        public int InsuranceFrequency { get; set; }

        public int KZ { get; set; }
    }
}