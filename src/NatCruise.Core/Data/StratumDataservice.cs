using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class StratumDataservice : CruiseDataserviceBase, IStratumDataservice
    {
        public StratumDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public StratumDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        protected const string SELECT_STRATUM_CORE =
@"SELECT
    Stratum.*,
    ( EXISTS ( SELECT * FROM Tree WHERE CruiseID = Stratum.CruiseID AND StratumCode = Stratum.StratumCode))
        AS HasTrees
FROM Stratum
";

        public IEnumerable<Stratum> GetStrata(string cuttingUnitCode = null)
        {
            if (string.IsNullOrEmpty(cuttingUnitCode))
            {
                return Database.Query<Stratum>(SELECT_STRATUM_CORE + "WHERE Stratum.CruiseID = @p1;", CruiseID)
                    .ToArray();

                //return Database.From<Stratum>(SELECT_STRATUM_CORE)
                //.Where("CruiseID = @p1").Query(CruiseID).ToArray();
            }
            else
            {
                return Database.Query<Stratum>(SELECT_STRATUM_CORE + "WHERE Stratum.CruiseID = @p1 " +
                    "AND Stratum.StratumCode IN (SELECT StratumCode FROM CuttingUnit_Stratum WHERE CruiseID = @p1 AND CuttingUnitCode = @p2);",
                    CruiseID, cuttingUnitCode).ToArray();

                //return Database.From<Stratum>()
                //    .Where("Stratum.CruiseID = @p1 " +
                //        "AND Stratum.StratumCode IN (SELECT StratumCode FROM CuttingUnit_Stratum WHERE CruiseID = @p1)")
                //    .Query(CruiseID, cuttingUnitCode).ToArray();
            }
        }

        public Stratum GetStratum(string stratumCode)
        {
            if (stratumCode is null) { throw new ArgumentNullException(nameof(stratumCode)); }

            return Database.Query<Stratum>(SELECT_STRATUM_CORE + "WHERE CruiseID = @p1 AND StratumCode = @p2;", CruiseID, stratumCode)
                .Single();

            //return Database.From<Stratum>()
            //    .Where("CruiseID = @p1 AND StratumCode = @p2").Query(CruiseID, stratumCode).Single();
        }

        public void AddStratum(Stratum stratum)
        {
            if (stratum is null) { throw new ArgumentNullException(nameof(stratum)); }

            stratum.StratumID ??= Guid.NewGuid().ToString();
            Database.Execute2(
@"INSERT INTO Stratum (
    CruiseID,
    StratumID,
    StratumCode,
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    FixCNTField,
    CreatedBy
) VALUES (
    @CruiseID,
    @StratumID,
    @StratumCode,
    @Description,
    @Method,
    @BasalAreaFactor,
    @FixedPlotSize,
    @KZ3PPNT,
    @SamplingFrequency,
    @HotKey,
    @FBSCode,
    @YieldComponent,
    @FixCNTField,
    @DeviceID
);", new
{
    CruiseID,
    stratum.StratumID,
    stratum.StratumCode,
    stratum.Description,
    stratum.Method,
    stratum.BasalAreaFactor,
    stratum.FixedPlotSize,
    stratum.KZ3PPNT,
    stratum.SamplingFrequency,
    stratum.HotKey,
    stratum.FBSCode,
    stratum.YieldComponent,
    stratum.FixCNTField,
    DeviceID,
});
        }

        public void AddStratumToCuttingUnit(string cuttingUnitCode, string stratumCode)
        {
            Database.Execute(
@"INSERT OR IGNORE INTO CuttingUnit_Stratum (
    CuttingUnitCode,
    StratumCode,
    CruiseID,
    CreatedBy
) VALUES (@p1, @p2, @p3, @p4);", cuttingUnitCode, stratumCode, CruiseID, DeviceID);
        }

        public void DeleteStratum(Stratum stratum)
        {
            if (stratum is null) { throw new ArgumentNullException(nameof(stratum)); }

            DeleteStratum(stratum.StratumCode);
        }

        public void DeleteStratum(string stratumCode)
        {
            if (string.IsNullOrEmpty(stratumCode)) { throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty.", nameof(stratumCode)); }

            Database.Execute("DELETE FROM Stratum WHERE StratumCode = @p1 AND CruiseID = @p2;", stratumCode, CruiseID);
        }

        public string GetCruiseMethod(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT Method FROM Stratum WHERE StratumCode = @p1 AND CruiseID = @p2;", stratumCode, CruiseID);
        }

        public IEnumerable<StratumCuttingUnitInfo> GetStratumUnitInfo(string stratumCode)
        {
            return Database.Query<StratumCuttingUnitInfo>(
@"SELECT cust.*,
(
    EXISTS ( SELECT * FROM Tree WHERE CruiseID = cust.CruiseID AND StratumCode = cust.StratumCode)
) AS HasTrees
FROM CuttingUnit_Stratum as cust
WHERE cust.CruiseID = @p1 AND cust.StratumCode = @p2;", CruiseID, stratumCode);
        }

        public IEnumerable<string> GetStratumCodes(string cuttingUnitCode = null)
        {
            return Database.QueryScalar<string>(
                "SELECT StratumCode FROM CuttingUnit_Stratum " +
                "WHERE (@p1 IS NULL OR CuttingUnitCode = @p1) AND CruiseID = @p2;", cuttingUnitCode, CruiseID);
        }

        public IEnumerable<string> GetStratumCodesByUnit(string unitCode)
        {
            return Database.QueryScalar<string>(
                "SELECT StratumCode FROM CuttingUnit_Stratum " +
                "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2;", unitCode, CruiseID);
        }

        // TODO method just returns tree based strata, check logic
        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.Query<Stratum>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID) " +
                "WHERE CuttingUnitCode = @p1 AND st.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 0)",
                new object[] { unitCode, CruiseID })
                .ToArray();
        }

        public IEnumerable<Stratum> GetPlotStrata(string unitCode)
        {
            return Database.Query<Stratum>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID) " +
                "WHERE CuttingUnitCode = @p1 AND st.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)",
                new object[] { unitCode, CruiseID })
                .ToArray();
        }

        public void RemoveStratumFromCuttingUnit(string cuttingUnitCode, string stratumCode)
        {
            Database.Execute("DELETE FROM CuttingUnit_Stratum WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND StratumCode = @p3;", CruiseID, cuttingUnitCode, stratumCode);

            Database.Execute("DELETE FROM Plot_Stratum WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND StratumCode = @p3;", CruiseID, cuttingUnitCode, stratumCode);
        }

        public void UpdateStratum(Stratum stratum)
        {
            if (stratum is null) { throw new ArgumentNullException(nameof(stratum)); }

            Database.Execute2(
@"UPDATE Stratum SET
    Description = @Description,
    Method = @Method,
    BasalAreaFactor = @BasalAreaFactor,
    FixedPlotSize = @FixedPlotSize,
    KZ3PPNT = @KZ3PPNT,
    SamplingFrequency = @SamplingFrequency,
    FBSCode = @FBSCode,
    YieldComponent = @YieldComponent,
    FixCNTField = @FixCNTField,
    ModifiedBy = @DeviceID
WHERE StratumID = @StratumID;",
            new
            {
                stratum.StratumID,
                stratum.Description,
                stratum.Method,
                stratum.BasalAreaFactor,
                stratum.FixedPlotSize,
                stratum.KZ3PPNT,
                stratum.SamplingFrequency,
                stratum.FBSCode,
                stratum.YieldComponent,
                stratum.FixCNTField,
                DeviceID,
            });
        }

        public void UpdateStratumCode(Stratum stratum)
        {
            if (stratum is null) { throw new ArgumentNullException(nameof(stratum)); }

            Database.Execute2(
@"UPDATE Stratum SET
    StratumCode = @StratumCode,
    ModifiedBy = @DeviceID
WHERE StratumID = @StratumID;",
            new
            {
                stratum.StratumID,
                stratum.StratumCode,
                DeviceID,
            });
        }

        // TODO should method return true if has tree counts or trees?
        // Indicates if specific unit in stratum has field data
        public bool HasTreeCounts(string unitCode, string stratum)
        {
            var treecount = Database.ExecuteScalar<int>("SELECT sum(TreeCount) FROM TallyLedger WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND CruiseID = @p3;"
                , unitCode, stratum, CruiseID);

            var numTrees = Database.ExecuteScalar<int>("SELECT count(*) FROM Tree WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND CruiseID = @p3;"
                , unitCode, stratum, CruiseID);

            return numTrees > 0;
        }

        public bool HasTrees(string unitCode, string stratum)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND CruiseID = @p3;"
                , unitCode, stratum, CruiseID) > 0;
        }
    }
}