using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISampleGroupDataservice : IDataservice
    {
        IEnumerable<string> GetSampleGroupCodes(string stratumCode = null);

        IEnumerable<SampleGroup> GetSampleGroups(string stratumCode = null);

        SampleGroup GetSampleGroup(string stratumCode, string sampleGroupCode);

        void SetTallyBySubPop(bool tallyBySubpop, string stratumCode, string sampleGroupCode);

        string GetMethod(string stratumCode);

        void AddSampleGroup(SampleGroup sg);

        void UpdateSampleGroup(SampleGroup sg);

        void UpdateSampleGroupCode(SampleGroup sg);

        void DeleteSampleGroup(SampleGroup sampleGroup);
    }
}