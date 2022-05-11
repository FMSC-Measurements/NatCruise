using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TreeFieldDataservice : CruiseDataserviceBase, ITreeFieldDataservice
    {
        public TreeFieldDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeFieldDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        
        public IEnumerable<string> GetTreeFieldsNames()
        {
            throw new System.NotImplementedException();
        }

        public void AddTreeField(TreeField field)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTreeField(string field)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TreeField> GetTreeFieldsByStratum(string stratumCode)
        {
            throw new System.NotImplementedException();
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

        public IEnumerable<TreeField> GetTreeFields()
        {
            return Database.Query<TreeField>(
@"SELECT
    tf.Field,
    tfh.Heading,
    tf.DefaultHeading,
    DbType,
    IsTreeMeasurmentField
FROM TreeField AS tf
LEFT JOIN TreeFieldHeading AS tfh ON tf.Field = tfh.Field AND tfh.CruiseID = @p1
ORDER BY ifnull(tfh.Heading, tf.DefaultHeading);", CruiseID).ToArray();
        }

        public void UpdateTreeField(TreeField treeField)
        {
            if (string.IsNullOrEmpty(treeField.Heading))
            {
                Database.Execute2("DELETE FROM TreeFieldHeading WHERE CruiseID = @CruiseID AND Field = @Field;",
                    new { CruiseID, treeField.Field });
            }
            else
            {
                Database.Execute2(
@"INSERT INTO TreeFieldHeading (
    CruiseID,
    Field,
    Heading
) VALUES (
    @CruiseID,
    @Field,
    @Heading
)
ON CONFLICT (CruiseID, Field) DO
UPDATE SET Heading = @Heading
WHERE CruiseID = @CruiseID AND Field = @Field;",
new
{
    CruiseID,
    treeField.Field,
    treeField.Heading,
});
            }
        }
    }
}