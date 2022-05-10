using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Tree")]
    public class Tree : BindableBase
    {
        private const string DEFAULT_STM = "N";
        private const string DEFAULT_COUNT_MEASURE = "C";
        private const string DEFAULT_LIVE_DEAD = "L";

        private string _stratumCode;
        private string _sampleGroupCode;
        private int _treeNumber;
        private string _species;
        private string _countOrMeasure = DEFAULT_COUNT_MEASURE;
        private string _liveDead = DEFAULT_LIVE_DEAD;
        private int? _plotNumber;
        private string _unitCode;

        public string TreeID { get; set; }

        public string CuttingUnitCode
        {
            get { return _unitCode; }
            set { SetProperty(ref _unitCode, value); }
        }

        public string StratumCode
        {
            get { return _stratumCode; }
            set { SetProperty(ref _stratumCode, value); }
        }

        public string SampleGroupCode
        {
            get { return _sampleGroupCode; }
            set { SetProperty(ref _sampleGroupCode, value); }
        }

        public int? PlotNumber
        {
            get { return _plotNumber; }
            set { SetProperty(ref _plotNumber, value); }
        }

        public int TreeNumber
        {
            get { return _treeNumber; }
            set { SetProperty(ref _treeNumber, value); }
        }

        public string SpeciesCode
        {
            get { return _species; }
            set { SetProperty(ref _species, value); }
        }

        public string CountOrMeasure
        {
            get { return _countOrMeasure; }
            set { SetProperty(ref _countOrMeasure, value); }
        }

        public string LiveDead
        {
            get { return _liveDead; }
            set { SetProperty(ref _liveDead, value); }
        }
    }
}