using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class TreeDefaultValueDataservice : CruiseDataserviceBase, ITreeDefaultValueDataservice
    {
        public TreeDefaultValueDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

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
    }
}
