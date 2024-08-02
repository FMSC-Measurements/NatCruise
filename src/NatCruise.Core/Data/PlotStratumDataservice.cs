using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class PlotStratumDataservice : CruiseDataserviceBase, IPlotStratumDataservice
    {
        public PlotStratumDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public PlotStratumDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public PlotStratumDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
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

        public IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber)
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

        [Obsolete("parameter insertIfNotExists no longer supported")]
        public IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists)
        {
            return GetPlot_Strata(unitCode, plotNumber);
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
            Database.Execute("DELETE FROM Plot_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3 AND CruiseID = @p4; " +
                "DELETE FROM Tree WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3 AND CruiseID = @p4;"
                , cuttingUnitCode, stratumCode, plotNumber, CruiseID);
        }
    }
}