using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class CuttingUnitDataservice : CruiseDataserviceBase, ICuttingUnitDataservice
    {
        public CuttingUnitDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public CuttingUnitDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void AddCuttingUnit(CuttingUnit unit)
        {
            unit.CuttingUnitID ??= Guid.NewGuid().ToString();

            Database.Execute2(
@"INSERT INTO CuttingUnit (
    CuttingUnitID,
    CruiseID,
    CuttingUnitCode,
    Area,
    Description,
    Remarks,
    LoggingMethod,
    PaymentUnit,
    Rx,
    CreatedBy
) VALUES (
    @CuttingUnitID,
    @CruiseID,
    @CuttingUnitCode,
    @Area,
    @Description,
    @Remarks,
    @LoggingMethod,
    @PaymentUnit,
    @Rx,
    @DeviceID
);", new
{
    unit.CuttingUnitID,
    CruiseID,
    unit.CuttingUnitCode,
    unit.Area,
    unit.Description,
    unit.Remarks,
    unit.LoggingMethod,
    unit.PaymentUnit,
    unit.Rx,
    DeviceID,
});
        }

        public void DeleteCuttingUnit(CuttingUnit unit)
        {
            Database.Execute("DELETE FROM  CuttingUnit WHERE CuttingUnitCode = @p1 AND CruiseID = @p2", unit.CuttingUnitCode, CruiseID);
        }

        public IEnumerable<string> GetCuttingUnitCodes()
        {
            return Database.ExecuteScalar<string>("SELECT group_concat(cu.CuttingUnitCode) FROM CuttingUnit AS cu WHERE CruiseID = @p1;", CruiseID)?.Split(',') ?? new string[0];
        }

        public IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT group_concat(CuttingUnitCode) FROM CuttingUnit AS cu " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID) " +
                "WHERE cust.StratumCode = @p1 AND cu.CruiseID = @p2;", stratumCode, CruiseID)?.Split(',') ?? new string[0];
        }

        public CuttingUnit GetCuttingUnit(string unitCode)
        {
            return Database.From<CuttingUnit>()
                .Where("CuttingUnitCode = @p1 AND CruiseID = @p2")
                .Query(unitCode, CruiseID).FirstOrDefault();
        }

        public IEnumerable<CuttingUnit> GetCuttingUnits()
        {
            return Database.From<CuttingUnit>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public IEnumerable<CuttingUnit> GetCuttingUnitsByStratum(string stratumCode)
        {
            return Database.Query<CuttingUnit>("SELECT cu.* FROM CuttingUnit AS cu " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode, CruiseID) " +
                "WHERE cust.StratumCode = @p1 AND cu.CruiseID = @p2;", stratumCode, CruiseID).ToArray();
        }

        public CuttingUnitStrataSummary GetCuttingUnitStrataSummary(string unitCode)
        {
            var summary = Database.Query<CuttingUnitStrataSummary>(
@"SELECT
    CuttingUnitCode,
    (SELECT count(*) > 0
        FROM CuttingUnit_Stratum AS cust
        JOIN Stratum USING (StratumCode, CruiseID)
        JOIN LK_CruiseMethod USING (Method)
        WHERE IsPlotMethod = 1
            AND cust.CuttingUnitCode = cu.CuttingUnitCode
            AND cust.CruiseID = cu.CruiseID) AS HasPlotStrata,
    (SELECT count(*) > 0
        FROM CuttingUnit_Stratum AS cust
        JOIN Stratum USING (StratumCode, CruiseID)
        JOIN LK_CruiseMethod USING (Method)
        WHERE IsPlotMethod = 0
            AND cust.CuttingUnitCode = cu.CuttingUnitCode
            AND cust.CruiseID = cu.CruiseID) AS HasTreeStrata
FROM CuttingUnit AS cu
WHERE CruiseID = @p1 AND CuttingUnitCode = @p2;", CruiseID, unitCode).SingleOrDefault();
            if (summary != null)
            {
                summary.Methods = Database.QueryScalar<string>(
@"SELECT Method FROM Stratum AS st
JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
WHERE st.CruiseID = @p1 AND cust.CuttingUnitCode = @p2;", CruiseID, unitCode).ToArray();
            }

            return summary;
        }

        public void UpdateCuttingUnit(CuttingUnit unit)
        {
            Database.Execute2(
@"UPDATE CuttingUnit SET
    Area = @Area,
    Description = @Description,
    Remarks = @Remarks,
    LoggingMethod = @LoggingMethod,
    PaymentUnit = @PaymentUnit,
    Rx = @Rx,
    ModifiedBy = @DeviceID
WHERE CuttingUnitID = @CuttingUnitID;",
            new
            {
                unit.CuttingUnitID,
                unit.Area,
                unit.Description,
                unit.Remarks,
                unit.LoggingMethod,
                unit.PaymentUnit,
                unit.Rx,
                DeviceID,
            });
        }

        public void UpdateCuttingUnitCode(CuttingUnit unit)
        {
            Database.Execute2(
@"UPDATE CuttingUnit SET
    CuttingUnitCode = @CuttingUnitCode,
    ModifiedBy = @DeviceID
WHERE CuttingUnitID = @CuttingUnitID;",
            new
            {
                unit.CuttingUnitID,
                unit.CuttingUnitCode,
                DeviceID,
            });
        }
    }
}