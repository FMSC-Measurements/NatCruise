using FMSC.ORM.EntityModel.Attributes;
using System;

namespace NatCruise.Models
{
    [Table("TallyLedger")]
    public class TallyLedger 
    {
        public TallyLedger()
        { }

        public TallyLedger(string unitCode, TallyPopulation tallyPopulation)
        {
            if (string.IsNullOrEmpty(unitCode)) { throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty", nameof(unitCode)); }
            if (tallyPopulation is null) { throw new ArgumentNullException(nameof(tallyPopulation)); }

            CuttingUnitCode = unitCode;
            StratumCode = tallyPopulation.StratumCode;
            SampleGroupCode = tallyPopulation.SampleGroupCode;
            SpeciesCode = tallyPopulation.SpeciesCode;
            LiveDead = tallyPopulation.LiveDead;
        }

        public string TallyLedgerID { get; set; }

        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string PlotNumber { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public string TreeID { get; set; }

        [Field(SQLExpression = "Tree.TreeNumber", Alias = "TreeNumber", PersistanceFlags = PersistanceFlags.Never)]
        public int? TreeNumber { get; set; }

        public int TreeCount { get; set; }

        public int KPI { get; set; }

        public int ThreePRandomValue { get; set; }

        public string CreatedBy { get; set; }

        public string Reason { get; set; }

        public string Signature { get; set; }

        public string Remarks { get; set; }

        public string EntryType { get; set; }
    }
}