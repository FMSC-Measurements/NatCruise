using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
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
);",        new
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

        public void UpdateCuttingUnit(CuttingUnit unit)
        {
            Database.Execute2(
@"UPDATE CuttingUnit SET
    CuttingUnitCode = @CuttingUnitCode,
    Area = @Area,
    Description = @Description,
    Remarks = @Remarks,
    LoggingMethod = @LoggingMethod,
    PaymentUnit = @PaymentUnit,
    Rx = @Rx
WHERE CuttingUnitID = @CuttingUnitID;", unit);
        }
    }
}