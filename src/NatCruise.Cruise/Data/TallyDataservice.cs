using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;

namespace NatCruise.Cruise.Data
{
    public class TallyDataservice : SamplerInfoDataservice, ITallyDataservice
    {

        public TallyDataservice(string path, string cruiseID, IDeviceInfoService deviceInfo)
            : base(path, cruiseID, deviceInfo)
        {
        }

        public TallyDataservice(CruiseDatastore_V3 database, string cruiseID, IDeviceInfoService deviceInfo)
            : base(database, cruiseID, deviceInfo)
        {
        }

        const string QUERY_TALLYENTRY_BASE =
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
            (SELECT count(*) FROM TreeError AS te WHERE Level = 'E' AND te.TreeID = tl.TreeID AND Resolution IS NULL)
        END) AS ErrorCount,
        (CASE WHEN tl.TreeID IS NULL THEN 0 ELSE
            (SELECT count(*) FROM TreeError AS te WHERE Level = 'W' AND te.TreeID = tl.TreeID AND Resolution IS NULL)
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
            try
            {
                var tallyEntry = new TallyEntry(atn);

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
    TreeID
) VALUES (
    @TreeID
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
                            CreatedBy = DeviceInfo.DeviceID,
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
                        CreatedBy = DeviceInfo.DeviceID,
                    });

                var samplerState = atn.SamplerState;
                if (samplerState != null)
                {
                    UpsertSamplerState(atn.SamplerState);
                }

                Database.CommitTransaction();

                return tallyEntry;
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
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
    }
}
