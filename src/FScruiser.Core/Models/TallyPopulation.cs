using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.Sampling;
using FScruiser.Util;
using System.Linq;

namespace FScruiser.Models
{
    public class TallyPopulation : TallyPopulation_Base
    {
        int _treeCount;
        int _sumKPI;

        public int TreeCount
        {
            get { return _treeCount; }
            set { SetValue(ref _treeCount, value); }
        }

        public int SumKPI
        {
            get { return _sumKPI; }
            set { SetValue(ref _sumKPI, value); }
        }

        public bool IsClickerTally { get; set; }

        public bool UseExternalSampler { get; set; }

        public int Frequency { get; set; }
    }
}