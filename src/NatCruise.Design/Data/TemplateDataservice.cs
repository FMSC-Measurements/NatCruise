using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class TemplateDataservice : CruiseDataserviceBase, ITemplateDataservice
    {
        public TemplateDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TemplateDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        #region Species

        public void AddSpecies(Species sp)
        {
            Database.Execute2(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode,
    ContractSpecies,
    FIACode
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @ContractSpecies,
    @FIACode
)",
    new
    {
        CruiseID,
        sp.SpeciesCode,
        sp.ContractSpecies,
        sp.FIACode,
    });
        }

        public IEnumerable<Species> GetSpecies()
        {
            return Database.From<Species>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void UpsertSpecies(Species sp)
        {
            Database.Execute2(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode,
    ContractSpecies,
    FIACode
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @ContractSpecies,
    @FIACode
)
ON CONFLICT (CruiseID, SpeciesCode) DO
UPDATE SET
    ContractSpecies = @ContractSpecies,
    FIACode = @FIACode
WHERE CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode;",
    new
    {
        CruiseID,
        sp.SpeciesCode,
        sp.ContractSpecies,
        sp.FIACode,
    });
        }

        public void DeleteSpecies(string speciesCode)
        {
            Database.Execute("DELETE FROM Species WHERE SpeciesCode = @p1 AND CruiseID = @p2;", speciesCode, CruiseID);
        }

        #endregion Species

        #region species code

        public void AddSpeciesCode(string speciesCode)
        {
            Database.Execute(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode
) VALUES (
    @p1,
    @p2
);", CruiseID, speciesCode);
        }

        public IEnumerable<string> GetSpeciesCodes()
        {
            return Database.QueryScalar<string>("SELECT SpeciesCode FROM Species WHERE CruiseID = @p1;", CruiseID).ToArray();
        }

        #endregion species code

        #region rule selector

        public void AddRuleSelector(TreeAuditRuleSelector tars)
        {
            Database.Execute2(
@"INSERT INTO TreeAuditRuleSelector (
    CruiseID,
    SpeciesCode,
    LiveDead,
    PrimaryProduct,
    TreeAuditRuleID
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @LiveDead,
    @PrimaryProduct,
    @TreeAuditRuleID
);"
            , new
            {
                CruiseID,
                tars.SpeciesCode,
                tars.LiveDead,
                tars.PrimaryProduct,
                tars.TreeAuditRuleID,
            });
        }

        public IEnumerable<TreeAuditRuleSelector> GetRuleSelectors()
        {
            return Database.From<TreeAuditRuleSelector>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public IEnumerable<TreeAuditRuleSelector> GetRuleSelectors(string tarID)
        {
            return Database.From<TreeAuditRuleSelector>()
                .Where("TreeAuditRuleID = @p1")
                .Query(tarID).ToArray();
        }

        public void DeleteRuleSelector(TreeAuditRuleSelector tars)
        {
            Database.Execute2("DELETE FROM TreeAuditRuleSelector WHERE CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode AND PrimaryProduct = @PrimaryProduct AND LiveDead = @LiveDead;",
                new
                {
                    CruiseID,
                    tars.SpeciesCode,
                    tars.PrimaryProduct,
                    tars.LiveDead,
                });
        }

        #endregion rule selector

        #region tree audit rule

        public void AddTreeAuditRule(TreeAuditRule tar)
        {
            if (string.IsNullOrEmpty(tar.TreeAuditRuleID)) { throw new ArgumentException("Value was null or empty", nameof(tar)); }

            Database.Execute2(
@"INSERT INTO TreeAuditRule (
    CruiseID,
    TreeAuditRuleID,
    Field,
    Min,
    Max,
    Description
) VALUES (
    @CruiseID,
    @TreeAuditRuleID,
    @Field,
    @Min,
    @Max,
    @Description
);",
    new
    {
        CruiseID,
        tar.TreeAuditRuleID,
        tar.Field,
        tar.Min,
        tar.Max,
        tar.Description,
    });
        }

        public IEnumerable<TreeAuditRule> GetTreeAuditRules()
        {
            return Database.From<TreeAuditRule>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public IEnumerable<TreeAuditRule> GetTreeAuditRules(string species, string prod, string livedead)
        {
            return Database.From<TreeAuditRule>()
                .Join("TreeAuditRuleSelector", "USING (TreeAuditRuleID)")
                .Where("CruiseID = @CruiseID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '')")
                .Query2(new
                {
                    CruiseID,
                    SpeciesCode = species,
                    PrimaryProduct = prod,
                    LiveDead = livedead,
                });
        }

        public void DeleteTreeAuditRule(TreeAuditRule tar)
        {
            Database.Execute("DELETE FROM TreeAuditRule WHERE TreeAuditRuleID = @p1;", tar.TreeAuditRuleID);
        }

        public void UpsertTreeAuditRule(TreeAuditRule tar)
        {
            if (string.IsNullOrEmpty(tar.TreeAuditRuleID)) { throw new ArgumentException("Value was null or empty", nameof(tar)); }

            Database.Execute2(
@"INSERT INTO TreeAuditRule (
    CruiseID,
    TreeAuditRuleID,
    Field,
    Min,
    Max,
    Description
) VALUES (
    @CruiseID,
    @TreeAuditRuleID,
    @Field,
    @Min,
    @Max,
    @Description
)
ON CONFLICT (TreeAuditRuleID) DO
UPDATE SET
    Field = @Field,
    Min = @Min,
    Max = @Max,
    Description = @Description
WHERE TreeAuditRuleID = @TreeAuditRuleID;",
    new
    {
        CruiseID,
        tar.TreeAuditRuleID,
        tar.Field,
        tar.Min,
        tar.Max,
        tar.Description,
    });
        }

        #endregion tree audit rule

        #region tree default value

        public void AddTreeDefaultValue(TreeDefaultValue tdv)
        {
            Database.Execute2(
@"INSERT INTO TreeDefaultValue (
    CruiseID,
    SpeciesCode,
    PrimaryProduct,
    CullPrimary,
    CullPrimaryDead,
    HiddenPrimary,
    HiddenPrimaryDead,
    TreeGrade,
    TreeGradeDead,
    CullSecondary,
    HiddenSecondary,
    Recoverable,
    MerchHeightLogLength,
    MerchHeightType,
    FormClass,
    BarkThicknessRatio,
    AverageZ,
    ReferenceHeightPercent,
    CreatedBy
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @PrimaryProduct,
    @CullPrimary,
    @CullPrimaryDead,
    @HiddenPrimary,
    @HiddenPrimaryDead,
    @TreeGrade,
    @TreeGradeDead,
    @CullSecondary,
    @HiddenSecondary,
    @Recoverable,
    @MerchHeightLogLength,
    @MerchHeightType,
    @FormClass,
    @BarkThicknessRatio,
    @AverageZ,
    @ReferenceHeightPercent,
    @DeviceID
);",
            new
            {
                CruiseID,
                tdv.SpeciesCode,
                tdv.PrimaryProduct,
                tdv.CullPrimary,
                tdv.CullPrimaryDead,
                tdv.HiddenPrimary,
                tdv.HiddenPrimaryDead,
                tdv.TreeGrade,
                tdv.TreeGradeDead,
                tdv.CullSecondary,
                tdv.HiddenSecondary,
                tdv.Recoverable,
                tdv.MerchHeightLogLength,
                tdv.MerchHeightType,
                tdv.FormClass,
                tdv.BarkThicknessRatio,
                tdv.AverageZ,
                tdv.ReferenceHeightPercent,
                DeviceID,
            });
        }

        public IEnumerable<TreeDefaultValue> GetTreeDefaultValues()
        {
            return Database.From<TreeDefaultValue>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void DeleteTreeDefaultValue(TreeDefaultValue tdv)
        {
            Database.Execute2("DELETE FROM TreeDefaultValue WHERE CruiseID = @CruiseID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '')",
                new
                {
                    CruiseID,
                    tdv.SpeciesCode,
                    tdv.PrimaryProduct,
                });
        }

        public void UpsertTreeDefaultValue(TreeDefaultValue tdv)
        {
            var changes = Database.Execute2(
@"INSERT INTO TreeDefaultValue (
    CruiseID,
    SpeciesCode,
    PrimaryProduct,
    CullPrimary,
    CullPrimaryDead,
    HiddenPrimary,
    HiddenPrimaryDead,
    TreeGrade,
    TreeGradeDead,
    CullSecondary,
    HiddenSecondary,
    Recoverable,
    MerchHeightLogLength,
    MerchHeightType,
    FormClass,
    BarkThicknessRatio,
    AverageZ,
    ReferenceHeightPercent,
    CreatedBy
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @PrimaryProduct,
    @CullPrimary,
    @CullPrimaryDead,
    @HiddenPrimary,
    @HiddenPrimaryDead,
    @TreeGrade,
    @TreeGradeDead,
    @CullSecondary,
    @HiddenSecondary,
    @Recoverable,
    @MerchHeightLogLength,
    @MerchHeightType,
    @FormClass,
    @BarkThicknessRatio,
    @AverageZ,
    @ReferenceHeightPercent,
    @DeviceID)
ON CONFLICT (CruiseID, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(PrimaryProduct, '') COLLATE NOCASE) DO
UPDATE SET
    CullPrimary = @CullPrimary,
    CullPrimaryDead = @CullPrimaryDead,
    HiddenPrimary = @HiddenPrimary,
    HiddenPrimaryDead = @HiddenPrimaryDead,
    TreeGrade = @TreeGrade,
    TreeGradeDead = @TreeGradeDead,
    CullSecondary = @CullSecondary,
    HiddenSecondary = @HiddenSecondary,
    Recoverable = @Recoverable,
    MerchHeightLogLength = @MerchHeightLogLength,
    MerchHeightType = @MerchHeightType,
    FormClass = @FormClass,
    BarkThicknessRatio = @BarkThicknessRatio,
    AverageZ = @AverageZ,
    ReferenceHeightPercent = @ReferenceHeightPercent,
    CreatedBy = @DeviceID
WHERE CruiseID = @CruiseID
AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '')
AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '');",
            new
            {
                CruiseID,
                tdv.SpeciesCode,
                tdv.PrimaryProduct,
                tdv.CullPrimary,
                tdv.CullPrimaryDead,
                tdv.HiddenPrimary,
                tdv.HiddenPrimaryDead,
                tdv.TreeGrade,
                tdv.TreeGradeDead,
                tdv.CullSecondary,
                tdv.HiddenSecondary,
                tdv.Recoverable,
                tdv.MerchHeightLogLength,
                tdv.MerchHeightType,
                tdv.FormClass,
                tdv.BarkThicknessRatio,
                tdv.AverageZ,
                tdv.ReferenceHeightPercent,
                DeviceID,
            });
            if (changes == 0) { throw new Exception("Expected changes to be greater than 0"); }
        }

        #endregion tree default value

        #region Tree Field

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
ORDER BY tf.Field;", CruiseID).ToArray();
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

        #endregion Tree Field

        #region LogFields

        public IEnumerable<LogField> GetLogFields()
        {
            return Database.Query<LogField>(
@"SELECT
    lf.Field,
    lfh.Heading,
    lf.DefaultHeading,
    DbType
FROM LogField AS lf
LEFT JOIN LogFieldHeading AS lfh ON lf.Field = lfh.Field AND lfh.CruiseID = @p1;", CruiseID).ToArray();
        }

        public void UpdateLogField(LogField lf)
        {
            if (string.IsNullOrEmpty(lf.Heading))
            {
                Database.Execute2("DELETE FROM LogFieldHeading WHERE CruiseID = @CruiseID AND Field = @Field;",
                    new { CruiseID, lf.Field });
            }
            else
            {
                Database.Execute2(
@"INSERT INTO LogFieldHeading (
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
                    lf.Field,
                    lf.Heading,
                });
            }
        }

        #endregion LogFields

        #region StratumTemplate

        public IEnumerable<StratumTemplate> GetStratumTemplates()
        {
            return Database
                .From<StratumTemplate>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void UpsertStratumTemplate(StratumTemplate st)
        {
            Database.Execute2(
@"INSERT INTO StratumTemplate (
    StratumTemplateName,
    CruiseID,
    StratumCode,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    FixCNTField
) VALUES (
    @StratumTemplateName,
    @CruiseID,
    @StratumCode,
    @Method,
    @BasalAreaFactor,
    @FixedPlotSize,
    @KZ3PPNT,
    @SamplingFrequency,
    @Hotkey,
    @FBSCode,
    @YieldComponent,
    @FixCNTField
) ON CONFLICT (StratumTemplateName, CruiseID) DO
UPDATE SET
    StratumCode = @StratumCode,
    Method = @Method,
    BasalAreaFactor = @BasalAreaFactor,
    FixedPlotSize = @FixedPlotSize,
    KZ3PPNT = @KZ3PPNT,
    SamplingFrequency = @SamplingFrequency,
    Hotkey = @Hotkey,
    FBSCode = @FBSCode,
    YieldComponent = @YieldComponent,
    FixCNTField = @FixCNTField
WHERE StratumTemplateName = @StratumTemplateName AND CruiseID = @CruiseID;",
            new
            {
                CruiseID,
                st.StratumTemplateName,
                st.StratumCode,
                st.Method,
                st.BasalAreaFactor,
                st.FixedPlotSize,
                st.KZ3PPNT,
                st.SamplingFrequency,
                st.Hotkey,
                st.FBSCode,
                st.YieldComponent,
                st.FixCNTField,
            });
        }

        public void DeleteStratumTemplate(StratumTemplate st)
        {
            Database.Execute("DELETE FROM StratumTemplate WHERE StratumTemplateName = @p1 AND CruiseID = @p2;", st.StratumTemplateName, CruiseID);
        }

        #endregion StratumTemplate

        #region StratumTemplateTreeFieldSetup

        public IEnumerable<StratumTemplateTreeFieldSetup> GetStratumTemplateTreeFieldSetups(string stratumTemplateName)
        {
            return Database.From<StratumTemplateTreeFieldSetup>()
                .Where("StratumTemplateName = @p1 AND CruiseID = @p2")
                .Query(stratumTemplateName, CruiseID).ToArray();
        }

        public void UpsertStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs)
        {
            Database.Execute2(
@"INSERT INTO StratumTemplateTreeFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
) VALUES (
    @StratumTemplateName,
    @CruiseID,
    @Field,
    @FieldOrder,
    @IsHidden,
    @IsLocked,
    @DefaultValueInt,
    @DefaultValueReal,
    @DefaultValueBool,
    @DefaultValueText
) ON CONFLICT (StratumTemplateName, CruiseID, Field) DO
UPDATE SET
    FieldOrder = @FieldOrder,
    IsHidden = @IsHidden,
    IsLocked = @IsLocked,
    DefaultValueInt = @DefaultValueInt,
    DefaultValueReal = @DefaultValueReal,
    DefaultValueBool = @DefaultValueBool,
    DefaultValueText = @DefaultValueText
WHERE StratumTemplateName = @StratumTemplateName AND CruiseID = @CruiseID AND Field = @Field;",
            new
            {
                stfs.StratumTemplateName,
                CruiseID,
                stfs.Field,
                stfs.FieldOrder,
                stfs.IsHidden,
                stfs.IsLocked,
                stfs.DefaultValueInt,
                stfs.DefaultValueReal,
                stfs.DefaultValueText,
                stfs.DefaultValueBool,
            });
        }

        public void DeleteStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs)
        {
            Database.Execute("DELETE FROM StratumTemplateTreeFieldSetup StratumTemplateName = @p1 AND CruiseID = @p2 AND Field = @p3;", stfs.StratumTemplateName, CruiseID, stfs.Field);
        }

        #endregion StratumTemplateTreeFieldSetup

        #region StratumTemplateLogFieldSetup

        public IEnumerable<StratumTemplateLogFieldSetup> GetStratumTemplateLogFieldSetup(string stratumTemplateName)
        {
            return Database.From<StratumTemplateLogFieldSetup>()
                .Where("StratumTemplateName = @p1 AND CruiseID = @p2")
                .Query(stratumTemplateName, CruiseID).ToArray();
        }

        public void UpsertStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs)
        {
            Database.Execute2(
@"INSERT INTO StratumTemplateLogFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder
) VALUES (
    @StratumTemplateName,
    @CruiseID,
    @Field,
    @FieldOrder
) ON CONFLICT (StratumTemplateName, CruiseID, Field) DO
UPDATE SET
        FieldOrder = @FieldOrder
WHERE StratumTemplateName = @StratumTemplateName AND CruiseID = @CruiseID AND Field = @Field;",
            new
            {
                stlfs.StratumTemplateName,
                CruiseID,
                stlfs.Field,
                stlfs.FieldOrder,
            });
        }

        public void DeleteStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs)
        {
            Database.Execute("DELETE FROM StratumTemplateLogFieldSetup WHERE StratumTemplateName = @p1 AND CruiseID = @p2 AND Field = @p3;"
                , stlfs.StratumTemplateName, CruiseID, stlfs.Field);
        }

        #endregion StratumTemplateLogFieldSetup

        #region Reports

        public IEnumerable<Reports> GetReports()
        {
            return Database.From<Reports>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void AddReport(Reports report)
        {
            Database.Execute2(
@"INSERT INTO Reports (
    ReportID,
    CruiseID,
    Selected,
    Title
) VALUES (
    @ReportID,
    @CruiseID,
    @Selected,
    @Title
);", new { report.ReportID, CruiseID, report.Selected, report.Title });
        }

        public void UpsertReport(Reports report)
        {
            Database.Execute2(
@"INSERT INTO Report (
    ReportID,
    CruiseID,
    Selected,
    Title
) VALUES (
    @ReportID,
    @CruiseID,
    @Selected,
    @Title
)
ON CONFLICT (ReportID, CruiseID) DO
UPDATE SET
    Selected = @Selected,
    Title = @Title
WHERE ReportID = @ReportID AND CruiseID = @CruiseID;",
            new
            {
                report.ReportID,
                CruiseID,
                report.Selected,
                report.Title
            });
        }

        #endregion Reports

        #region VolumeEquation

        public IEnumerable<VolumeEquation> GetVolumeEquations()
        {
            return Database.From<VolumeEquation>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public void AddVolumeEquation(VolumeEquation ve)
        {
            Database.Execute2(
@"INSERT INTO VolumeEquation (
    CruiseID,
    Species,
    PrimaryProduct,
    VolumeEquationNumber,
    StumpHeight,
    TopDIBPrimary,
    TopDIBSecondary,
    CalcTotal,
    CalcBoard,
    CalcCubic,
    CalcCord,
    CalcTopwood,
    CalcBiomass,
    Trim,
    SegmentationLogic,
    MinLogLengthPrimary,
    MaxLogLengthPrimary,
    MinLogLengthSecondary,
    MaxLogLengthSecondary,
    MinMerchLength,
    Model,
    CommonSpeciesName,
    MerchModFlag,
    EvenOddSegment,
    CreatedBy
) VALUES (
    @CruiseID,
    @Species,
    @PrimaryProduct,
    @VolumeEquationNumber,
    @StumpHeight,
    @TopDIBPrimary,
    @TopDIBSecondary,
    @CalcTotal,
    @CalcBoard,
    @CalcCubic,
    @CalcCord,
    @CalcTopwood,
    @CalcBiomass,
    @Trim,
    @SegmentationLogic,
    @MinLogLengthPrimary,
    @MaxLogLengthPrimary,
    @MinLogLengthSecondary,
    @MaxLogLengthSecondary,
    @MinMerchLength,
    @Model,
    @CommonSpeciesName,
    @MerchModFlag,
    @EvenOddSegment,
    @DeviceID
);",
            new
            {
                CruiseID,
                ve.Species,
                ve.PrimaryProduct,
                ve.VolumeEquationNumber,
                ve.StumpHeight,
                ve.TopDIBPrimary,
                ve.TopDIBSecondary,
                ve.CalcTotal,
                ve.CalcBoard,
                ve.CalcCubic,
                ve.CalcCord,
                ve.CalcTopwood,
                ve.CalcBiomass,
                ve.Trim,
                ve.SegmentationLogic,
                ve.MinLogLengthPrimary,
                ve.MaxLogLengthPrimary,
                ve.MinLogLengthSecondary,
                ve.MaxLogLengthSecondary,
                ve.MinMerchLength,
                ve.Model,
                ve.CommonSpeciesName,
                ve.MerchModFlag,
                ve.EvenOddSegment,
                DeviceID,
            });
        }

        public void UpsertVolumeEquation(VolumeEquation ve)
        {
            Database.Execute2(
@"INSERT INTO VolumeEquation (
    CruiseID,
    Species,
    PrimaryProduct,
    VolumeEquationNumber,
    StumpHeight,
    TopDIBPrimary,
    TopDIBSecondary,
    CalcTotal,
    CalcBoard,
    CalcCubic,
    CalcCord,
    CalcTopwood,
    CalcBiomass,
    Trim,
    SegmentationLogic,
    MinLogLengthPrimary,
    MaxLogLengthPrimary,
    MinLogLengthSecondary,
    MaxLogLengthSecondary,
    MinMerchLength,
    Model,
    CommonSpeciesName,
    MerchModFlag,
    EvenOddSegment,
    CreatedBy
) VALUES (
    @CruiseID,
    @Species,
    @PrimaryProduct,
    @VolumeEquationNumber,
    @StumpHeight,
    @TopDIBPrimary,
    @TopDIBSecondary,
    @CalcTotal,
    @CalcBoard,
    @CalcCubic,
    @CalcCord,
    @CalcTopwood,
    @CalcBiomass,
    @Trim,
    @SegmentationLogic,
    @MinLogLengthPrimary,
    @MaxLogLengthPrimary,
    @MinLogLengthSecondary,
    @MaxLogLengthSecondary,
    @MinMerchLength,
    @Model,
    @CommonSpeciesName,
    @MerchModFlag,
    @EvenOddSegment,
    @DeviceID
)
ON CONFLICT (CruiseID, Species, PrimaryProduct, VolumeEquationNumber) DO
UPDATE SET
    StumpHeight = @StumpHeight,
    TopDIBPrimary = @TopDIBPrimary,
    TopDIBSecondary = @TopDIBSecondary,
    CalcTotal = @CalcTotal,
    CalcBoard = @CalcBoard,
    CalcCubic = @CalcCubic,
    CalcCord = @CalcCord,
    CalcTopwood = @CalcTopwood,
    CalcBiomass = @CalcBiomass,
    Trim = @Trim,
    SegmentationLogic = @SegmentationLogic,
    MinLogLengthPrimary = @MinLogLengthPrimary,
    MaxLogLengthPrimary = @MaxLogLengthPrimary,
    MinLogLengthSecondary = @MinLogLengthSecondary,
    MaxLogLengthSecondary = @MaxLogLengthSecondary,
    MinMerchLength = @MinMerchLength,
    Model = @Model,
    CommonSpeciesName = @CommonSpeciesName,
    MerchModFlag = @MerchModFlag,
    EvenOddSegment =  @EvenOddSegment,
    ModifiedBy = @DeviceID
WHERE CruiseID = @CruiseID
    AND Species = @Species
    AND PrimaryProduct = @PrimaryProduct
    AND VolumeEquationNumber = @VolumeEquationNumber;",
            new
            {
                CruiseID,
                ve.Species,
                ve.PrimaryProduct,
                ve.VolumeEquationNumber,
                ve.StumpHeight,
                ve.TopDIBPrimary,
                ve.TopDIBSecondary,
                ve.CalcTotal,
                ve.CalcBoard,
                ve.CalcCubic,
                ve.CalcCord,
                ve.CalcTopwood,
                ve.CalcBiomass,
                ve.Trim,
                ve.SegmentationLogic,
                ve.MinLogLengthPrimary,
                ve.MaxLogLengthPrimary,
                ve.MinLogLengthSecondary,
                ve.MaxLogLengthSecondary,
                ve.MinMerchLength,
                ve.Model,
                ve.CommonSpeciesName,
                ve.MerchModFlag,
                ve.EvenOddSegment,
                DeviceID,
            });
        }

        #endregion VolumeEquation
    }
}