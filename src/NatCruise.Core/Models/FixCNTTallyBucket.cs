using NatCruise.Models;
using Prism.Mvvm;
using System;

namespace NatCruise.Models
{
    public class FixCNTTallyBucket : BindableBase
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
            set => SetProperty(ref _treeCount, value);
        }
    }
}