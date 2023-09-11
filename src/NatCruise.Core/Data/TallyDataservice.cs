using CruiseDAL;
using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class TallyDataservice : CruiseDataserviceBase, ITallyDataservice
    {
        public TallyDataservice(string path, string cruiseID, string deviceID, ISamplerStateDataservice sampleInfoDataservice)
            : base(path, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
        }

        public TallyDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID, ISamplerStateDataservice sampleInfoDataservice)
            : base(database, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
        }

        public ISamplerStateDataservice SampleInfoDataservice { get; }

        private const string QUERY_TALLYENTRY_BASE =
@"SELECT
        tl.TreeID,
        tl.TallyLedgerID,
        tl.CuttingUnitCode,
        tl.StratumCode,
        tl.SampleGroupCode,
        tl.SpeciesCode,
        tl.LiveDead,
        tl.TreeCount,
        tl.Reason,
        tl.KPI,
        tl.EntryType,
        tl.Remarks,
        tl.Signature,
        tl.Created_TS,
        t.TreeNumber,
        t.CountOrMeasure,
        tl.STM,
        (CASE WHEN tl.TreeID IS NULL THEN 0 ELSE
            (SELECT count(*) FROM TreeError AS te WHERE Level = 'E' AND te.TreeID = tl.TreeID AND IsResolved = 0)
        END) AS ErrorCount,
        (CASE WHEN tl.TreeID IS NULL THEN 0 ELSE
            (SELECT count(*) FROM TreeError AS te WHERE Level = 'W' AND te.TreeID = tl.TreeID AND IsResolved = 0)
        END) AS WarningCount
    FROM TallyLedger AS tl
    LEFT JOIN Tree AS t USING (TreeID) ";

        public TallyEntry GetTallyEntry(string tallyLedgerID)
        {
            if (string.IsNullOrEmpty(tallyLedgerID)) { throw new ArgumentException($"'{nameof(tallyLedgerID)}' cannot be null or empty", nameof(tallyLedgerID)); }

            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.TallyLedgerID = @p1;",
                new object[] { tallyLedgerID })
                .FirstOrDefault();
        }

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode)) { throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty", nameof(unitCode)); }

            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.CuttingUnitCode = @p1 AND tl.CruiseID = @p2 AND tl.PlotNumber IS NULL " +
                "ORDER BY tl.Created_TS DESC;",
                new object[] { unitCode, CruiseID })
                .ToArray();

            //From<TallyEntry>()
            ////.Where("UnitCode = @p1 AND PlotNumber IS NULL ")
            //.Where("UnitCode = @p1")
            //.OrderBy("TimeStamp DESC")
            //.Limit(NUMBER_OF_TALLY_ENTRIES_PERPAGE, 0 * NUMBER_OF_TALLY_ENTRIES_PERPAGE)
            //.Query(unitCode);
        }

        public IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber)
        {
            if (string.IsNullOrEmpty(unitCode)) { throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty", nameof(unitCode)); }

            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.CuttingUnitCode = @p1" +
                "AND tl.CruiseID = @p2 " +
                "AND tl.PolotNumber = @p3;",
                new object[] { unitCode, CruiseID, plotNumber })
                .ToArray();

            //return Database.From<TallyEntry>()
            //    .LeftJoin("Tree", "USING (Tree_GUID)")
            //    .Where("UnitCode = @p1 AND PlotNumber = @p2 ")
            //    .OrderBy("TimeStamp DESC")
            //    .Query(unitCode, plotNumber);
        }

        

        public Task<TallyEntry> InsertTallyActionAsync(TallyAction tallyAction)
        {
            return Task.Run(() => InsertTallyAction(tallyAction));
            //return Task.Factory.StartNew(() => InsertTallyAction(tallyAction));
        }

        public TallyEntry InsertTallyAction(TallyAction atn)
        {
            if (atn is null) { throw new ArgumentNullException(nameof(atn)); }

            if (atn.IsInsuranceSample == true && atn.IsSample == false) { throw new InvalidOperationException("If action is insurance sample it must be sample aswell"); }

            Database.BeginTransaction();
            var tallyEntry = new TallyEntry(atn);
            try
            {
               
                tallyEntry.TallyLedgerID = Guid.NewGuid().ToString();

                if (atn.IsSample)
                {
                    tallyEntry.TreeID = tallyEntry.TallyLedgerID;

                    tallyEntry.TreeNumber = Database.ExecuteScalar2<int>(
                        "SELECT " +
                        "ifnull(max(TreeNumber), 0) + 1 " +
                        "FROM Tree " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode AND CruiseID = @CruiseID " +
                        "AND ifnull(PlotNumber, -1) = ifnull(@PlotNumber, -1)",
                        new { CruiseID, atn.CuttingUnitCode, atn.PlotNumber });

                    //if (string.IsNullOrEmpty(tallyEntry.LiveDead))
                    //{
                    //    tallyEntry.LiveDead = Database.ExecuteScalar2<string>(
                    //        "SELECT DefaultLiveDead " +
                    //        "FROM SampleGroup " +
                    //        "WHERE CruiseID = @CruiseID " +
                    //        "AND StratumCode = @StratumCode " +
                    //        "AND SampleGroupCode = @SampleGroupCode;",
                    //        new { CruiseID, atn.StratumCode, atn.SampleGroupCode });
                    //}

                    Database.Execute2(
@"INSERT INTO Tree (
    TreeID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeNumber,
    CountOrMeasure,
    CreatedBy
) VALUES (
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @TreeNumber,
    @CountOrMeasure,
    @CreatedBy
);
INSERT INTO TreeMeasurment (
    TreeID,
    CreatedBy
) VALUES (
    @TreeID,
    @CreatedBy
);",
                        new
                        {
                            CruiseID,
                            tallyEntry.TreeID,
                            tallyEntry.TreeNumber,
                            atn.CuttingUnitCode,
                            atn.PlotNumber,
                            atn.StratumCode,
                            atn.SampleGroupCode,
                            atn.SpeciesCode,
                            atn.LiveDead,
                            tallyEntry.CountOrMeasure,
                            CreatedBy = DeviceID,
                        });
                }

                Database.Execute2(
@"INSERT INTO TallyLedger (
    TreeID,
    TallyLedgerID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    STM,
    ThreePRandomValue,
    EntryType,
    CreatedBy
) VALUES (
    @TreeID,
    @TallyLedgerID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @TreeCount,
    @KPI,
    @STM,
    @ThreePRandomValue,
    @EntryType,
    @CreatedBy
);",
                    new
                    {
                        CruiseID,
                        tallyEntry.TreeID,
                        tallyEntry.TallyLedgerID,
                        atn.CuttingUnitCode,
                        atn.PlotNumber,
                        atn.StratumCode,
                        atn.SampleGroupCode,
                        atn.SpeciesCode,
                        atn.LiveDead,
                        atn.TreeCount,
                        atn.KPI,
                        atn.STM,
                        atn.ThreePRandomValue,
                        atn.EntryType,
                        CreatedBy = DeviceID,
                    });

                var samplerState = atn.SamplerState;
                if (samplerState != null)
                {
                    SampleInfoDataservice.UpsertSamplerState(samplerState);
                }

                Database.CommitTransaction();
                
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }

            RefreshErrorsAndWarnings(tallyEntry);
            return tallyEntry;
        }

        public void DeleteTallyEntry(string tallyLedgerID)
        {
            Database.BeginTransaction();
            try
            {
                Database.Execute("DELETE FROM Tree WHERE TreeID IN (SELECT TreeID FROM TallyLedger WHERE TallyLedgerID = @p1);", tallyLedgerID);
                Database.Execute("DELETE FROM TallyLedger WHERE TallyLedgerID = @p1;", tallyLedgerID);

                Database.CommitTransaction();
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
        }

        public void RefreshErrorsAndWarnings(TallyEntry tallyEntry)
        {
            if (tallyEntry is null) { throw new ArgumentNullException(nameof(tallyEntry)); }

            tallyEntry.ErrorCount = Database.ExecuteScalar<int>("SELECT count(*) FROM TreeError WHERE Level = 'E' AND IsResolved = 0 AND TreeID = @p1;", tallyEntry.TreeID);
            tallyEntry.WarningCount = Database.ExecuteScalar<int>("SELECT count(*) FROM TreeError WHERE Level = 'W' AND IsResolved = 0 AND TreeID = @p1;", tallyEntry.TreeID);
        }
    }
}