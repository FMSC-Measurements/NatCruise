using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public interface ISampleGroupDataservice : IDataservice
    {
        IEnumerable<SampleGroup> GetSampleGroups();

        IEnumerable<SampleGroup> GetSampleGroups(string stratumCode);

        SampleGroup GetSampleGroup(string stratumCode, string sampleGroupCode);

        void SetTallyBySubPop(bool tallyBySubpop, string stratumCode, string sampleGroupCode);

        string GetMethod(string stratumCode);

        void AddSampleGroup(SampleGroup sampleGroup);

        void UpdateSampleGroup(SampleGroup sampleGroup);

        void UpdateSampleGroupCode(SampleGroup sg);

        void DeleteSampleGroup(SampleGroup sampleGroup);
    }
}