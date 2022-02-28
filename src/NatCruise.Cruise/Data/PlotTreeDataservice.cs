using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;
using System.Collections.Generic;

namespace NatCruise.Cruise.Data
{
    public class PlotTreeDataservice : CruiseDataserviceBase, IPlotTreeDataservice
    {
        public ISampleInfoDataservice SampleInfoDataservice { get; }

        public PlotTreeDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID, ISampleInfoDataservice sampleInfoDataservice) : base(database, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
        }

        public PlotTreeDataservice(string path, string cruiseID, string deviceID, ISampleInfoDataservice sampleInfoDataservice) : base(path, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
        }

        public IEnumerable<PlotTreeEntry> GetPlotTrees(string unitCode, int plotNumber)
        {
            return Database.Query<PlotTreeEntry>(
@"SELECT
    t.TreeID,
    t.CuttingUnitCode,
    t.TreeNumber,
    t.PlotNumber,
    t.StratumCode,
    t.SampleGroupCode,
    t.SpeciesCode,
    t.LiveDead,
    tl.TreeCount,
    tl.STM,
    tl.KPI,
    max(tm.TotalHeight, tm.MerchHeightPrimary, tm.UpperStemHeight) AS Height,
    max(tm.DBH, tm.DRC, tm.DBHDoubleBarkThickness) AS Diameter,
    t.CountOrMeasure,
    st.Method,
    (SELECT count(*) FROM TreeError AS te WHERE Level = 'E' AND te.TreeID = tl.TreeID AND IsResolved = 0) AS ErrorCount,
    (SELECT count(*) FROM TreeError AS te WHERE Level = 'W' AND te.TreeID = tl.TreeID AND IsResolved = 0) AS WarningCount
FROM Tree AS t
JOIN Stratum AS st USING (StratumCode, CruiseID)
LEFT JOIN TallyLedger_Tree_Totals AS tl USING (TreeID)
LEFT JOIN TreeMeasurment AS tm USING (TreeID)
WHERE t.CuttingUnitCode = @p1
AND t.CruiseID = @p2
AND t.PlotNumber = @p3
GROUP BY tl.TreeID
ORDER BY t.TreeNumber
;",
                new object[] { unitCode, CruiseID, plotNumber });
        }

        public void InsertTree(PlotTreeEntry tree, SamplerState samplerState)
        {
            if (tree is null) { throw new ArgumentNullException(nameof(tree)); }

            var database = Database;
            database.BeginTransaction();
            try
            {
                var treeID = tree.TreeID ?? Guid.NewGuid().ToString();

                var nextTreeNumber = GetNextPlotTreeNumber(tree.CuttingUnitCode, tree.StratumCode, tree.PlotNumber);
                tree.TreeNumber = nextTreeNumber;

                database.Execute2(
    @"INSERT INTO Tree (
    CruiseID,
    TreeID,
    TreeNumber,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    CountOrMeasure
) VALUES (
    @CruiseID,
    @TreeID,
    @TreeNumber,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @CountOrMeasure
);
INSERT INTO TallyLedger (
    CruiseID,
    TallyLedgerID,
    TreeID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    ThreePRandomValue,
    STM
) VALUES (
    @CruiseID,
    @TreeID,
    @TreeID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @TreeCount,
    @KPI,
    @ThreePRandomValue,
    @STM
); "
                    , new
                    {
                        CruiseID,
                        TreeID = treeID,
                        tree.TreeNumber,
                        tree.CuttingUnitCode,
                        tree.PlotNumber,
                        tree.StratumCode,
                        tree.SampleGroupCode,
                        tree.SpeciesCode,
                        tree.LiveDead,
                        tree.CountOrMeasure,
                        tree.TreeCount,
                        tree.KPI,
                        tree.ThreePRandomValue,
                        tree.STM,
                    });

                tree.TreeID = treeID;

                if (samplerState != null)
                {
                    SampleInfoDataservice.UpsertSamplerState(samplerState);
                }
                database.CommitTransaction();
            }
            catch
            {
                database.RollbackTransaction();
                throw;
            }
        }

        public string CreatePlotTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode,
            string species = null,
            string liveDead = "L",
            string countMeasure = "M",
            int treeCount = 1,
            int kpi = 0,
            bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreatePlotTree(tree_guid, unitCode, plotNumber, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
        }

        protected void CreatePlotTree(string treeID, string unitCode, int plotNumber,
            string stratumCode, string sampleGroupCode,
            string species = null, string liveDead = null,
            string countMeasure = "M", int treeCount = 1,
            int kpi = 0, bool stm = false)
        {
            var tallyLedgerID = treeID;

            if(string.IsNullOrEmpty(liveDead))
            {
                liveDead = Database.ExecuteScalar<string>(
                    "SELECT DefaultLiveDead " +
                    "FROM SampleGroup " +
                    "WHERE CruiseID = @p1 " +
                    "AND StratumCode = @p2 " +
                    "AND SampleGroupCode = @p3;", CruiseID, stratumCode, sampleGroupCode);
            }

            Database.Execute2(
$@"INSERT INTO Tree (
    CruiseID,
    TreeID,
    TreeNumber,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    CountOrMeasure
) VALUES (
    @CruiseID,
    @TreeID,
    (SELECT ifnull(max(TreeNumber), 0) + 1 FROM Tree WHERE CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber),
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @CountOrMeasure);
INSERT INTO TallyLedger (
    CruiseID,
    TallyLedgerID,
    TreeID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    STM
) VALUES (
    @CruiseID,
    @TallyLedgerID,
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @TreeCount,
    @KPI,
    @STM
);"
                , new
                {
                    CruiseID,
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    SpeciesCode = species,
                    LiveDead = liveDead,
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = stm,
                }
            );
        }

        public int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber)
        {
            var database = Database;
            var useCrossStrataTreeNumbering = database.ExecuteScalar<bool>("SELECT UseCrossStrataPlotTreeNumbering FROM Cruise WHERE CruiseID = @p1;", CruiseID);

            if (useCrossStrataTreeNumbering)
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree " +
                    "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber = @p3;"
                    , unitCode, CruiseID, plotNumber);
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree " +
                    "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber = @p3 AND StratumCode = @p4;"
                    , unitCode, CruiseID, plotNumber, stratumCode);
            }
        }

        public void RefreshErrorsAndWarnings(PlotTreeEntry treeEntry)
        {
            if (treeEntry is null) { throw new ArgumentNullException(nameof(treeEntry)); }

            treeEntry.ErrorCount = Database.ExecuteScalar<int>("SELECT count(*) FROM TreeError WHERE Level = 'E' AND IsResolved = 0 AND TreeID = @p1;", treeEntry.TreeID);
            treeEntry.WarningCount = Database.ExecuteScalar<int>("SELECT count(*) FROM TreeError WHERE Level = 'W' AND IsResolved = 0 AND TreeID = @p1;", treeEntry.TreeID);
        }
    }
}