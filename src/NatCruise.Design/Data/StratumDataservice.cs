using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class StratumDataservice : CruiseDataserviceBase, IStratumDataservice
    {
        public StratumDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public StratumDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
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

        public IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode)
        {
            return Database.QueryScalar2<string>(
@"SELECT cu.CuttingUnitCode
FROM CuttingUnit AS cu 
JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID)
WHERE StratumCode = @StratumCode AND CruiseID = @CruiseID;",
                new { StratumCode = stratumCode, CruiseID }).ToArray();
        }

        public IEnumerable<Stratum> GetStrata()
        {
            return Database.From<Stratum>().Query();
        }



        public void RemoveStratumFromCuttingUnit(string cuttingUnitCode, string stratumCode)
        {
            bool force = false;

            if (force || !HasTreeCounts(cuttingUnitCode, stratumCode))
            {
                Database.Execute("DELETE FROM CuttingUnit_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2;", cuttingUnitCode, stratumCode);
            }
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

        public bool HasTreeCounts(string unitCode, string stratum)
        {
            var treecount = Database.ExecuteScalar<int>("SELECT sum(TreeCount) FROM TallyLedger WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND CruiseID = @p3;"
                , unitCode, stratum, CruiseID);

            var numTrees = Database.ExecuteScalar<int>("SELECT count(*) FROM Tree WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND CruiseID = @p3;"
                , unitCode, stratum, CruiseID);

            return numTrees > 0;
        }
    }
}