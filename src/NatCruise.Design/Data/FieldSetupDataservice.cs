using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class FieldSetupDataservice : CruiseDataserviceBase, IFieldSetupDataservice
    {
        public FieldSetupDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public FieldSetupDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        protected TreeField GetTreeField(string field)
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
WHERE tF.Field = @p2;", CruiseID, field).FirstOrDefault();
        }

        public IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode)
        {
            var fieldSetups = Database.From<CruiseDAL.V3.Models.TreeFieldSetup>()
                .Where("StratumCode = @p1 AND SampleGroupCode IS NULL AND CruiseID = @p2")
                .OrderBy("FieldOrder")
                .Query(stratumCode, CruiseID).Select(x =>
                {
                    return new TreeFieldSetup()
                    {
                        StratumCode = x.StratumCode,
                        SampleGroupCode = x.SampleGroupCode,
                        Field = GetTreeField(x.Field),
                        FieldOrder = x.FieldOrder,
                        IsHidden = x.IsHidden.GetValueOrDefault(),
                        IsLocked = x.IsLocked.GetValueOrDefault(),
                        DefaultValueInt = x.DefaultValueInt,
                        DefaultValueReal = x.DefaultValueReal,
                        DefaultValueText = x.DefaultValueText,
                        DefaultValueBool = x.DefaultValueBool,
                    };
                }).ToArray();

            return fieldSetups;
        }

        public IEnumerable<TreeFieldSetup> GetTreeFieldSetups(string stratumCode, string sampleGroupCode)
        {
            var fieldSetups = Database.From<CruiseDAL.V3.Models.TreeFieldSetup>()
                .Where("StratumCode = @p1 AND SampleGroupCode = @p2 AND CruiseID = @p3")
                .OrderBy("FieldOrder")
                .Query(stratumCode, sampleGroupCode, CruiseID).Select(x =>
                {
                    return new TreeFieldSetup()
                    {
                        StratumCode = x.StratumCode,
                        SampleGroupCode = x.SampleGroupCode,
                        Field = GetTreeField(x.Field),
                        FieldOrder = x.FieldOrder,
                        IsHidden = x.IsHidden.GetValueOrDefault(),
                        IsLocked = x.IsLocked.GetValueOrDefault(),
                        DefaultValueInt = x.DefaultValueInt,
                        DefaultValueReal = x.DefaultValueReal,
                        DefaultValueText = x.DefaultValueText,
                        DefaultValueBool = x.DefaultValueBool,
                    };
                }).ToArray();

            return fieldSetups;
        }

        public void UpsertTreeFieldSetup(TreeFieldSetup tfs)
        {
            Database.Execute2(
@"INSERT INTO TreeFieldSetup (
    CruiseID,
    StratumCode,
    SampleGroupCode,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
) VALUES (
    @CruiseID,
    @StratumCode,
    @SampleGroupCode,
    @Field,
    @FieldOrder,
    @IsHidden,
    @IsLocked,
    @DefaultValueInt,
    @DefaultValueReal,
    @DefaultValueBool,
    @DefaultValueText
)
ON CONFLICT (CruiseID, StratumCode, ifnull(SampleGroupCode, ''), Field) DO
UPDATE SET
    FieldOrder = @FieldOrder,
    IsHidden = @IsHidden,
    IsLocked = @IsLocked,
    DefaultValueInt = @DefaultValueInt,
    DefaultValueReal = @DefaultValueReal,
    DefaultValueBool = @DefaultValueBool,
    DefaultValueText = @DefaultValueText
WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND ifnull(SampleGroupCode, '') = ifnull(@SampleGroupCode, '') AND Field = @Field;",
            new
            {
                CruiseID,
                tfs.StratumCode,
                tfs.SampleGroupCode,
                tfs.Field.Field,
                tfs.FieldOrder,
                tfs.IsHidden,
                tfs.IsLocked,
                tfs.DefaultValueInt,
                tfs.DefaultValueReal,
                tfs.DefaultValueText,
                tfs.DefaultValueBool
            });
        }

        public void DeleteTreeFieldSetup(TreeFieldSetup tfs)
        {
            if (tfs is null) { throw new ArgumentNullException(nameof(tfs)); }
            if (tfs.Field is null) { throw new ArgumentNullException(nameof(TreeField.Field)); }

            Database.Execute("DELETE FROM TreeFieldSetup WHERE CruiseID = @p1 AND StratumCode = @p2 AND ifnull(SampleGroupCode, '') = ifnull(@p3, '') AND Field = @p4;",
                CruiseID, tfs.StratumCode, tfs.SampleGroupCode, tfs.Field.Field);
        }

        public void SetTreeFieldsFromStratumTemplate(string stratumCode, string stratumTemplateName)
        {
            Database.Execute2(
@"BEGIN;
Delete FROM TreeFieldSetup WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode IS  NULL;

INSERT INTO TreeFieldSetup (
    CruiseID,
    StratumCode,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
)
SELECT
    @CruiseID,
    @StratumCode,
    sttfs.Field,
    sttfs.FieldOrder,
    sttfs.IsHidden,
    sttfs.IsLocked,
    sttfs.DefaultValueInt,
    sttfs.DefaultValueReal,
    sttfs.DefaultValueBool,
    sttfs.DefaultValueText
FROM StratumTemplateTreeFieldSetup AS sttfs
WHERE StratumTemplateName = @StratumTemplateName AND CruiseID = @CruiseID;

COMMIT;",
                new
                {
                    CruiseID,
                    StratumCode = stratumCode,
                    StratumTemplateName = stratumTemplateName,
                });
        }

        public IEnumerable<LogFieldSetup> GetLogFieldSetups(string stratumCode)
        {
            return Database.From<LogFieldSetup>()
                .Where("CruiseID = @p1 AND StratumCode = @p2")
                .OrderBy("FieldOrder")
                .Query(CruiseID, stratumCode)
                .ToArray();
        }

        public void UpsertLogFieldSetup(LogFieldSetup lfs)
        {
            if (lfs is null) { throw new ArgumentNullException(nameof(lfs)); }

            Database.Execute2(
@"INSERT INTO LogFieldSetup (
    CruiseID,
    StratumCode,
    Field,
    FieldOrder
) VALUES (
    @CruiseID,
    @StratumCode,
    @Field,
    @FieldOrder
) ON CONFLICT (CruiseID, StratumCode, Field) DO
UPDATE SET
    Field = @Field,
    FieldOrder = @FieldOrder
WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field;",
            new
            {
                CruiseID,
                lfs.StratumCode,
                lfs.Field,
                lfs.FieldOrder,
            });
        }

        public void DeleteLogFieldSetup(LogFieldSetup lfs)
        {
            if (lfs is null) { throw new ArgumentNullException(nameof(lfs)); }

            Database.Execute2("DELETE FROM LogFieldSetup WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field",
                new
                {
                    CruiseID,
                    lfs.StratumCode,
                    lfs.Field,
                });
        }

        public void SetLogFieldsFromStratumTemplate(string stratumCode, string stratumTemplateName)
        {
            Database.Execute2(
@"BEGIN;
Delete FROM LogFieldSetup WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode;

INSERT INTO LogFieldSetup (
    CruiseID,
    StratumCode,
    Field,
    FieldOrder
)
SELECT
    @CruiseID,
    @StratumCode,
    stlfs.Field,
    stlfs.FieldOrder
FROM StratumTemplateLogFieldSetup AS stlfs
WHERE StratumTemplateName = @StratumTemplateName AND CruiseID = @CruiseID;

COMMIT;",
                new
                {
                    CruiseID,
                    StratumCode = stratumCode,
                    StratumTemplateName = stratumTemplateName,
                });
        }
    }
}