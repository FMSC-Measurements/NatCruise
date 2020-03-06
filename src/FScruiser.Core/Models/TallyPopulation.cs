namespace FScruiser.Models
{
    public class TallyPopulation : TallyPopulation_Base
    {
        private int _treeCount;
        private int _sumKPI;

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