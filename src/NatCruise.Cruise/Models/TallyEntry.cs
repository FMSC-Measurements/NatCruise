using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;

namespace NatCruise.Cruise.Models
{
    [Table("TallyLedger")]
    public class TallyEntry : BindableBase
    {
        private int _warningCount;
        private int _errorCount;
        private string _stratumCode;
        private string _sampleGroupCode;
        private string _speciesCode;
        private string _liveDead;
        private string _countOrMeasure;
        private int? _treeNumber;

        public TallyEntry()
        { }

        public TallyEntry(TallyAction action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            CuttingUnitCode = action.CuttingUnitCode;
            PlotNumber = action.PlotNumber;
            StratumCode = action.StratumCode;
            SampleGroupCode = action.SampleGroupCode;
            SpeciesCode = action.SpeciesCode;
            TreeCount = action.TreeCount;
            KPI = action.KPI;
            STM = action.STM;
            LiveDead = action.LiveDead;
            EntryType = action.EntryType;
            CountOrMeasure = action.SampleResult.ToString();
        }

        // non changing fields
        public string TallyLedgerID { get; set; }
        public string TreeID { get; set; }
        
        public string CuttingUnitCode { get; set; }
        public int? PlotNumber { get; set; }
        public string EntryType { get; set; }
        public int TreeCount { get; set; }
        public int KPI { get; set; }
        public string Reason { get; set; }
        public bool STM { get; set; }

        // fields that can get changed when entry gets reloaded
        // because we are refreshing TallyEntry records inplace
        // we need to implement INPC on fields that can be updated
        // this may change if we find a better way of doing reloads
        // on the Tally page
        public int? TreeNumber
        {
            get => _treeNumber;
            set => SetProperty(ref _treeNumber, value);
        }

        public string StratumCode
        {
            get => _stratumCode;
            set => SetProperty(ref _stratumCode, value);
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set => SetProperty(ref _sampleGroupCode, value);
        }

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

        public string CountOrMeasure
        {
            get => _countOrMeasure;
            set => SetProperty(ref _countOrMeasure, value);
        }

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

        public override string ToString()
        {
            var treeNum = (TreeNumber != null) ? $"Tree#:{TreeNumber}" : "";

            return $"St:{StratumCode} Sg:{SampleGroupCode} Sp:{SpeciesCode} LD:{LiveDead} {treeNum}";
        }
    }
}