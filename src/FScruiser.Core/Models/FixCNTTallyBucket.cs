using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class FixCNTTallyBucket : INPC_Base
    {
        private int _treeCount;

        public FixCNTTallyBucket(FixCntTallyPopulation tallyPopulation, double value, int treeCount)
        {
            TallyPopulation = tallyPopulation ?? throw new ArgumentNullException(nameof(tallyPopulation));
            Value = value;
            TreeCount = treeCount;
        }

        public FixCntTallyPopulation TallyPopulation { get; set; }

        public double Value { get; set; }

        public int TreeCount
        {
            get => _treeCount;
            set => SetValue(ref _treeCount, value);
        }
    }
}
