using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;

namespace NatCruise.Cruise.Data
{
    public class PlotTallyDataservice : CruiseDataserviceBase, IPlotTallyDataservice
    {
        public ISampleInfoDataservice SampleInfoDataservice { get; }

        public PlotTallyDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID, ISampleInfoDataservice sampleInfoDataservice) : base(database, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
        }

        public PlotTallyDataservice(string path, string cruiseID, string deviceID, ISampleInfoDataservice sampleInfoDataservice) : base(path, cruiseID, deviceID)
        {
            SampleInfoDataservice = sampleInfoDataservice ?? throw new ArgumentNullException(nameof(sampleInfoDataservice));
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
    }
}