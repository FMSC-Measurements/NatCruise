using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using FScruiser.Models;
using FScruiser.Services;

namespace FScruiser.Data
{
    public class TallyDataservice : SamplerInfoDataservice, ITallyDataservice
    {

        public TallyDataservice(string path, IDeviceInfoService deviceInfo)
            : base(path, deviceInfo)
        {
        }

        public TallyDataservice(CruiseDatastore_V3 database, IDeviceInfoService deviceInfo)
            : base(database, deviceInfo)
        {
        }

        const string QUERY_TALLYENTRY_BASE =
            "SELECT " +
                    "tl.TreeID, " +
                    "tl.TallyLedgerID, " +
                    "tl.CuttingUnitCode, " +
                    "tl.StratumCode, " +
                    "tl.SampleGroupCode, " +
                    "tl.Species, " +
                    "tl.LiveDead, " +
                    "tl.TreeCount, " +
                    "tl.Reason, " +
                    "tl.KPI, " +
                    "tl.EntryType, " +
                    "tl.Remarks, " +
                    "tl.Signature, " +
                    "tl.CreatedDate, " +
                    "t.TreeNumber, " +
                    "t.CountOrMeasure, " +
                    "tl.STM, " +
                    "(SELECT count(*) FROM TreeError AS te WHERE tl.TreeID IS NOT NULL AND Level = 'E' AND te.TreeID = tl.TreeID AND Resolution IS NULL) AS ErrorCount, " +
                    "(SELECT count(*) FROM TreeError AS te WHERE tl.TreeID IS NOT NULL AND Level = 'W' AND te.TreeID = tl.TreeID AND Resolution IS NULL) AS WarningCount " +
                "FROM TallyLedger AS tl " +
                "LEFT JOIN Tree_V3 AS t USING (TreeID) ";

        public TallyEntry GetTallyEntry(string tallyLedgerID)
        {
            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.TallyLedgerID = @p1;",
                new object[] { tallyLedgerID })
                .FirstOrDefault();
        }

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.CuttingUnitCode = @p1 AND tl.PlotNumber IS NULL " +
                "ORDER BY tl.CreatedDate DESC;",
                new object[] { unitCode })
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
            return Database.Query<TallyEntry>(
                QUERY_TALLYENTRY_BASE +
                "WHERE tl.CuttingUnitCode = @p1" +
                "AND tl.PolotNumber = @p2;",
                new object[] { unitCode, plotNumber })
                .ToArray();

            //return Database.From<TallyEntry>()
            //    .LeftJoin("Tree", "USING (Tree_GUID)")
            //    .Where("UnitCode = @p1 AND PlotNumber = @p2 ")
            //    .OrderBy("TimeStamp DESC")
            //    .Query(unitCode, plotNumber);
        }

        public void InsertTallyLedger(TallyLedger tallyLedger)
        {
            var tallyLedgerID = tallyLedger.TallyLedgerID ?? Guid.NewGuid().ToString();

            Database.Execute2(
                "INSERT INTO TallyLedger (" +
                    "TallyLedgerID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "PlotNumber, " +
                    "Species, " +
                    "LiveDead," +
                    "TreeCount, " +
                    "KPI, " +
                    "ThreePRandomValue, " +
                    "TreeID, " +
                    "CreatedBy, " +
                    "Reason, " +
                    "Signature, " +
                    "Remarks, " +
                    "EntryType" +
                ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@PlotNumber, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@ThreePRandomValue, " +
                    "@TreeID, " +
                    "@CreatedBy, " +
                    "@Reason, " +
                    "@Signature, " +
                    "@Remarks, " +
                    "@EntryType" +
                ");",
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    tallyLedger.CuttingUnitCode,
                    tallyLedger.StratumCode,
                    tallyLedger.SampleGroupCode,
                    tallyLedger.PlotNumber,
                    tallyLedger.Species,
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
            return Task.Factory.StartNew(() => InsertTallyAction(tallyAction));
        }

        public TallyEntry InsertTallyAction(TallyAction atn)
        {
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
                        "FROM Tree_V3 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode " +
                        "AND ifnull(PlotNumber, -1) = ifnull(@PlotNumber, -1)",
                        new { atn.CuttingUnitCode, atn.PlotNumber });

                    Database.Execute2(
                        "INSERT INTO Tree_V3 ( " +
                            "TreeID, " +
                            "CuttingUnitCode, " +
                            "PlotNumber, " +
                            "StratumCode, " +
                            "SampleGroupCode, " +
                            "Species, " +
                            "LiveDead, " +
                            "TreeNumber, " +
                            "CountOrMeasure, " +
                            "CreatedBy " +
                        ") VALUES ( " +
                            "@TreeID, " +
                            "@CuttingUnitCode, " +
                            "@PlotNumber, " +
                            "@StratumCode, " +
                            "@SampleGroupCode, " +
                            "@Species, " +
                            "@LiveDead, " +
                            "@TreeNumber," +
                            "@CountOrMeasure," +
                            "@CreatedBy " +
                        ");" +
                        "INSERT INTO TreeMeasurment (" +
                            "TreeID" +
                        ") VALUES ( " +
                            "@TreeID" +
                        ");",
                        new
                        {
                            tallyEntry.TreeID,
                            tallyEntry.TreeNumber,
                            atn.CuttingUnitCode,
                            atn.PlotNumber,
                            atn.StratumCode,
                            atn.SampleGroupCode,
                            atn.Species,
                            atn.LiveDead,
                            tallyEntry.CountOrMeasure,
                            CreatedBy = DeviceInfo.DeviceID,
                        });
                }

                Database.Execute2(
                    "INSERT INTO TallyLedger ( " +
                        "TreeID, " +
                        "TallyLedgerID, " +
                        "CuttingUnitCode, " +
                        "PlotNumber, " +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead, " +
                        "TreeCount, " +
                        "KPI, " +
                        "STM, " +
                        "ThreePRandomValue, " +
                        "EntryType, " +
                        "CreatedBy" +
                    ") VALUES ( " +
                        "@TreeID, " +
                        "@TallyLedgerID, " +
                        "@CuttingUnitCode, " +
                        "@PlotNumber, " +
                        "@StratumCode, " +
                        "@SampleGroupCode, " +
                        "@Species, " +
                        "@LiveDead, " +
                        "@TreeCount, " +
                        "@KPI, " +
                        "@STM, " +
                        "@ThreePRandomValue," +
                        "@EntryType," +
                        "@CreatedBy" +
                    ");",
                    new
                    {
                        tallyEntry.TreeID,
                        tallyEntry.TallyLedgerID,
                        atn.CuttingUnitCode,
                        atn.PlotNumber,
                        atn.StratumCode,
                        atn.SampleGroupCode,
                        atn.Species,
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
                Database.Execute("DELETE FROM TREE_V3 WHERE TreeID IN (SELECT TreeID FROM TallyLedger WHERE TallyLedgerID = @p1);", tallyLedgerID);
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
