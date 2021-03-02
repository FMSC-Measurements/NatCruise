using CruiseDAL;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class CuttingUnitDataservice : ICuttingUnitDataservice
    {
        public CuttingUnitDataservice(string path)
        {
            Database = new CruiseDatastore_V3(path);
        }

        private CruiseDatastore Database { get; }

        public void AddCuttingUnit(CuttingUnit unit)
        {
            Database.Insert(unit);
        }

        public void DeleteCuttingUnit(CuttingUnit unit)
        {
            Database.Execute("DELETE FROM  CuttingUnit WHERE CuttingUnit_CN = @p1", unit.CuttingUnit_CN);
        }

        public IEnumerable<string> GetCuttingUnitCodes()
        {
            return Database.ExecuteScalar<string>("SELECT group_concat(cu.Code) FROM CuttingUnit AS cu;")?.Split(',') ?? new string[0];
        }

        public IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT group_concat(Code) FROM CuttingUnit AS cu " +
                "JOIN CuttingUnit_Stratum AS cust ON cu.Code = cust.CuttingUnitCode " +
                "WHERE cust.StratumCode = @p1;")?.Split(',') ?? new string[0];
        }

        public IEnumerable<CuttingUnit> GetCuttingUnits()
        {
            return Database.From<CuttingUnit>().Query().ToArray();
        }

        public IEnumerable<CuttingUnit> GetCuttingUnitsByStratum(string stratumCode)
        {
            return Database.Query<CuttingUnit>("SELECT cu.* FROM CuttingUnit AS cu " +
                "JOIN CuttingUnit_Stratum AS cust ON cu.Code = cust.CuttingUnitCode " +
                "WHERE cust.StratumCode = @p1;").ToArray();
        }

        public IEnumerable<LoggingMethod> GetLoggingMethods()
        {
            return new LoggingMethod[0];
        }

        public void UpdateCuttingUnit(CuttingUnit unit)
        {
            Database.Execute2(
@"UPDATE CuttingUnit SET
    Code = @CuttingUnitCode,
    Area = @Area,
    Description = @Description,
    LoggingMethod = @LoggingMethod,
    PaymentUnit = @PaymentUnit,
    Rx = @Rx
WHERE CuttingUnit_CN = @CuttingUnit_CN;", unit);
        }
    }
}