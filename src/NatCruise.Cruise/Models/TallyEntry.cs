using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;

namespace NatCruise.Cruise.Models
{
    [Table("TallyLedger")]
    public class TallyEntry : BindableBase, IHasTreeID
    {
        private int _warningCount;
        private int _errorCount;

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
            LiveDead = action.LiveDead;
            EntryType = action.EntryType;
            CountOrMeasure = action.SampleResult.ToString();
        }

        public string CuttingUnitCode { get; set; }

        public int? PlotNumber { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

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

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public string EntryType { get; set; }

        public int TreeCount { get; set; }

        public int KPI { get; set; }

        public string Reason { get; set; }

        public int? TreeNumber { get; set; }

        public string TreeID { get; set; }

        public string CountOrMeasure { get; set; }

        public string TallyLedgerID { get; set; }

        public override string ToString()
        {
            var treeNum = (TreeNumber != null) ? $"Tree#:{TreeNumber}" : "";

            return $"St:{StratumCode} Sg:{SampleGroupCode} Sp:{SpeciesCode} LD:{LiveDead} {treeNum}";
        }
    }
}