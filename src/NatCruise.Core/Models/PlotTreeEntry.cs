using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Tree")]
    public class PlotTreeEntry : BindableBase
    {
        private string _speciesCode;
        private string _liveDead;
        private int _errorCount;
        private int _warningCount;
        private int _treeCount;
        private string _countOrMeasure;

        public string TreeID { get; set; }

        public int TreeNumber { get; set; }

        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode
        {
            get => _speciesCode;
            set => SetProperty(ref _speciesCode, value);
        }

        public string LiveDead
        {
            get => _liveDead;
            set => SetProperty(ref _liveDead, value);
        }

        public int TreeCount
        {
            get => _treeCount;
            set => SetProperty(ref _treeCount, value);
        }

        public int PlotNumber { get; set; }

        public string CountOrMeasure
        {
            get => _countOrMeasure;
            set => SetProperty(ref _countOrMeasure, value);
        }

        // used because we want to display TreeCount for just FixCNT trees
        public string Method { get; set; }

        public bool STM { get; set; }

        public int KPI { get; set; }

        public int ThreePRandomValue { get; set; }

        public string Initials { get; set; }

        public int ErrorCount
        {
            get => _errorCount;
            set => SetProperty(ref _errorCount, value);
        }

        public int WarningCount
        {
            get => _warningCount;
            set => SetProperty(ref _warningCount, value);
        }
    }
}