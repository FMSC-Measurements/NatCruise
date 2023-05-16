using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public class StratumTemplateDataservice : CruiseDataserviceBase,IStratumTemplateDataservice
    {
        public StratumTemplateDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public StratumTemplateDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

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
            Database.Execute("DELETE FROM StratumTemplateTreeFieldSetup WHERE StratumTemplateName = @p1 AND CruiseID = @p2 AND Field = @p3;", stfs.StratumTemplateName, CruiseID, stfs.Field);
        }

        #endregion StratumTemplateTreeFieldSetup

        #region StratumTemplateLogFieldSetup

        public IEnumerable<StratumTemplateLogFieldSetup> GetStratumTemplateLogFieldSetups(string stratumTemplateName)
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
    }
}
