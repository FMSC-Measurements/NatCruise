using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using System;

namespace FScruiser.Models
{
    [Table("TallyLedger")]
    public class TallyLedger : IHasTreeID
    {
        public class EntryTypeValues
        {
            public const string TALLY = "tally";
            public const string UTILITY = "utility";
            public const string TREECOUNT_EDIT = "treecount_edit";
            public const string CLICKER = "clicker";
        }

        public TallyLedger(string unitCode, TallyPopulation tallyPopulation)
        {
            CuttingUnitCode = unitCode;
            StratumCode = tallyPopulation.StratumCode;
            SampleGroupCode = tallyPopulation.SampleGroupCode;
            Species = tallyPopulation.Species;
            LiveDead = tallyPopulation.LiveDead;
        }

        [Field(nameof(TallyLedgerID))]
        public string TallyLedgerID { get; set; }

        [Field(nameof(CuttingUnitCode))]
        public string CuttingUnitCode { get; set; }

        [Field(nameof(StratumCode))]
        public string StratumCode { get; set; }

        [Field(nameof(SampleGroupCode))]
        public string SampleGroupCode { get; set; }

        [Field(nameof(PlotNumber))]
        public string PlotNumber { get; set; }

        [Field(nameof(Species))]
        public string Species { get; set; }

        [Field(nameof(LiveDead))]
        public string LiveDead { get; set; }

        [Field(nameof(TreeID))]
        public string TreeID { get; set; }

        [Field(nameof(TreeCount))]
        public int TreeCount { get; set; }

        [Field(nameof(KPI))]
        public int KPI { get; set; }

        [Field(nameof(ThreePRandomValue))]
        public int ThreePRandomValue { get; set; }

        [Field(nameof(CreatedBy))]
        public DateTime CreatedBy { get; set; }

        [Field(nameof(Reason))]
        public string Reason { get; set; }

        [Field(nameof(Signature))]
        public string Signature { get; set; }

        [Field(nameof(Remarks))]
        public string Remarks { get; set; }

        [Field(nameof(EntryType))]
        public string EntryType { get; set; }
    }
}