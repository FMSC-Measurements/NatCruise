﻿using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class SampleGroupDataservice : CruiseDataserviceBase, ISampleGroupDataservice
    {
        public SampleGroupDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public SampleGroupDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SampleGroupDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public const string SELECT_SAMPLEGROUP_CORE =
@"SELECT
    sg.*,
    st.Method AS CruiseMethod,
    (
        EXISTS ( SELECT * FROM Tree WHERE CruiseID = sg.CruiseID AND StratumCode = sg.StratumCode AND SampleGroupCode = sg.SampleGroupCode)
    ) AS HasTrees
FROM SampleGroup AS sg
JOIN Stratum AS st USING (StratumCode, CruiseID)
";

        public void AddSampleGroup(SampleGroup sg)
        {
            sg.SampleGroupID ??= Guid.NewGuid().ToString();

            Database.Execute2(
@"INSERT INTO SampleGroup (
    CruiseID,
    SampleGroupID,
    SampleGroupCode,
    StratumCode,
    CutLeave,
    UOM,
    PrimaryProduct,
    SecondaryProduct,
    BiomassProduct,
    DefaultLiveDead,
    SamplingFrequency,
    InsuranceFrequency,
    KZ,
    BigBAF,
    TallyBySubPop,
    UseExternalSampler,
    SampleSelectorType,
    Description,
    MinKPI,
    MaxKPI,
    SmallFPS,
    CreatedBy
) VALUES (
    @CruiseID,
    @SampleGroupID,
    @SampleGroupCode,
    @StratumCode,
    @CutLeave,
    @UOM,
    @PrimaryProduct,
    @SecondaryProduct,
    @BiomassProduct,
    @DefaultLiveDead,
    @SamplingFrequency,
    @InsuranceFrequency,
    @KZ,
    @BigBAF,
    @TallyBySubPop,
    @UseExternalSampler,
    @SampleSelectorType,
    @Description,
    @MinKPI,
    @MaxKPI,
    @SmallFPS,
    @DeviceID
);", new
{
    CruiseID,
    sg.SampleGroupID,
    sg.SampleGroupCode,
    sg.StratumCode,
    sg.CutLeave,
    sg.UOM,
    sg.PrimaryProduct,
    sg.SecondaryProduct,
    sg.BiomassProduct,
    sg.DefaultLiveDead,
    sg.SamplingFrequency,
    sg.InsuranceFrequency,
    sg.KZ,
    sg.BigBAF,
    sg.TallyBySubPop,
    sg.UseExternalSampler,
    sg.SampleSelectorType,
    sg.Description,
    sg.MinKPI,
    sg.MaxKPI,
    sg.SmallFPS,
    DeviceID,
});
        }

        public void DeleteSampleGroup(SampleGroup sampleGroup)
        {
            Database.Execute("DELETE FROM SampleGroup WHERE SampleGroupID = @p1;", sampleGroup.SampleGroupID);
        }

        public SampleGroup GetSampleGroup(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SampleGroup>(SELECT_SAMPLEGROUP_CORE +
                "WHERE sg.StratumCode = @p1 AND sg.SampleGroupCode = @p2 AND sg.CruiseID = @p3;", stratumCode, sampleGroupCode, CruiseID).FirstOrDefault();
        }

        public IEnumerable<SampleGroup> GetSampleGroups(string stratumCode = null)
        {
            return Database.Query<SampleGroup>(SELECT_SAMPLEGROUP_CORE +
                "WHERE (@p1 IS NULL OR sg.StratumCode = @p1) AND sg.CruiseID = @p2;", stratumCode, CruiseID);
        }

        public IEnumerable<string> GetSampleGroupCodes(string stratumCode = null)
        {
            return Database.QueryScalar<string>("SELECT SampleGroupCode FROM SampleGroup " +
                "WHERE (@p1 IS NULL OR StratumCode = @p1) AND CruiseID = @p2;", stratumCode, CruiseID);
        }

        public void UpdateSampleGroup(SampleGroup sg)
        {
            Database.Execute2(
@"UPDATE SampleGroup SET
    Description = @Description,
    CutLeave = @CutLeave,
    UOM = @UOM,
    PrimaryProduct = @PrimaryProduct,
    SecondaryProduct = @SecondaryProduct,
    BiomassProduct = @BiomassProduct,
    DefaultLiveDead = @DefaultLiveDead,
    SamplingFrequency = @SamplingFrequency,
    InsuranceFrequency = @InsuranceFrequency,
    KZ = @KZ,
    BigBAF = @BigBAF,
    TallyBySubPop = @TallyBySubPop,
    SampleSelectorType = @SampleSelectorType,
    UseExternalSampler = @UseExternalSampler,
    MinKPI = @MinKPI,
    MaxKPI = @MaxKPI,
    SmallFPS = @SmallFPS,
    ModifiedBy = @DeviceID
WHERE SampleGroupID = @SampleGroupID;",
                new
                {
                    sg.SampleGroupID,
                    sg.Description,
                    sg.CutLeave,
                    sg.UOM,
                    sg.PrimaryProduct,
                    sg.SecondaryProduct,
                    sg.BiomassProduct,
                    sg.DefaultLiveDead,
                    sg.SamplingFrequency,
                    sg.InsuranceFrequency,
                    sg.KZ,
                    sg.BigBAF,
                    sg.TallyBySubPop,
                    sg.SampleSelectorType,
                    sg.UseExternalSampler,
                    sg.MinKPI,
                    sg.MaxKPI,
                    sg.SmallFPS,
                    DeviceID
                });
        }

        public void UpdateSampleGroupCode(SampleGroup sg)
        {
            Database.Execute2(
@"UPDATE SampleGroup SET
    SampleGroupCode = @SampleGroupCode,
    ModifiedBy = @DeviceID
WHERE SampleGroupID = @SampleGroupID;",
                new
                {
                    sg.SampleGroupID,
                    sg.SampleGroupCode,
                    DeviceID
                });
        }
    }
}