using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public class SampleGroupDataservice : CruiseDataserviceBase, ISampleGroupDataservice
    {
        public SampleGroupDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SampleGroupDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void AddSampleGroup(SampleGroup sampleGroup)
        {
            sampleGroup.SampleGroupID ??= Guid.NewGuid().ToString();

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
);",        new
            {
                CruiseID,
                sampleGroup.SampleGroupID,
                sampleGroup.SampleGroupCode,
                sampleGroup.StratumCode,
                sampleGroup.CutLeave,
                sampleGroup.UOM,
                sampleGroup.PrimaryProduct,
                sampleGroup.SecondaryProduct,
                sampleGroup.BiomassProduct,
                sampleGroup.DefaultLiveDead,
                sampleGroup.SamplingFrequency,
                sampleGroup.InsuranceFrequency,
                sampleGroup.KZ,
                sampleGroup.BigBAF,
                sampleGroup.TallyBySubPop,
                sampleGroup.UseExternalSampler,
                sampleGroup.SampleSelectorType,
                sampleGroup.Description,
                sampleGroup.MinKPI,
                sampleGroup.MaxKPI,
                sampleGroup.SmallFPS,
                DeviceID,
            });
        }

        public void DeleteSampleGroup(SampleGroup sampleGroup)
        {
            Database.Execute("DELETE FROM SampleGroup WHERE SampleGroupID = @p1;", sampleGroup.SampleGroupID);
        }

        public string GetMethod(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT Method FROM Stratum WHERE StratumCode = @p1 AND CruiseID = @p2;", stratumCode, CruiseID);
        }

        public SampleGroup GetSampleGroup(string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SampleGroup> GetSampleGroups()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SampleGroup> GetSampleGroups(string stratumCode)
        {
            return Database.Query<SampleGroup>("SELECT sg.*, st.Method AS CruiseMethod FROM SampleGroup AS sg JOIN Stratum AS st USING (StratumCode, CruiseID) WHERE sg.StratumCode = @p1 AND sg.CruiseID = @p2;", stratumCode, CruiseID);
        }

        public void SetTallyBySubPop(bool tallyBySubpop, string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
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
    }
}