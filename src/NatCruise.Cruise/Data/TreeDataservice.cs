using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Data
{
    public class TreeDataservice : CruiseDataserviceBase, ITreeDataservice
    {
        private const string UPSERT_TREEMEASURMENT_COMMAND =
@"INSERT INTO TreeMeasurment (
    TreeID,

    SeenDefectPrimary,
    SeenDefectSecondary,
    RecoverablePrimary,
    HiddenPrimary,
    Grade,

    HeightToFirstLiveLimb,
    PoleLength,
    ClearFace,
    CrownRatio,
    DBH,

    DRC,
    TotalHeight,
    MerchHeightPrimary,
    MerchHeightSecondary,
    FormClass,

    UpperStemDiameter,
    UpperStemHeight,
    DBHDoubleBarkThickness,
    TopDIBPrimary,
    TopDIBSecondary,

    DefectCode,
    DiameterAtDefect,
    VoidPercent,
    Slope,
    Aspect,

    Remarks,
    IsFallBuckScale,
    Initials,
    CreatedBy

) VALUES (
    @TreeID,

    @SeenDefectPrimary,
    @SeenDefectSecondary,
    @RecoverablePrimary,
    @HiddenPrimary,
    @Grade,

    @HeightToFirstLiveLimb,
    @PoleLength,
    @ClearFace,
    @CrownRatio,
    @DBH,

    @DRC,
    @TotalHeight,
    @MerchHeightPrimary,
    @MerchHeightSecondary,
    @FormClass,

    @UpperStemDiameter,
    @UpperStemHeight,
    @DBHDoubleBarkThickness,
    @TopDIBPrimary,
    @TopDIBSecondary,

    @DefectCode,
    @DiameterAtDefect,
    @VoidPercent,
    @Slope,
    @Aspect,

    @Remarks,
    @IsFallBuckScale,
    @Initials,
    @DeviceID )
ON CONFLICT (TreeID) DO UPDATE SET

    SeenDefectPrimary = @SeenDefectPrimary,
    SeenDefectSecondary = @SeenDefectSecondary,
    RecoverablePrimary = @RecoverablePrimary,
    HiddenPrimary = @HiddenPrimary,
    Grade = @Grade,

    HeightToFirstLiveLimb = @HeightToFirstLiveLimb,
    PoleLength = @PoleLength,
    ClearFace = @ClearFace,
    CrownRatio = @CrownRatio,
    DBH = @DBH,

    DRC = @DRC,
    TotalHeight = @TotalHeight,
    MerchHeightPrimary = @MerchHeightPrimary,
    MerchHeightSecondary = @MerchHeightSecondary,
    FormClass = @FormClass,

    UpperStemDiameter = @UpperStemDiameter,
    UpperStemHeight = @UpperStemHeight,
    DBHDoubleBarkThickness = @DBHDoubleBarkThickness,
    TopDIBPrimary = @TopDIBPrimary,
    TopDIBSecondary = @TopDIBSecondary,

    DefectCode = @DefectCode,
    DiameterAtDefect = @DiameterAtDefect,
    VoidPercent = @VoidPercent,
    Slope = @Slope,
    Aspect = @Aspect,

    Remarks = @Remarks,
    IsFallBuckScale = @IsFallBuckScale,
    Initials = @Initials,

    ModifiedBy = @DeviceID
WHERE TreeID = @TreeID;";

        private const string GET_TREESTUB_BASE_COMMAND =
@"WITH treeErrorCount AS
(
    SELECT
        TreeID,
        count(*)AS ErrorCount
    FROM TreeError
    WHERE Level = 'E'
    GROUP BY TreeID
),

treeWarningCount AS
(
    SELECT
        TreeID,
        count(*)AS WarningCount
    FROM TreeError
    WHERE Level = 'W'
    GROUP BY TreeID
)

SELECT
    t.TreeID,
    t.TreeNumber,
    t.StratumCode,
    t.SampleGroupCode,
    t.SpeciesCode,
    t.LiveDead,
    t.PlotNumber,
    max(tm.TotalHeight, tm.MerchHeightPrimary, tm.UpperStemHeight) AS Height,
    max(tm.DBH, tm.DRC, tm.DBHDoubleBarkThickness) AS Diameter,
    t.CountOrMeasure,
    te.ErrorCount,
    tw.WarningCount
FROM Tree AS t
LEFT JOIN TreeMeasurment AS tm USING (TreeID)
LEFT JOIN treeErrorCount AS te USING (TreeID)
LEFT JOIN treeWarningCount AS tw USING (TreeID)
";

        private const string GET_TREEEX_BASE_COMMAND =
@"WITH treeErrorCount AS
(
    SELECT
        TreeID,
        count(*)AS ErrorCount
    FROM TreeError
    WHERE Level = 'E'
    GROUP BY TreeID
),

treeWarningCount AS
(
    SELECT
        TreeID,
        count(*)AS WarningCount
    FROM TreeError
    WHERE Level = 'W'
    GROUP BY TreeID
)

SELECT
    t.*,
    tm.*,
    te.ErrorCount,
    tw.WarningCount
FROM Tree AS t
LEFT JOIN TreeMeasurment AS tm USING (TreeID)
LEFT JOIN treeErrorCount AS te USING (TreeID)
LEFT JOIN treeWarningCount AS tw USING (TreeID)
";

        private const string GET_TREEEX_BASE_COMMAND_2 =
@"WITH treeErrorCount AS
(
    SELECT
        TreeID,
        count(*)AS ErrorCount
    FROM TreeError
    WHERE Level = 'E'
    GROUP BY TreeID
),

treeWarningCount AS
(
    SELECT
        TreeID,
        count(*)AS WarningCount
    FROM TreeError
    WHERE Level = 'W'
    GROUP BY TreeID
)


SELECT
    t.*,
    CAST (ifnull(tl.KPI, 0) AS REAL) AS KPI,
    (CASE ifnull(tl.STM, 0) WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END) AS STM,
    te.ErrorCount,
    tw.WarningCount,

        -- MEASURMENT FIELDS
        ifnull(tm.SeenDefectPrimary, 
            (SELECT tfs.DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS SeenDefectPrimary,
        ifnull(tm.SeenDefectSecondary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'SeenDefectSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS SeenDefectSecondary,
        ifnull(tm.RecoverablePrimary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'RecoverablePrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS RecoverablePrimary,
        ifnull(tm.HiddenPrimary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HiddenPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS HiddenPrimary,
        ifnull(tm.Grade, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Grade' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS Grade,

        ifnull(tm.HeightToFirstLiveLimb, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'HeightToFirstLiveLimb' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS HeightToFirstLiveLimb,
        ifnull(tm.PoleLength, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'PoleLength' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS PoleLength,
        ifnull(tm.ClearFace, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'ClearFace' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS ClearFace,
        ifnull(tm.CrownRatio, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'CrownRatio' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS CrownRatio,
        ifnull(tm.DBH, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBH' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS DBH,

        ifnull(tm.DRC, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DRC' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS DRC,
        ifnull(tm.TotalHeight, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TotalHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS TotalHeight,
        ifnull(tm.MerchHeightPrimary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS MerchHeightPrimary,
        ifnull(tm.MerchHeightSecondary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MerchHeightSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS MerchHeightSecondary,
        ifnull(tm.FormClass, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'FormClass' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS FormClass,

        ifnull(tm.UpperStemDiameter, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemDiameter' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS UpperStemDiameter,
        ifnull(tm.UpperStemHeight, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'UpperStemHeight' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS UpperStemHeight,
        ifnull(tm.DBHDoubleBarkThickness, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DBHDoubleBarkThickness' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS DBHDoubleBarkThickness,
        ifnull(tm.TopDIBPrimary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBPrimary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS TopDIBPrimary,
        ifnull(tm.TopDIBSecondary, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'TopDIBSecondary' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS TopDIBSecondary,

        ifnull(tm.DefectCode, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DefectCode' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS DefectCode,
        ifnull(tm.DiameterAtDefect, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'DiameterAtDefect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS DiameterAtDefect,
        ifnull(tm.VoidPercent, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'VoidPercent' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS VoidPercent,
        ifnull(tm.Slope, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Slope' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS Slope,
        ifnull(tm.Aspect, 
            (SELECT DefaultValueReal FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Aspect' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS Aspect,

        ifnull(tm.Remarks, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Remarks' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS Remarks,
        ifnull(tm.IsFallBuckScale, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'IsFallBuckScale' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS IsFallBuckScale,

        ifnull(tm.MetaData, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'MetaData' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS MetaData, 
        ifnull(tm.Initials, 
            (SELECT DefaultValueText FROM TreeFieldSetup AS tfs WHERE tfs.CruiseID = t.CruiseID AND t.StratumCode = tfs.StratumCode AND (t.SampleGroupCode = tfs.SampleGroupCode OR tfs.SampleGroupCode IS NULL) AND tfs.Field = 'Initials' ORDER BY tfs.SampleGroupCode DESC LIMIT 1)) AS Initials

FROM Tree AS t
LEFT JOIN TallyLedger AS tl ON tl.TreeID = t.TreeID
LEFT JOIN TreeMeasurment AS tm ON t.TreeID = tm.TreeID
LEFT JOIN treeErrorCount AS te USING (TreeID)
LEFT JOIN treeWarningCount AS tw USING (TreeID)
";

        public TreeDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

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
@"INSERT INTO Tree (
    TreeID,
    TreeNumber,
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    CountOrMeasure
) VALUES (
    @TreeID,
    (SELECT ifnull(max(TreeNumber), 0) +1
        FROM Tree
        WHERE CuttingUnitCode = @CuttingUnitCode AND CruiseID = @CruiseID
        AND PlotNumber IS NULL),
    @CruiseID,
    @CuttingUnitCode,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    'M'
);
INSERT INTO TallyLedger (
    TallyLedgerID,
    TreeID,
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    STM
) VALUES (
    @TallyLedgerID,
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @TreeCount,
    @KPI,
    @STM
);"
,
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CruiseID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    SpeciesCode = species,
                    LiveDead = liveDead,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = stm,
                });
        }

        public string GetDefaultLiveDead(string stratumCode, string sampleGroupCode)
        {
            return Database.ExecuteScalar<string>("SELECT DefaultLiveDead FROM SampleGroup WHERE StratumCode = @p1 AND SampleGroupCode = @p2 AND CruiseID = @p3;"
                , stratumCode, sampleGroupCode, CruiseID);
        }

        public Tree_Ex GetTree(string treeID)
        {
            return Database.Query<Tree_Ex>(GET_TREEEX_BASE_COMMAND + "WHERE t.TreeID = @p1;", treeID).FirstOrDefault();
        }

        public IEnumerable<Tree_Ex> GetTreesByUnitCode(string unitCode)
        {
            return Database.Query<Tree_Ex>(GET_TREEEX_BASE_COMMAND_2 +
                "WHERE t.CuttingUnitCode = @p1 AND t.CruiseID = @p2 AND t.PlotNumber IS NULL",
                unitCode, CruiseID).ToArray();
        }

        public TreeStub GetTreeStub(string treeID)
        {
            return Database.Query<TreeStub>(GET_TREESTUB_BASE_COMMAND + " WHERE TreeID = @p1", treeID).FirstOrDefault();
        }

        public IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode)
        {
            return Database.Query<TreeStub>(GET_TREESTUB_BASE_COMMAND + " WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber IS NULL;",
                unitCode, CruiseID);
        }

        public IEnumerable<TreeStub> GetPlotTreeStubsByUnitCode(string unitCode)
        {
            //return Database.From<TreeStub>()
            //    .LeftJoin("TreeMeasurment", "USING (TreeID)")
            //    .Where("CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber NOT NULL")
            //    .Query(unitCode, CruiseID);

            return Database.Query<TreeStub>(GET_TREESTUB_BASE_COMMAND + " WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber NOT NULL;",
                unitCode, CruiseID);
        }

        public IEnumerable<Tree_Ex> GetPlotTreesByUnitCode(string unitCode)
        {
            return Database.Query<Tree_Ex>(GET_TREEEX_BASE_COMMAND_2 + " WHERE t.CuttingUnitCode = @p1 AND t.CruiseID = @p2 AND t.PlotNumber NOT NULL;",
                unitCode, CruiseID);
        }

        public IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID)
        {
            return Database.Query<TreeFieldValue>(
                "SELECT " +
                    "t.TreeID, " +
                    "tf.Field, " +
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

        public void UpdateTree(Tree tree)
        {
            if (tree is null) { throw new ArgumentNullException(nameof(tree)); }

            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
                "UPDATE Tree SET \r\n " +
                    "TreeNumber = @TreeNumber, " +
                    "StratumCode = @StratumCode, " +
                    "SampleGroupCode = @SampleGroupCode, " +
                    "SpeciesCode = @SpeciesCode," +
                    "LiveDead = @LiveDead, " +
                    "CountOrMeasure = @CountOrMeasure, " +
                    "ModifiedBy = @DeviceID " +
                "WHERE TreeID = @TreeID; ",
                new
                {
                    tree.TreeID,
                    tree.TreeNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.SpeciesCode,
                    tree.LiveDead,
                    tree.CountOrMeasure,

                    DeviceID,
                });
        }

        public void UpdateTree(Tree_Ex tree)
        {
            if (tree == null) { throw new ArgumentNullException(nameof(tree)); }

            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
@"UPDATE Tree SET
    TreeNumber = @TreeNumber,
    StratumCode = @StratumCode,
    SampleGroupCode = @SampleGroupCode,
    SpeciesCode = @SpeciesCode,
    LiveDead = @LiveDead,
    CountOrMeasure = @CountOrMeasure,
    ModifiedBy = @DeviceID
WHERE TreeID = @TreeID;
" +
UPSERT_TREEMEASURMENT_COMMAND,
                new
                {
                    tree.TreeID,
                    tree.TreeNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.SpeciesCode,
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
                    DeviceID,
                });
        }

        public void UpsertTreeMeasurments(TreeMeasurment mes)
        {
            if (mes is null) { throw new ArgumentNullException(nameof(mes)); }

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
                    DeviceID,
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
            if (treeFieldValue is null) { throw new ArgumentNullException(nameof(treeFieldValue)); }

            Database.Execute(
                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {treeFieldValue.Field}, CreatedBy)" +
                $"VALUES (@p1, @p2, @p3)" +
                $"ON CONFLICT (TreeID) DO " +
                $"UPDATE SET {treeFieldValue.Field} = @p2, ModifiedBy = @p3 WHERE TreeID = @p1;",
                treeFieldValue.TreeID, treeFieldValue.Value, DeviceID);
        }

        public void DeleteTree(string tree_guid)
        {
            Database.Execute("Delete FROM Tree WHERE TreeID = @p1", tree_guid);
        }

        #region util

        public int? GetTreeNumber(string treeID)
        {
            return Database.ExecuteScalar<int?>("SELECT TreeNumber FROM Tree WHERE TreeID = @p1;", treeID);
        }

        public bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null, string stratumCode = null)
        {
            if (plotNumber != null)
            {
                if (stratumCode == null) { throw new ArgumentNullException(nameof(stratumCode), "if plot number is not null, stratum code must be as well"); }

                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber = @p2 " +
                    "AND TreeNumber = @p3 " +
                    "AND ((SELECT UseCrossStrataPlotTreeNumbering FROM Cruise WHERE CruiseID = @p4) = 1 OR StratumCode = @p5) " +
                    "AND CruiseID = @p4;",
                    unit, plotNumber, treeNumber, CruiseID, stratumCode) == 0;
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber IS NULL " +
                    "AND TreeNumber = @p2 " +
                    "AND CruiseID = @p3;",
                    unit, treeNumber, CruiseID) == 0;
            }
        }

        public void UpdateTreeInitials(string tree_guid, string value)
        {
            Database.Execute2(
@"INSERT INTO TreeMeasurment (
    TreeID,
    Initials
) VALUES (
    @TreeID,
    @Initials
)
ON CONFLICT (TreeID) DO
UPDATE SET Initials = @Initials
WHERE TreeID = @TreeID;",
                new
                {
                    TreeID = tree_guid,
                    Initials = value,
                });
        }

        #endregion util

        #region validation

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

        public void SetTreeAuditResolution(string treeID, string treeAuditRuleID, string resolution, string initials)
        {
            Database.Execute("INSERT OR REPLACE INTO TreeAuditResolution " +
                "(TreeID, CruiseID, TreeAuditRuleID, Resolution, Initials)" +
                "VALUES" +
                "(@p1, @p2, @p3, @p4, @p5);"
                , treeID, CruiseID, treeAuditRuleID, resolution, initials);
        }

        public void ClearTreeAuditResolution(string treeID, string treeAuditRuleID)
        {
            Database.Execute("DELETE FROM TreeAuditResolution WHERE TreeID = @p1 AND TreeAuditRuleID = @p2"
                , treeID, treeAuditRuleID);
        }

        #endregion validation
    }
}