using NatCruise.Models;
using System;

namespace FScruiser.Models
{
    public class FixCNTTallyBucket : Model_Base
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