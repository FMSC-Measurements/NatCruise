using CruiseDAL;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class CuttingUnitStratumDataservice : CruiseDataserviceBase, ICuttingUnitStratumDataservice
    {
        public CuttingUnitStratumDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public CuttingUnitStratumDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public IReadOnlyCollection<CuttingUnitStratum> GetCuttingUnitStrata()
        {
            return Database.Query<CuttingUnitStratum>(
@"SELECT    CuttingUnitCode,
            StratumCode,
            CuttingUnit_CN,
            Stratum_CN,
            Method
FROM CuttingUnit_Stratum
JOIN Stratum USING (CruiseID, StratumCode)
JOIN CuttingUnit USING (CruiseID, CuttingUnitCode)
WHERE CruiseID = @p1", CruiseID).ToArray();
        }
    }
}