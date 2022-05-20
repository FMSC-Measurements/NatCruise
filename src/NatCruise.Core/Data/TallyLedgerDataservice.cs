using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TallyLedgerDataservice : CruiseDataserviceBase, ITallyLedgerDataservice
    {
        public TallyLedgerDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TallyLedgerDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void InsertTallyLedger(TallyLedger tallyLedger)
        {
            if (tallyLedger is null) { throw new ArgumentNullException(nameof(tallyLedger)); }

            var tallyLedgerID = tallyLedger.TallyLedgerID ?? Guid.NewGuid().ToString();

            Database.Execute2(
@"INSERT INTO TallyLedger (
    TallyLedgerID,
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    PlotNumber,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    ThreePRandomValue,
    TreeID,
    CreatedBy,
    Reason,
    Signature,
    Remarks,
    EntryType
) VALUES (
    @TallyLedgerID,
    @CruiseID,
    @CuttingUnitCode,
    @StratumCode,
    @SampleGroupCode,
    @PlotNumber,
    @SpeciesCode,
    @LiveDead,
    @TreeCount,
    @KPI,
    @ThreePRandomValue,
    @TreeID,
    @CreatedBy,
    @Reason,
    @Signature,
    @Remarks,
    @EntryType
);",
                new
                {
                    CruiseID,
                    TallyLedgerID = tallyLedgerID,
                    tallyLedger.CuttingUnitCode,
                    tallyLedger.StratumCode,
                    tallyLedger.SampleGroupCode,
                    tallyLedger.PlotNumber,
                    tallyLedger.SpeciesCode,
                    tallyLedger.LiveDead,
                    tallyLedger.TreeCount,
                    tallyLedger.KPI,
                    tallyLedger.ThreePRandomValue,
                    tallyLedger.TreeID,
                    tallyLedger.CreatedBy,
                    tallyLedger.Reason,
                    tallyLedger.Signature,
                    tallyLedger.Remarks,
                    tallyLedger.EntryType,
                });

            tallyLedger.TallyLedgerID = tallyLedgerID;
        }

        public IEnumerable<TallyLedger> GetTallyLedgers(string cuttingUnitCode = null, string stratumCode = null, string sampleGroupCode = null, string speciesCode = null, string liveDead = null)
        {
            return Database.From<TallyLedger>()
                .Where("(@CuttingUnitCode IS NULL OR CuttingUnitCode = @CuttingUnitCode) " +
                "AND (@StratumCode IS NULL OR StratumCode = @StratumCode) " +
                "AND (@SampleGroupCode IS NULL OR SampleGroupCode = @SampleGroupCode) " +
                "AND (@SpeciesCode IS NULL OR SpeciesCode = @SpeciesCode) " +
                "AND (@LiveDead IS NULL OR LiveDead = @LiveDead)")
                .Query2(
                    new {
                        CuttingUnitCode = cuttingUnitCode,
                        StratumCode = stratumCode,
                        SampleGroupCode = sampleGroupCode,
                        SpeciesCode = speciesCode,
                        LiveDead = liveDead
                    }).ToArray();

        }
    }
}