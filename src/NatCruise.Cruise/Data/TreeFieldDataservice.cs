using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Data
{
    public class TreeFieldDataservice : CruiseDataserviceBase, ITreeFieldDataservice
    {
        public TreeFieldDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeFieldDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<TreeField> GetNonPlotTreeFields(string unitCode)
        {
            return Database.Query2<TreeField>(
@"SELECT    
    tfs.Field,
    ifnull(tfh.Heading, tf.DefaultHeading) AS Heading
FROM (
    SELECT DISTINCT Field, CruiseID
    FROM TreeFieldSetup
    JOIN CuttingUnit_Stratum AS cust USING (CruiseID, StratumCode)
    JOIN Stratum AS st USING (CruiseID, StratumCode)
    JOIN LK_CruiseMethod AS cm USING (Method)
    WHERE TreeFieldSetup.CruiseID = @CruiseID AND cust.CuttingUnitCode = @CuttingUnitCode AND cm.IsPlotMethod == 0
    ORDER BY FieldOrder) AS tfs

JOIN TreeField AS tf USING (Field)
LEFT JOIN TreeFieldHeading AS tfh USING (CruiseID, Field)
WHERE tf.IsTreeMeasurmentField = 1;", new { CruiseID, CuttingUnitCode = unitCode }).ToArray();
        }

        public IEnumerable<TreeField> GetPlotTreeFields(string unitCode)
        {
            return Database.Query2<TreeField>(
@"SELECT    
    tfs.Field,
    ifnull(tfh.Heading, tf.DefaultHeading) AS Heading
FROM (
    SELECT DISTINCT Field, CruiseID
    FROM TreeFieldSetup
    JOIN CuttingUnit_Stratum AS cust USING (CruiseID, StratumCode)
    JOIN Stratum AS st USING (CruiseID, StratumCode)
    JOIN LK_CruiseMethod AS cm USING (Method)
    WHERE TreeFieldSetup.CruiseID = @CruiseID AND cust.CuttingUnitCode = @CuttingUnitCode AND cm.IsPlotMethod == 1
    ORDER BY FieldOrder) AS tfs

JOIN TreeField AS tf USING (Field)
LEFT JOIN TreeFieldHeading AS tfh USING (CruiseID, Field)
WHERE tf.IsTreeMeasurmentField = 1;", new { CruiseID, CuttingUnitCode = unitCode }).ToArray();
        }
    }
}
