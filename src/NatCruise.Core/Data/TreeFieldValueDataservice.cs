using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TreeFieldValueDataservice : CruiseDataserviceBase, ITreeFieldValueDataservice
    {
        public const string INPUTREGEX_PERCENTAGE = "^\\s*(100|[0-9]{0,2})\\s*$";
        public const string INPUTREGEX_POSINT = "^\\s*[0-9]+\\s*$";
        public const string INPUTREGEX_POSREAL = "^\\s*([0-9]+\\.?[0-9]*|\\.[0-9]*)\\s*$";
        public const string INPUTREGEX_SLOPE = "^\\s*(200|[0-1]?[0-9]{0,2})\\s*$";
        public const string INPUTREGEX_ASPECT = "^\\s*(360|3[0-5][0-9]|[0-2]?[0-9]{0,2})\\s*$";

        public static IDictionary<string, string> FIELD_INPUTREGEX_MAP { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
                {"SeenDefectPrimary", INPUTREGEX_PERCENTAGE},
                {"SeenDefectSecondary", INPUTREGEX_PERCENTAGE},
                { "RecoverablePrimary", INPUTREGEX_PERCENTAGE},
                { "HiddenPrimary", INPUTREGEX_PERCENTAGE},
                { "Grade", null},
                { "HeightToFirstLiveLimb", INPUTREGEX_POSREAL},
                { "PoleLength", INPUTREGEX_POSREAL},
                { "ClearFace", null},
                { "CrownRatio", null},
                { "DBH", INPUTREGEX_POSREAL},
                { "DRC", INPUTREGEX_POSREAL},
                { "TotalHeight", INPUTREGEX_POSREAL},
                { "MerchHeightPrimary", INPUTREGEX_POSREAL},
                { "MerchHeightSecondary", INPUTREGEX_POSREAL},
                { "FormClass", INPUTREGEX_POSREAL},
                { "UpperStemDiameter", INPUTREGEX_POSREAL},
                { "UpperStemHeight", INPUTREGEX_POSREAL},
                { "DBHDoubleBarkThickness", INPUTREGEX_POSREAL},
                { "TopDIBPrimary", INPUTREGEX_POSREAL},
                { "TopDIBSecondary", INPUTREGEX_POSREAL},
                { "DefectCode", INPUTREGEX_POSREAL},
                { "DiameterAtDefect", INPUTREGEX_POSREAL},
                { "VoidPercent", INPUTREGEX_PERCENTAGE},
                { "Slope", INPUTREGEX_SLOPE},
                { "Aspect", INPUTREGEX_ASPECT},
                { "Remarks", null},
                { "MetaData", null},
                { "IsFallBuckScale", null},
                { "Initials", null},
        };

        public TreeFieldValueDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeFieldValueDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public TreeFieldValueDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID)
        {
            var tfvs = Database.Query<TreeFieldValue>(
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

            foreach(var x in tfvs)
            {
                if (FIELD_INPUTREGEX_MAP.TryGetValue(x.Field, out var inputRegex))
                {
                    x.InputRegex = inputRegex;
                }
            }

            return tfvs;
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