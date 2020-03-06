using FMSC.ORM.Core.SQL.QueryBuilder;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public partial class CuttingUnitDatastore : ITreeDatastore
    {
        private const string UPSERT_TREEMEASURMENT_COMMAND =
            "INSERT INTO TreeMeasurment ( " +
                "TreeID, " +

                "SeenDefectPrimary," +
                "SeenDefectSecondary, " +
                "RecoverablePrimary, " +
                "HiddenPrimary, " +
                "Grade, " +

                "HeightToFirstLiveLimb, " +
                "PoleLength, " +
                "ClearFace, " +
                "CrownRatio, " +
                "DBH, " +

                "DRC, " +
                "TotalHeight, " +
                "MerchHeightPrimary, " +
                "MerchHeightSecondary, " +
                "FormClass, " +

                "UpperStemDiameter, " +
                "UpperStemHeight, " +
                "DBHDoubleBarkThickness, " +
                "TopDIBPrimary, " +
                "TopDIBSecondary, " +

                "DefectCode, " +
                "DiameterAtDefect, " +
                "VoidPercent, " +
                "Slope, " +
                "Aspect, " +

                "Remarks, " +
                "IsFallBuckScale, " +
                "Initials, " +
                "CreatedBy" +

            ") VALUES ( " +
                "@TreeID, " +

                "@SeenDefectPrimary, " +
                "@SeenDefectSecondary, " +
                "@RecoverablePrimary, " +
                "@HiddenPrimary, " +
                "@Grade, " +

                "@HeightToFirstLiveLimb, " +
                "@PoleLength, " +
                "@ClearFace, " +
                "@CrownRatio, " +
                "@DBH, " +

                "@DRC, " +
                "@TotalHeight, " +
                "@MerchHeightPrimary, " +
                "@MerchHeightSecondary, " +
                "@FormClass, " +

                "@UpperStemDiameter, " +
                "@UpperStemHeight, " +
                "@DBHDoubleBarkThickness, " +
                "@TopDIBPrimary, " +
                "@TopDIBSecondary, " +

                "@DefectCode, " +
                "@DiameterAtDefect, " +
                "@VoidPercent, " +
                "@Slope, " +
                "@Aspect, " +

                "@Remarks, " +
                "@IsFallBuckScale, " +
                "@Initials, " +
                "@UserName ) " +
            "ON CONFLICT (TreeID) DO UPDATE SET " +

                "SeenDefectPrimary = @SeenDefectPrimary, " +
                "SeenDefectSecondary = @SeenDefectSecondary, " +
                "RecoverablePrimary = @RecoverablePrimary, " +
                "HiddenPrimary = @HiddenPrimary, " +
                "Grade = @Grade, " +

                "HeightToFirstLiveLimb = @HeightToFirstLiveLimb, " +
                "PoleLength = @PoleLength, " +
                "ClearFace = @ClearFace, " +
                "CrownRatio = @CrownRatio, " +
                "DBH = @DBH, " +

                "DRC = @DRC, " +
                "TotalHeight = @TotalHeight, " +
                "MerchHeightPrimary = @MerchHeightPrimary, " +
                "MerchHeightSecondary = @MerchHeightSecondary, " +
                "FormClass = @FormClass, " +

                "UpperStemDiameter = @UpperStemDiameter, " +
                "UpperStemHeight = @UpperStemHeight, " +
                "DBHDoubleBarkThickness = @DBHDoubleBarkThickness, " +
                "TopDIBPrimary = @TopDIBPrimary, " +
                "TopDIBSecondary = @TopDIBSecondary, " +

                "DefectCode = @DefectCode, " +
                "DiameterAtDefect = @DiameterAtDefect, " +
                "VoidPercent = @VoidPercent, " +
                "Slope = @Slope, " +
                "Aspect = @Aspect, " +

                "Remarks = @Remarks, " +
                "IsFallBuckScale = @IsFallBuckScale, " +
                "Initials = @Initials, " +

                "ModifiedBy = @UserName " +
            "WHERE TreeID = @TreeID;";

        public string CreateMeasureTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreateMeasureTree(tree_guid, unitCode, stratumCode, sampleGroupCode, species, liveDead, treeCount, kpi, stm);
            return tree_guid;
        }

        protected void CreateMeasureTree(string treeID, string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false)
        {
            liveDead = liveDead ?? GetDefaultLiveDead(stratumCode, sampleGroupCode);

            var tallyLedgerID = treeID;

            Database.Execute2(
                "INSERT INTO Tree_V3 (" +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES ( " +
                    "@TreeID, " +
                    "(SELECT ifnull(max(TreeNumber), 0) +1 " +
                        "FROM Tree_V3 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode " +
                        "AND PlotNumber IS NULL), " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "'M'" +
                "); " +
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode," +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM " +
                ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM" +
                ");",
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    Species = species,
                    LiveDead = liveDead,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = stm,
                });
        }

        public string GetDefaultLiveDead(string stratumCode, string sampleGroupCode)
        {
            return Database.ExecuteScalar<string>("SELECT DefaultLiveDead FROM SampleGroup_V3 WHERE StratumCode = @p1 AND SampleGroupCode = @p2;"
                , stratumCode, sampleGroupCode);
        }

        public Tree_Ex GetTree(string treeID)
        {
            return Database.Query<Tree_Ex>(
                "SELECT t.*, tm.* FROM Tree_V3 AS t " +
                "LEFT JOIN TreeMeasurment AS tm USING (TreeID) " +
                "WHERE t.TreeID = @p1;", treeID).FirstOrDefault();
        }

        public IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID)
        {
            return Database.Query<TreeFieldValue>(
                "SELECT " +
                "t.TreeID, " +
                "tf.Field, " +
                "tfs.Heading, " +
                "tf.DbType, " +
                "tfv.ValueReal, " +
                "tfv.ValueBool, " +
                "tfv.ValueText, " +
                "tfv.ValueInt " +
                "FROM Tree_V3 AS t " +
                "JOIN TreeFieldSetup_V3 AS tfs USING (StratumCode) " +
                "JOIN TreeField AS tf USING (Field) " +
                "LEFT JOIN TreeFieldValue_TreeMeasurment AS tfv USING (TreeID, Field) " +
                "WHERE t.TreeID = @p1 " +
                "ORDER BY tfs.FieldOrder;", treeID).ToArray();
        }

        public void UpdateTree(Tree tree)
        {
            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
                "UPDATE Tree_V3 SET \r\n " +
                    "TreeNumber = @TreeNumber, " +
                    "StratumCode = @StratumCode, " +
                    "SampleGroupCode = @SampleGroupCode, " +
                    "Species = @Species," +
                    "LiveDead = @LiveDead, " +
                    "CountOrMeasure = @CountOrMeasure, " +
                    "ModifiedBy = @UserName " +
                "WHERE TreeID = @TreeID; ",
                new
                {
                    tree.TreeID,
                    tree.TreeNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    tree.CountOrMeasure,

                    UserName,
                });
        }

        public void UpdateTree(Tree_Ex tree)
        {
            if(tree == null) { throw new ArgumentNullException(nameof(tree)); }

            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
                "UPDATE Tree_V3 SET \r\n " +
                    "TreeNumber = @TreeNumber, " +
                    "StratumCode = @StratumCode, " +
                    "SampleGroupCode = @SampleGroupCode, " +
                    "Species = @Species," +
                    "LiveDead = @LiveDead, " +
                    "CountOrMeasure = @CountOrMeasure, " +
                    "ModifiedBy = @UserName " +
                "WHERE TreeID = @TreeID; " +
                UPSERT_TREEMEASURMENT_COMMAND,
                new
                {
                    tree.TreeID,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    CountOrMeasure = tree.CountOrMeasure ?? "",

                    tree.SeenDefectPrimary,
                    tree.SeenDefectSecondary,
                    tree.RecoverablePrimary,
                    tree.HiddenPrimary,
                    tree.Grade,

                    tree.HeightToFirstLiveLimb,
                    tree.PoleLength,
                    tree.ClearFace,
                    tree.CrownRatio,
                    tree.DBH,

                    tree.DRC,
                    tree.TotalHeight,
                    tree.MerchHeightPrimary,
                    tree.MerchHeightSecondary,
                    tree.FormClass,

                    tree.UpperStemDiameter,
                    tree.UpperStemHeight,
                    tree.DBHDoubleBarkThickness,
                    tree.TopDIBPrimary,
                    tree.TopDIBSecondary,

                    tree.DefectCode,
                    tree.DiameterAtDefect,
                    tree.VoidPercent,
                    tree.Slope,
                    tree.Aspect,

                    tree.Remarks,
                    tree.IsFallBuckScale,
                    tree.Initials,
                    UserName,
                });
        }

        public void UpsertTreeMeasurments(TreeMeasurment mes)
        {
            Database.Execute2(
                UPSERT_TREEMEASURMENT_COMMAND,
                new
                {
                    mes.TreeID,

                    mes.SeenDefectPrimary,
                    mes.SeenDefectSecondary,
                    mes.RecoverablePrimary,
                    mes.HiddenPrimary,
                    mes.Grade,

                    mes.HeightToFirstLiveLimb,
                    mes.PoleLength,
                    mes.ClearFace,
                    mes.CrownRatio,
                    mes.DBH,

                    mes.DRC,
                    mes.TotalHeight,
                    mes.MerchHeightPrimary,
                    mes.MerchHeightSecondary,
                    mes.FormClass,

                    mes.UpperStemDiameter,
                    mes.UpperStemHeight,
                    mes.DBHDoubleBarkThickness,
                    mes.TopDIBPrimary,
                    mes.TopDIBSecondary,

                    mes.DefectCode,
                    mes.DiameterAtDefect,
                    mes.VoidPercent,
                    mes.Slope,
                    mes.Aspect,

                    mes.Remarks,
                    mes.IsFallBuckScale,
                    mes.Initials,
                    UserName,
                }
                );
        }

        public Task UpdateTreeAsync(Tree_Ex tree)
        {
            return Task.Run(() => UpdateTree(tree));
        }

        public void UpdateTreeRemarks(string treeID, string remarks)
        {
            //try
            //{
            //    var stuff = Database.QueryGeneric($"Select * from TreeMeasurment where treeid = '{treeID}';").ToArray();

                UpdateTreeFieldValue(
                    new TreeFieldValue
                    {
                        TreeID = treeID,
                        Field = "Remarks",
                        ValueText = remarks,
                        DBType = "TEXT",
                    });

            //    var stuffagain = Database.QueryGeneric($"Select * from TreeMeasurment where treeid = '{treeID}';").ToArray();
            //}
            //catch(Exception e)
            //{
            //    Logger.Log.E(e);
            //}
        }

        public void UpdateTreeFieldValue(TreeFieldValue treeFieldValue)
        {
            Database.Execute(
                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {treeFieldValue.Field})" +
                $"VALUES (@p1, @p2)" +
                $"ON CONFLICT (TreeID) DO " +
                $"UPDATE SET {treeFieldValue.Field} = @p2 WHERE TreeID = @p1;",
                treeFieldValue.TreeID, treeFieldValue.Value);
        }

        public void DeleteTree(string tree_guid)
        {
            Database.Execute("Delete FROM Tree_V3 WHERE TreeID = @p1", tree_guid);
        }

        #region util

        public int? GetTreeNumber(string treeID)
        {
            return Database.ExecuteScalar<int?>("SELECT TreeNumber FROM Tree_V3 WHERE TreeID = @p1;", treeID);
        }

        public IEnumerable<TreeError> GetTreeErrors(string treeID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.TreeAuditRuleID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.IsResolved," +
                "te.Resolution, " +
                "te.ResolutionInitials " +
                "FROM TreeError AS te " +
                "WHERE te.TreeID = @p1;",
                new object[] { treeID }).ToArray();
        }

        public TreeError GetTreeError(string treeID, string treeAuditRuleID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.TreeAuditRuleID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.IsResolved," +
                "te.Resolution, " +
                "te.ResolutionInitials " +
                "FROM TreeError AS te " +
                "WHERE te.TreeID = @p1 " +
                "AND te.TreeAuditRuleID = @p2;",
                new object[] { treeID, treeAuditRuleID }).FirstOrDefault();
        }

        public bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null)
        {
            if (plotNumber != null)
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber = @p2 " +
                    "AND TreeNumber = @p3;",
                    unit, plotNumber, treeNumber) == 0;
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber IS NULL " +
                    "AND TreeNumber = @p2;",
                    unit, treeNumber) == 0;
            }
        }

        public void UpdateTreeInitials(string tree_guid, string value)
        {
            Database.Execute(
                "UPDATE TreeMeasurment SET " +
                "Initials = @p1 " +
                "WHERE TreeID = @p2",
                value, tree_guid);
        }

        public void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials)
        {
            Database.Execute("INSERT OR REPLACE INTO TreeAuditResolution " +
                "(TreeID, TreeAuditRuleID, Resolution, Initials)" +
                "VALUES" +
                "(@p1, @p2, @p3, @p4);"
                , treeID, treeAuditRuleID, resolution, initials);
        }

        public void ClearTreeAuditResolution(string treeID, string treeAuditRuleID)
        {
            Database.Execute("DELETE FROM TreeAuditResolution WHERE TreeID = @p1 AND TreeAuditRuleID = @p2"
                , treeID, treeAuditRuleID);
        }

        #endregion util
    }
}