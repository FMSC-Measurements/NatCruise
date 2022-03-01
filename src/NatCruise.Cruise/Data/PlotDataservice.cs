using CruiseDAL.Schema;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Cruise.Data
{
    public class PlotDataservice : CruiseDataserviceBase, IPlotDataservice
    {
        public PlotDataservice(CruiseDAL.CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public PlotDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        #region plot

        public string AddNewPlot(string cuttingUnitCode)
        {
            var plotID = Guid.NewGuid().ToString();

            var plotNumber = GetNextPlotNumber(cuttingUnitCode);

            Database.Execute2(
$@"INSERT INTO Plot (
    PlotID,
    CruiseID,
    PlotNumber,
    CuttingUnitCode,
    CreatedBy
) VALUES (
    @PlotID,
    @CruiseID,
    @PlotNumber,
    @CuttingUnitCode,
    @CreatedBy
);
INSERT INTO Plot_Stratum (
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    CreatedBy
)
SELECT
    p.CruiseID,
    p.CuttingUnitCode,
    p.PlotNumber,
    st.StratumCode,
    @CreatedBy AS CreatedBy
FROM Plot AS p
JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID)
JOIN Stratum AS st USING (StratumCode, CruiseID)
WHERE p.PlotID = @PlotID AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)
AND st.Method != '{CruiseMethods.THREEPPNT}';",
                new { CruiseID, CuttingUnitCode = cuttingUnitCode, PlotID = plotID, PlotNumber = plotNumber, CreatedBy = DeviceID }); // dont automaticly add plot_stratum for 3ppnt methods

            return plotID;
        }

        public Plot GetPlot(string plotID)
        {
            return Database.Query<Plot>(
                "SELECT " +
                    "p.* " +
                "FROM Plot AS p " +
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
                    "p.Remarks " +
                "FROM Plot AS p " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND CruiseID = @p3;", new object[] { cuttingUnitCode, plotNumber, CruiseID })
                .FirstOrDefault();
        }

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>("SELECT *  FROM Plot " +
                "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2;"
                , new object[] { unit, CruiseID });
        }

        public void UpdatePlot(Plot plot)
        {
            if (plot is null) { throw new ArgumentNullException(nameof(plot)); }

            Database.Execute2(
                "UPDATE Plot SET " +
                    "PlotNumber = @PlotNumber, " +
                    "Slope = @Slope, " +
                    "Aspect = @Aspect, " +
                    "Remarks = @Remarks, " +
                    "ModifiedBy = @DeviceID " +
                "WHERE PlotID = @PlotID; ",
                    new
                    {
                        plot.PlotNumber,
                        plot.Slope,
                        plot.Aspect,
                        plot.Remarks,
                        DeviceID,
                        plot.PlotID,
                    });
        }

        public void UpdatePlotNumber(string plotID, int plotNumber)
        {
            Database.Execute("UPDATE Plot SET PlotNumber = @p1 WHERE PlotID = @p2;", plotNumber, plotID);
        }

        public void DeletePlot(string unitCode, int plotNumber)
        {
            Database.Execute(
                "DELETE FROM Plot WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND CruiseID = @p3;", new object[] { unitCode, plotNumber, CruiseID });
        }

        #endregion plot

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var cruiseID = CruiseID;

            var tallyPops = Database.Query<TallyPopulation_Plot>(
@"WITH tallyPopTreeCounts AS (
    SELECT CruiseID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        sum(TreeCount) AS TreeCount,
        sum(KPI) AS SumKPI
    FROM TallyLedger AS tl
    WHERE CuttingUnitCode = @p1 AND CruiseID = @p2
    GROUP BY
        CruiseID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        ifnull(SpeciesCode, ''),
        ifnull(LiveDead, ''))

SELECT
    tp.Description,
    tp.StratumCode,
    st.Method AS StratumMethod,
    tp.SampleGroupCode,
    tp.SpeciesCode,
    tp.LiveDead,
    tp.HotKey,
    ifnull(tl.TreeCount, 0) AS TreeCount,
    ifnull(tl.SumKPI, 0) AS SumKPI,
    --sum(tl.KPI) SumKPI,
    sg.SamplingFrequency AS Frequency,
    sg.MinKPI AS sgMinKPI,
    sg.MaxKPI AS sgMaxKPI,
    sg.UseExternalSampler
-- ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally
FROM TallyPopulation AS tp
JOIN SampleGroup AS sg USING (StratumCode, SampleGroupCode, CruiseID)
-- Left JOIN SamplerState ss USING (StratumCode, SampleGroupCode)
JOIN Stratum AS st USING (StratumCode, CruiseID)
JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
LEFT JOIN tallyPopTreeCounts AS tl
        ON tl.CuttingUnitCode = cust.CuttingUnitCode
        AND tl.CruiseID =  tp.CruiseID
        AND tp.StratumCode = tl.StratumCode
        AND tp.SampleGroupCode = tl.SampleGroupCode
        AND ifnull(tp.SpeciesCode, '') = ifnull(tl.SpeciesCode, '')
        AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '')
WHERE cust.CuttingUnitCode = @p1 AND tp.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)"
                , new object[] { unitCode, cruiseID }).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode, cruiseID);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3 AND CruiseID = @p4;",
                    unitCode, plotNumber, pop.StratumCode, cruiseID) == 1;
            }

            return tallyPops;
        }

        private bool GetIsTallyPopInCruise(string unitCode, int plotNumber, string stratumCode, string cruiseID)
        {
            return Database.ExecuteScalar<bool?>(
                "SELECT EXISTS (" +
                    "SELECT * " +
                    "FROM Plot_Stratum " +
                    "WHERE StratumCode = @p1 " +
                        "AND CuttingUnitCode = @p2 " +
                        "AND PlotNumber = @p3 " +
                        "AND CruiseID = @p4);",
                stratumCode, unitCode, plotNumber, cruiseID) ?? false;
        }

        #region plot stratum

        public IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode)
        {
            return Database.Query<StratumProxy>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID) " +
                "WHERE CuttingUnitCode = @p1 AND st.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)",
                new object[] { unitCode, CruiseID })
                .ToArray();
        }

        public void InsertPlot_Stratum(Plot_Stratum plotStratum)
        {
            if (plotStratum is null) { throw new ArgumentNullException(nameof(plotStratum)); }

            var plot_stratum_CN = Database.ExecuteScalar2<long?>(
$@"INSERT INTO Plot_Stratum (
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    IsEmpty,
    CreatedBy
) VALUES (
    '{CruiseID}',
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @IsEmpty,
    '{DeviceID}'
);
SELECT last_insert_rowid();",
                plotStratum);

            plotStratum.InCruise = true;
            plotStratum.Plot_Stratum_CN = plot_stratum_CN;
        }

        public void Insert3PPNT_Plot_Stratum(Plot_Stratum plotStratum)
        {
            if (plotStratum is null) { throw new ArgumentNullException(nameof(plotStratum)); }

            var plot_stratum_CN = Database.ExecuteScalar2<long?>(
$@"INSERT INTO Plot_Stratum (
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    IsEmpty,
    KPI,
    TreeCount,
    AverageHeight,
    ThreePRandomValue,
    CreatedBy
) VALUES (
    '{CruiseID}',
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @IsEmpty,
    @KPI,
    @TreeCount,
    @AverageHeight,
    @ThreePRandomValue,
    '{DeviceID}'
);
SELECT last_insert_rowid();",
                plotStratum);

            plotStratum.InCruise = true;
            plotStratum.Plot_Stratum_CN = plot_stratum_CN;
        }

        public IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false)
        {
            return Database.Query<Plot_Stratum>(
@"SELECT
    ps.Plot_Stratum_CN,
    (CASE WHEN ps.Plot_Stratum_CN IS NOT NULL THEN 1 ELSE 0 END) AS InCruise,
    st.StratumCode,
    p.CuttingUnitCode,
    p.CruiseID,
    p.PlotNumber,
    st.BasalAreaFactor AS BAF,
    st.FixedPlotSize AS FPS,
    st.Method AS CruiseMethod,
    st.KZ3PPNT,
    ps.IsEmpty,
    ps.KPI
FROM Plot AS p
JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID)
JOIN Stratum AS st USING (StratumCode, CruiseID)
LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, CruiseID, PlotNumber)
WHERE p.CuttingUnitCode = @p1 AND p.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)
AND p.PlotNumber = @p3;",
                new object[] { unitCode, CruiseID, plotNumber }).ToArray();
        }

        public Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber)
        {
            // we're going to read from the plot table instead of the stratum table
            // because we want to return a dummy record with InCruise set to false
            // when a plot_stratum record doesn't exist
            return Database.Query<Plot_Stratum>(
$@"SELECT
    ps.Plot_Stratum_CN,
    (ps.Plot_Stratum_CN IS NOT NULL) AS InCruise,
    p.PlotNumber,
    st.StratumCode,
    p.CuttingUnitCode,
    p.CruiseID,
    st.BasalAreaFactor AS BAF,
    st.FixedPlotSize AS FPS,
    st.Method AS CruiseMethod,
    st.KZ3PPNT,
    ps.IsEmpty,
    ps.KPI
FROM Plot AS p
JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID)
JOIN Stratum AS st USING (StratumCode, CruiseID)
LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, CruiseID, PlotNumber)
WHERE p.CuttingUnitCode = @p1
AND p.CruiseID = @p2
AND st.StratumCode = @p3
AND p.PlotNumber = @p4; ",
                new object[] { unitCode, CruiseID, stratumCode, plotNumber }).FirstOrDefault();

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
                "Plot_Stratum_CN = @Plot_Stratum_CN;",
                stratumPlot);
        }

        public void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber)
        {
            Database.Execute("DELETE FROM Plot_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3 AND CruiseID = @p4; "
                , cuttingUnitCode, stratumCode, plotNumber, CruiseID);
        }

        #endregion plot stratum

        #region tree

        public IEnumerable<PlotTreeEntry> GetPlotTreeProxies(string unitCode, int plotNumber)
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

        #endregion tree

        public void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark)
        {
            Database.Execute(
                "UPDATE Plot SET Remarks = (CASE WHEN Remarks ISNULL THEN @p4 ELSE (Remarks || ', ' || @p4) END) " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND CruiseID = @p3;", cuttingUnitCode, plotNumber, CruiseID, remark);
        }

        public int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT Count(*) FROM Tree " +
                "WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3 AND CruiseID = @p4;",
                unitCode, stratumCode, plotNumber, CruiseID);
        }

        public int GetNextPlotNumber(string unitCode)
        {
            return Database.ExecuteScalar<int>("SELECT ifnull(max(PlotNumber), 0) + 1 FROM Plot AS p " +
                "WHERE p.CuttingUnitCode = @p1 AND p.CruiseID = @p2 ;", unitCode, CruiseID);
        }

        public bool IsPlotNumberAvalible(string unitCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM Plot AS p " +
                "WHERE p.CuttingUnitCode = @p1 AND p.CruiseID = @p2 AND p.PlotNumber = @p3;", unitCode, CruiseID, plotNumber) == 0;
        }

        public IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.CuttingUnitCode = @p1 " +
                "AND pe.PlotNumber = @p2" +
                "AND pe.CruiseID =  @p3;",
                unit, plotNumber, CruiseID).ToArray();
        }

        public IEnumerable<PlotError> GetPlotErrors(string plotID)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.PlotID = @p1;",
                plotID).ToArray();
        }

        // TODO remove unused method
        public IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID)
        {
            return Database.Query<TreeError>(
@"SELECT
    te.TreeID,
    te.Field,
    te.Level,
    te.Message,
    te.IsResolved,
    te.Resolution
FROM TreeError AS te
JOIN Tree AS t USING (TreeID)
JOIN Plot AS p USING (CuttingUnitCode, CruiseID, PlotNumber)
WHERE p.PlotID = @p1;",
                new object[] { plotID }).ToArray();
        }
    }
}