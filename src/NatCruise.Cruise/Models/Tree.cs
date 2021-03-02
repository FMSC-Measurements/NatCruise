using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    [Table("Tree")]
    public class Tree : Model_Base, IHasTreeID
    {
        #region table fields

        public string TreeID { get; set; }

        public string CuttingUnitCode
        {
            get { return _unitCode; }
            set { SetValue(ref _unitCode, value); }
        }

        public string StratumCode
        {
            get { return _stratumCode; }
            set { SetValue(ref _stratumCode, value); }
        }

        public string SampleGroupCode
        {
            get { return _sampleGroupCode; }
            set { SetValue(ref _sampleGroupCode, value); }
        }

        public int? PlotNumber
        {
            get { return _plotNumber; }
            set { SetValue(ref _plotNumber, value); }
        }

        public int TreeNumber
        {
            get { return _treeNumber; }
            set { SetValue(ref _treeNumber, value); }
        }

        public string SpeciesCode
        {
            get { return _species; }
            set { SetValue(ref _species, value); }
        }

        public string CountOrMeasure
        {
            get { return _countOrMeasure; }
            set { SetValue(ref _countOrMeasure, value); }
        }

        public string LiveDead
        {
            get { return _liveDead; }
            set { SetValue(ref _liveDead, value); }
        }

        #endregion table fields

        private string _stratumCode;
        private string _sampleGroupCode;
        private int _treeNumber;
        private string _species;
        private string _countOrMeasure = DEFAULT_COUNT_MEASURE;

        private string _liveDead = DEFAULT_LIVE_DEAD;

        private int? _plotNumber;
        private string _unitCode;
        private const string DEFAULT_STM = "N";
        private const string DEFAULT_COUNT_MEASURE = "C";
        private const string DEFAULT_LIVE_DEAD = "L";
    }
}