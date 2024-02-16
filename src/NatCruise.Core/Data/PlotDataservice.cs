using CruiseDAL.Schema;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class PlotDataservice : CruiseDataserviceBase, IPlotDataservice
    {
        private const string SELECT_PLOT_CORE =
@"SELECT
    p.PlotID,
    p.CuttingUnitCode,
    p.PlotNumber,
    p.Slope,
    p.Aspect,
    p.Remarks,
    (
        SELECT count(*) FROM TallyLedger AS tl
            WHERE tl.PlotNumber = p.PlotNumber
            AND tl.CruiseID = p.CruiseID
            AND tl.CuttingUnitCode = p.CuttingUnitCode
    ) AS TreeCount,
    (SELECT count(*) FROM PlotError AS pe WHERE pe.PlotID = p.PlotID AND Level = 'E') AS ErrorCount,
    (
        SELECT count(*) FROM TreeError AS te
            JOIN Tree AS t USING (TreeID)
            WHERE te.Level = 'E'
            AND t.PlotNumber = p.PlotNumber
            AND t.CruiseID = p.CruiseID
            AND t.CuttingUnitCode = p.CuttingUnitCode
    ) AS TreeErrorCount,
    (
        SELECT count(*) FROM TreeError AS te
            JOIN Tree AS t USING (TreeID)
            WHERE te.Level = 'W'
            AND t.PlotNumber = p.PlotNumber
            AND t.CruiseID = p.CruiseID
            AND t.CuttingUnitCode = p.CuttingUnitCode
    ) AS TreeWarningCount,
    (
        Select group_concat(StratumCode) FROM Plot_Stratum AS ps
            WHERE ps.PlotNumber = p.PlotNumber
            AND ps.CruiseID = p.CruiseID
            AND ps.CuttingUnitCode = p.CuttingUnitCode
            AND ifnull(ps.IsEmpty, 0) != 0
    ) AS NullStrata
FROM Plot AS p
";

        public PlotDataservice(CruiseDAL.CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public PlotDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public PlotDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        #region plot

        public string AddNewPlot(string cuttingUnitCode)
        {
            var plotNumber = GetNextPlotNumber(cuttingUnitCode);
            return AddNewPlot(cuttingUnitCode, plotNumber);
        }

        public string AddNewPlot(string cuttingUnitCode, int plotNumber)
        {
            if (string.IsNullOrWhiteSpace(cuttingUnitCode)) { throw new ArgumentException($"'{nameof(cuttingUnitCode)}' cannot be blank.", nameof(cuttingUnitCode)); }
            if (plotNumber <= 0) throw new ArgumentOutOfRangeException(nameof(plotNumber));

            var plotID = Guid.NewGuid().ToString();
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
                SELECT_PLOT_CORE +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND CruiseID = @p3;", new object[] { cuttingUnitCode, plotNumber, CruiseID })
                .FirstOrDefault();
        }

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>(
                SELECT_PLOT_CORE +
@"WHERE (@p1 IS NULL OR CuttingUnitCode = @p1)
    AND CruiseID = @p2;"
                , new object[] { unit, CruiseID });
        }

        public IEnumerable<int> GetPlotNumbersOrdered(string cuttingUnitCode, string stratumCode = null)
        {
            return Database.QueryScalar<int>("SELECT DISTINCT PlotNumber FROM Plot_Stratum WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND (@p3 IS NULL OR StratumCode = @p3) ORDER BY PlotNumber;"
                , CruiseID, cuttingUnitCode, stratumCode);
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
    }
}