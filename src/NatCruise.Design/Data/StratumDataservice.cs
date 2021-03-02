using CruiseDAL;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class StratumDataservice : IStratumDataservice
    {
        public StratumDataservice(string path)
        {
            Database = new CruiseDatastore_V3(path);
        }

        private CruiseDatastore Database { get; }

        public void AddStratum(Stratum stratum)
        {
            Database.Insert(stratum);
        }

        public void AddStratumToCuttingUnit(string cuttingUnitCode, string stratumCode)
        {
            Database.Execute("INSERT OR IGNORE INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES (@p1, @p2);",
                cuttingUnitCode, stratumCode);
        }

        public void DeleteStratum(Stratum stratum)
        {
            Database.Delete(stratum);
        }

        public void DeleteStratum(string stratumCode)
        {
            Database.Execute("DELETE FROM Stratum WHERE Code = @p1;", stratumCode);
        }

        public IEnumerable<string> GetCuttingUnitCodesByStratum(string stratumCode)
        {
            return Database.QueryGeneric2(
                "SELECT cu.Code AS CuCode " +
                "FROM CuttingUnit AS cu " +
                "JOIN CuttingUnit_Stratum AS cust ON cu.Code = cust.CuttingUnitCode " +
                "WHERE StratumCode = @stratumCode;",
                new { stratumCode })
                .Select(x => x["CuCode"] as String);
        }

        public IEnumerable<Method> GetMethods()
        {
            return CruiseDAL.Schema.CruiseMethods.SUPPORTED_METHODS.Select(x => new Method { MethodCode = x, FriendlyName = x }); ;
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
            Database.Update(stratum);
        }

        public bool HasTreeCounts(string unitCode, string stratum)
        {
            var treecount = Database.ExecuteScalar<int>("SELECT sum(TreeCount) FROM TallyLedger WHERE CuttingUnitCode = @p1 AND StratumCode = @p2;"
                , unitCode, stratum);

            var numTrees = Database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 WHERE CuttingUnitCode = @p1 AND StratumCode = @p2;"
                , unitCode, stratum);

            return numTrees > 0;
        }
    }
}