using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TreeFieldValueDataservice : CruiseDataserviceBase, ITreeFieldValueDataservice
    {
        public TreeFieldValueDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeFieldValueDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID)
        {
            return Database.Query<TreeFieldValue>(
                "SELECT " +
                    "t.TreeID, " +
                    "tfs.Field, " +
                    "ifnull(tfh.Heading, tf.DefaultHeading) AS Heading, " +
                    "tf.DbType, " +
                    "tfv.ValueReal, " +
                    "tfv.ValueBool, " +
                    "tfv.ValueText, " +
                    "tfv.ValueInt, " +
                    "tfs.DefaultValueInt, " +
                    "tfs.DefaultValueReal, " +
                    "tfs.DefaultValueBool, " +
                    "tfs.DefaultValueText, " +
                    "tfs.IsHidden, " +
                    "tfs.IsLocked " +
                "FROM Tree AS t " +
                "JOIN TreeFieldSetup AS tfs ON t.StratumCode = tfs.StratumCode AND t.CruiseID = tfs.CruiseID AND (tfs.SampleGroupCode IS NULL OR t.SampleGroupCode = tfs.SampleGroupCode) " +
                "LEFT JOIN TreeFieldHeading AS tfh USING (Field, CruiseID) " +
                "JOIN TreeField AS tf USING (Field) " +
                "LEFT JOIN TreeFieldValue_All AS tfv USING (TreeID, Field) " +
                "WHERE t.TreeID = @p1 " +
                "ORDER BY tfs.FieldOrder;", treeID).ToArray();
        }

        public void UpdateTreeFieldValue(TreeFieldValue treeFieldValue)
        {
            if (treeFieldValue is null) { throw new ArgumentNullException(nameof(treeFieldValue)); }

            Database.Execute(
                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {treeFieldValue.Field}, CreatedBy)" +
                $"VALUES (@p1, @p2, @p3)" +
                $"ON CONFLICT (TreeID) DO " +
                $"UPDATE SET {treeFieldValue.Field} = @p2, ModifiedBy = @p3 WHERE TreeID = @p1;",
                treeFieldValue.TreeID, treeFieldValue.Value, DeviceID);
        }
    }
}