using CruiseDAL.Schema;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.Services
{
    public partial class CuttingUnitDatastore : IPlotDatastore
    {
        private string SELECT_TALLYPOPULATION_CORE =
    "WITH tallyPopTreeCounts AS (" +
        "SELECT CuttingUnitCode, " +
            "StratumCode, " +
            "SampleGroupCode, " +
            "Species, " +
            "LiveDead, " +
            "sum(TreeCount) AS TreeCount, " +
            "sum(KPI) AS SumKPI " +
        "FROM TallyLedger AS tl " +
        "GROUP BY " +
            "CuttingUnitCode, " +
            "StratumCode, " +
            "SampleGroupCode, " +
            "ifnull(Species, ''), " +
            "ifnull(LiveDead, ''))\r\n" +

        "SELECT " +
            "tp.Description, " +
            "tp.StratumCode, " +
            "st.Method AS StratumMethod, " +
            "tp.SampleGroupCode, " +
            "tp.Species, " +
            "tp.LiveDead, " +
            "tp.HotKey, " +
            "ifnull(tl.TreeCount, 0) AS TreeCount, " +
            "ifnull(tl.SumKPI, 0) AS SumKPI, " +
            //"sum(tl.KPI) SumKPI, " +
            "sg.SamplingFrequency AS Frequency, " +
            "sg.MinKPI AS sgMinKPI, " +
            "sg.MaxKPI AS sgMaxKPI, " +
            "sg.UseExternalSampler " +
        //$"ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally " +
        "FROM TallyPopulation AS tp " +
        "JOIN SampleGroup_V3 AS sg USING (StratumCode, SampleGroupCode) " +
        //"Left JOIN SamplerState ss USING (StratumCode, SampleGroupCode) " +
        "JOIN Stratum AS st ON tp.StratumCode = st.Code " +
        "JOIN CuttingUnit_Stratum AS cust ON tp.StratumCode = cust.StratumCode AND cust.CuttingUnitCode = @p1 " +
        "LEFT JOIN tallyPopTreeCounts AS tl " +
            "ON tl.CuttingUnitCode = @p1 " +
            "AND tp.StratumCode = tl.StratumCode " +
            "AND tp.SampleGroupCode = tl.SampleGroupCode " +
            "AND ifnull(tp.Species, '') = ifnull(tl.Species, '') " +
            "AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '') ";


        #region plot

        public string AddNewPlot(string cuttingUnitCode)
        {
            var plotID = Guid.NewGuid().ToString();

            var plotNumber = GetNextPlotNumber(cuttingUnitCode);

            Database.Execute2(
                "INSERT INTO Plot_V3 (" +
                    "PlotID, " +
                    "PlotNumber, " +
                    "CuttingUnitCode, " +
                    "CreatedBy" +
                ") VALUES ( " +
                    "@PlotID," +
                    "@PlotNumber, " +
                    "@CuttingUnitCode, " +
                    "@CreatedBy " +
                ");" +
                "INSERT INTO Plot_Stratum (" +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "CreatedBy " +
                ")" +
                "SELECT " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "st.Code, " +
                    "@CreatedBy AS CreatedBy " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                $"WHERE p.PlotID = @PlotID AND st.Method IN ({PLOT_METHODS}) " +
                $"AND st.Method != '{CruiseMethods.THREEPPNT}';",
                new { CuttingUnitCode = cuttingUnitCode, PlotID = plotID, PlotNumber = plotNumber, CreatedBy = UserName }); // dont automaticly add plot_stratum for 3ppnt methods

            return plotID;
        }

        public Plot GetPlot(string plotID)
        {
            return Database.Query<Plot>(
                "SELECT " +
                    "p.* " +
                "FROM Plot_V3 AS p " +
                "WHERE PlotID = @p1;", new object[] { plotID })
                .FirstOrDefault();
        }

        public Plot GetPlot(string cuttingUnitCode, int plotNumber)
        {
            return Database.Query<Plot>(
                "SELECT " +
                    "p.PlotID, " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "p.Slope, " +
                    "p.Aspect, " +
                    "p.Remarks, " +
                    "p.XCoordinate, " +
                    "p.YCoordinate, " +
                    "p.ZCoordinate " +
                "FROM Plot_V3 AS p " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;", new object[] { cuttingUnitCode, plotNumber })
                .FirstOrDefault();
        }

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>("SELECT *  FROM Plot_V3 " +
                "WHERE CuttingUnitCode = @p1;"
                , new object[] { unit });
        }

        public void UpdatePlot(Plot plot)
        {
            Database.Execute2(
                "UPDATE Plot_V3 SET " +
                    "PlotNumber = @PlotNumber, " +
                    "Slope = @Slope, " +
                    "Aspect = @Aspect, " +
                    "Remarks = @Remarks, " +
                    "XCoordinate = @XCoordinate, " +
                    "YCoordinate = @YCoordinate, " +
                    "ZCoordinate = @ZCoordinate, " +
                    "ModifiedBy = @UserName " +
                "WHERE PlotID = @PlotID; ",
                    new
                    {
                        plot.PlotNumber,
                        plot.Slope,
                        plot.Aspect,
                        plot.Remarks,
                        plot.XCoordinate,
                        plot.YCoordinate,
                        plot.ZCoordinate,
                        UserName,
                        plot.PlotID,
                    });
        }

        public void UpdatePlotNumber(string plotID, int plotNumber)
        {
            Database.Execute("UPDATE Plot_V3 SET PlotNumber = @p1 WHERE PlotID = @p2;", plotNumber, plotID);
        }

        public void DeletePlot(string unitCode, int plotNumber)
        {
            Database.Execute(
                "DELETE FROM Plot_V3 WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 ;", new object[] { unitCode, plotNumber });
        }

        #endregion plot

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var tallyPops = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                $"WHERE st.Method IN ({PLOT_METHODS})"
                , new object[] { unitCode }).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;",
                    unitCode, plotNumber, pop.StratumCode) == 1;
            }

            return tallyPops;
        }

        #region plot stratum

        public void InsertPlot_Stratum(Plot_Stratum plotStratum)
        {
            var plot_stratum_CN = Database.ExecuteScalar2<long?>(
                "INSERT INTO Plot_Stratum (" +
                   "CuttingUnitCode, " +
                   "PlotNumber, " +
                   "StratumCode, " +
                   "IsEmpty, " +
                   "KPI, " +
                   "ThreePRandomValue, " +
                   "CreatedBy " +
                ") VALUES (" +
                    $"@CuttingUnitCode, " +
                    $"@PlotNumber, " +
                    $"@StratumCode, " +
                    $"@IsEmpty, " +
                    $"@KPI, " +
                    $"@ThreePRandomValue, " +
                    $"'{UserName}'" +
                ");" +
                "SELECT last_insert_rowid();",
                plotStratum);

            plotStratum.InCruise = true;
            plotStratum.Plot_Stratum_CN = plot_stratum_CN;
        }

        public IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false)
        {
            return Database.Query<Plot_Stratum>(
                "SELECT " +
                    "ps.Plot_Stratum_CN, " +
                    "(CASE WHEN ps.Plot_Stratum_CN IS NOT NULL THEN 1 ELSE 0 END) AS InCruise, " +
                    "st.Code AS StratumCode, " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "st.BasalAreaFactor AS BAF, " +
                    "st.FixedPlotSize AS FPS, " +
                    "st.Method AS CruiseMethod, " +
                    "st.KZ3PPNT AS KZ, " +
                    "ps.IsEmpty," +
                    "ps.KPI " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, PlotNumber) " +
                $"WHERE p.CuttingUnitCode = @p1 AND st.Method IN ({PLOT_METHODS}) " +
                "AND p.PlotNumber = @p2;",
                new object[] { unitCode, plotNumber }).ToArray();
        }

        public Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber)
        {
            // we're going to read from the plot table instead of the stratum table
            // because we want to return a dummy record with InCruise set to false
            // when a plot_stratum record doesn't exist
            return Database.Query<Plot_Stratum>(
                "SELECT " +
                    "ps.Plot_Stratum_CN, " +
                    "(ps.Plot_Stratum_CN IS NOT NULL) AS InCruise, " +
                    "p.PlotNumber, " +
                    "st.Code AS StratumCode, " +
                    "p.CuttingUnitCode, " +
                    "st.BasalAreaFactor AS BAF, " +
                    "st.FixedPlotSize AS FPS, " +
                    "st.Method AS CruiseMethod, " +
                    "st.KZ3PPNT AS KZ, " +
                    "ps.IsEmpty," +
                    "ps.KPI " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, PlotNumber) " +
                "WHERE p.CuttingUnitCode = @p1 " +
                "AND st.Code = @p2 " +
                "AND p.PlotNumber = @p3; ",
                new object[] { unitCode, stratumCode, plotNumber }).FirstOrDefault();

            //var stratumPlot = Database.Query<StratumPlot>(
            //    "SELECT " +
            //        "CAST (1 AS BOOLEAN) AS InCruise, " +
            //        "ps.StratumCode, " +
            //        "ps.CuttingUnitCode, " +
            //        "st.BasalAreaFactor AS BAF, " +
            //        "st.FixedPlotSize AS FPS, " +
            //        "st.Method AS CruiseMethod, " +
            //        "st.KZ3PPNT AS KZ, " +
            //        "ps.* " +
            //    "FROM Plot_Stratum " +
            //    "JOIN Stratum AS st ON ps.StratumCode = st.Code " +
            //    "WHERE ps.CuttingUnitCode = @p1 " +
            //    "AND ps.StratumCode = @p2 " +
            //    "AND ps.PlotNumber = @p3;", new object[] { unitCode, stratumCode, plotNumber }).FirstOrDefault();

            //if (stratumPlot == null)
            //{
            //    stratumPlot = Database.Query<StratumPlot>(
            //        "SELECT " +
            //            "CAST (0 AS BOOLEAN) AS InCruise, " +
            //            "st.Code AS StratumCode, " +
            //            "st.BasalAreaFactor AS BAF, " +
            //            "st.FixedPlotSize AS FPS, " +
            //            "st.Method AS CruiseMethod, " +
            //            "st.KZ3PPNT AS KZ " +
            //        "FROM Stratum AS st " +
            //            "WHERE Stratum.Code = @p1;"
            //            , new object[] { stratumCode }).FirstOrDefault();

            //    stratumPlot.UnitCode = unitCode;
            //    stratumPlot.PlotNumber = plotNumber;
            //}
            //else
            //{
            //    stratumPlot.InCruise = true;
            //}

            //return stratumPlot;
        }

        public void UpdatePlot_Stratum(Plot_Stratum stratumPlot)
        {
            Database.Execute2(
                "UPDATE Plot_Stratum SET " +
                    "IsEmpty = @IsEmpty, " +
                    "KPI = @KPI " +
                "WHERE " +
                "CuttingUnitCode = @CuttingUnitCode " +
                "AND StratumCode = @StratumCode " +
                "AND PlotNumber = @PlotNumber;",
                stratumPlot);
        }

        public void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber)
        {
            Database.Execute("DELETE FROM Plot_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3; "
                , cuttingUnitCode, stratumCode, plotNumber);
        }

        #endregion plot stratum

        #region tree

        public void InsertTree(TreeStub_Plot tree)
        {
            var treeID = tree.TreeID ?? Guid.NewGuid().ToString();

            Database.Execute2(
                "INSERT INTO Tree_V3 ( " +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES ( " +
                    "@TreeID,\r\n " +
                    "@TreeNumber,\r\n " +
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " +
                    "@StratumCode,\r\n " +
                    "@SampleGroupCode,\r\n " +
                    "@Species,\r\n" + // species
                    "@LiveDead,\r\n" + // liveDead
                    "@CountOrMeasure " + // countMeasure
                "); " +
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM " +
                ") VALUES ( " +
                    "@TreeID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM " +
                "); "
                , new
                {
                    TreeID = treeID,
                    tree.TreeNumber,
                    tree.CuttingUnitCode,
                    tree.PlotNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    tree.CountOrMeasure,
                    tree.TreeCount,
                    tree.KPI,
                    tree.STM,
                });

            tree.TreeID = treeID;
        }

        public string CreatePlotTree(string unitCode, int plotNumber,
            string stratumCode, string sampleGroupCode,
            string species = null, string liveDead = "L",
            string countMeasure = "M", int treeCount = 1,
            int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreatePlotTree(tree_guid, unitCode, plotNumber, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
        }

        protected void CreatePlotTree(string treeID, string unitCode, int plotNumber,
            string stratumCode, string sampleGroupCode,
            string species = null, string liveDead = "L",
            string countMeasure = "M", int treeCount = 1,
            int kpi = 0, bool stm = false)
        {
            var tallyLedgerID = treeID;

            Database.Execute2(
                "INSERT INTO Tree_V3 ( " +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES (" +
                    "@TreeID,\r\n " +
                    "(SELECT ifnull(max(TreeNumber), 0) + 1 FROM Tree_V3 WHERE CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber),\r\n " +
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " +
                    "@StratumCode,\r\n " +
                    "@SampleGroupCode,\r\n " +
                    "@Species,\r\n" + //species
                    "@LiveDead,\r\n" + //liveDead
                    "@CountOrMeasure);\r\n" + //countMeasure
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM" +
                ") VALUES ( " +
                    "@TallyLedgerID," +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM " +
                ");"
                , new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    Species = species,
                    LiveDead = liveDead,
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = (stm) ? "Y" : "N",
                }
            );
        }

        public IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber)
        {
            return Database.Query<TreeStub_Plot>(
                "SELECT " +
                "t.TreeID, " +
                "t.CuttingUnitCode, " +
                "t.TreeNumber, " +
                "t.PlotNumber, " +
                "t.StratumCode, " +
                "t.SampleGroupCode, " +
                "t.Species, " +
                "t.LiveDead, " +
                "tl.TreeCount, " +
                "tl.STM, " +
                "tl.KPI, " +
                "max(tm.TotalHeight, tm.MerchHeightPrimary, tm.UpperStemHeight) AS Height, " +
                "max(tm.DBH, tm.DRC, tm.DBHDoubleBarkThickness) AS Diameter, " +
                "t.CountOrMeasure " +
                "FROM Tree_V3 AS t " +
                "LEFT JOIN TallyLedger_Tree_Totals AS tl USING (TreeID) " +
                "LEFT JOIN TreeMeasurment AS tm USING (TreeID) " +
                "WHERE t.CuttingUnitCode = @p1 " +
                "AND t.PlotNumber = @p2 " +
                "GROUP BY tl.TreeID " +
                "ORDER BY t.TreeNumber " +
                ";", new object[] { unitCode, plotNumber });
        }

        public int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon)
        {
            if (isRecon)
            {
                // if cruise is a recon cruise we do number trees seperatly for each stratum
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;"
                    , unitCode, plotNumber, stratumCode);
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;"
                    , unitCode, plotNumber);
            }
        }

        #endregion tree

        public void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark)
        {
            Database.Execute(
                "UPDATE Plot_V3 SET Remarks = ifnull(Remarks || ', ' || @p3, @p3) " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;", cuttingUnitCode, plotNumber, remark);
        }

        public int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT Count(*) FROM Tree_V3 " +
                "WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3;",
                unitCode, stratumCode, plotNumber);
        }

        public int GetNextPlotNumber(string unitCode)
        {
            return Database.ExecuteScalar<int>("SELECT ifnull(max(PlotNumber), 0) + 1 FROM Plot_V3 AS p " +
                "JOIN CuttingUnit AS cu ON cu.Code = p.CuttingUnitCode " +
                "WHERE p.CuttingUnitCode = @p1;", unitCode);
        }

        public bool IsPlotNumberAvalible(string unitCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM Plot_V3 AS p " +
                "WHERE p.CuttingUnitCode = @p1 AND p.PlotNumber = @p2;", unitCode, plotNumber) == 0;
        }

        public IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.CuttingUnitCode = @p1 " +
                "AND pe.PlotNumber = @p2;",
                unit, plotNumber).ToArray();
        }

        public IEnumerable<PlotError> GetPlotErrors(string plotID)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.PlotID = @p1;",
                plotID).ToArray();
        }

        public IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.IsResolved," +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "JOIN Tree_V3 AS t USING (TreeID) " +
                "JOIN Plot_V3 AS p USING (CuttingUnitCode, PlotNumber) " +
                "WHERE p.PlotID = @p1;",
                new object[] { plotID }).ToArray();
        }

        private bool GetIsTallyPopInCruise(string unitCode, int plotNumber, string stratumCode)
        {
            return Database.ExecuteScalar<bool?>(
                "SELECT EXISTS (" +
                    "SELECT * " +
                    "FROM Plot_Stratum " +
                    "WHERE StratumCode = @p1 " +
                        "AND CuttingUnitCode = @p2 " +
                        "AND PlotNumber = @p3);",
                stratumCode, unitCode, plotNumber) ?? false;
        }
    }
}