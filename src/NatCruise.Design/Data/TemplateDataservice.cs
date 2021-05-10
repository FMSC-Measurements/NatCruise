using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

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
        #endregion

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
        #endregion

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
WHERE TreeAuditRuleID = @TreeAuditRule;",
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
        #endregion


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
            if(changes == 0) { throw new Exception("Expected changes to be greater than 0"); }
        }
        #endregion

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
LEFT JOIN TreeFieldHeading AS tfh ON tf.Field = tfh.Field AND tfh.CruiseID = @p1;", CruiseID).ToArray();

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
        #endregion

        #region LogFields
        public IEnumerable<LogField> GetLogFields()
        {
            return Database.Query<LogField>(
@"SELECT
    Field,
    lfh.Heading,
    lf.DefaultHeading,
    DbType
) FROM LogField AS lf
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
        #endregion


        #region StratumDefault
        public IEnumerable<StratumDefault> GetStratumDefaults()
        {
            return Database.From<StratumDefault>().Query().ToArray();
        }

        public IEnumerable<StratumDefault> GetTreeFieldSetupStratumDefaults()
        {
            return Database.From<StratumDefault>()
                .Where("EXISTS (SELECT * FROM TreeFieldSetupDefault AS tfsd WHERE tfsd.StratumDefaultID = StratumDefault.StratumDefaultID AND tfsd.SampleGroupDefaultID IS NULL)")
                .Query().ToArray();
        }

        public void AddStratumDefault(StratumDefault std)
        {
            if (string.IsNullOrEmpty(std.StratumDefaultID)) { throw new ArgumentException("Value was null or empty", nameof(std)); }

            Database.Insert(std);
        }
        #endregion

        #region TreeFieldSetupDefault
        public IEnumerable<TreeFieldSetupDefault> GetTreeFieldSetupDefaults(string stratumDefaultID)
        {
            return Database.From<TreeFieldSetupDefault>()
                .Where("StratumDefaultID =  @p1")
                .Query(stratumDefaultID).ToArray();
        }

        public void AddTreeFieldSetupDefault(TreeFieldSetupDefault tfsd)
        {
            Database.Execute2(
@"INSERT INTO TreeFieldSetupDefault (
    StratumDefaultID,
    SampleGroupDefaultID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
) VALUES (
    @StratumDefaultID,
    @SampleGroupDefaultID,
    @Field,
    @FieldOrder,
    @IsHidden,
    @IsLocked,
    @DefaultValueInt,
    @DefaultValueReal,
    @DefaultValueBool,
    @DefaultValueText
);", tfsd);
        }

        public void UpsertTreeFieldSetupDefault(TreeFieldSetupDefault tfsd)
        {
            Database.Execute2(
@"INSERT INTO TreeFieldSetupDefault (
    StratumDefaultID,
    SampleGroupDefaultID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
) VALUES (
    @StratumDefaultID,
    @SampleGroupDefaultID,
    @Field,
    @FieldOrder,
    @IsHidden,
    @IsLocked,
    @DefaultValueInt,
    @DefaultValueReal,
    @DefaultValueBool,
    @DefaultValueText
)
ON CONFLICT (ifnull(StratumDefaultID, ''), ifnull(SampleGroupDefaultID, ''), Field) DO
UPDATE SET
    FieldOrder = @FieldOrder,
    IsHidden = @IsHidden,
    IsLocked = @IsLocked,
    DefaultValueInt = @DefaultValueInt,
    DefaultValueReal = @DefaultValueReal,
    DefaultValueBool = @DefaultValueBool,
    DefaultValueText = @DefaultValueText
WHERE ifnull(StratumDefaultID, '') = ifnull(@StratumDefaultID, '')
    AND ifnull(SampleGroupDefaultID, '') = ifnull(@SampleGroupDefaultID, '')
    AND Field = @Field;", tfsd);
        }
        #endregion

        #region LogFieldSetupDefault
        public IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults()
        {
            return Database.From<LogFieldSetupDefault>().Query().ToArray();
        }

        public IEnumerable<LogFieldSetupDefault> GetLogFieldSetupDefaults(string stratumDefaultID)
        {
            return Database.From<LogFieldSetupDefault>()
                .Where("StratumDefaultID = @p1").Query(stratumDefaultID).ToArray();
        }

        public void AddLogFieldSetupDefault(LogFieldSetupDefault lfsd)
        {
            Database.Execute2(
@"INSERT INTO LogFieldSetupDefault (
    StratumDefaultID,
    Field,
    FieldOrder
) VALUES (
    @StratumDefaultID,
    @Field,
    @FieldOrder
);", lfsd);
        }

        public void UpsertLogFieldSetupDefault(LogFieldSetupDefault lfsd)
        {
            Database.Execute2(
@"INSERT INTO LogFieldSetupDefault (
    StratumDefaultID,
    Field,
    FieldOrder
) VALUES (
    @StratumDefaultID,
    @Field,
    @FieldOrder
)
ON CONFLICT (StratumDefaultID, Field) DO
UPDATE SET
    FieldOrder = @FieldOrder
WHERE StratumDefaultID = @StratumDefaultID AND Field = @Field;", lfsd);
        }
        #endregion

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
        #endregion

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

        #endregion
    }
}
