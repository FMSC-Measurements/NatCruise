using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ISampleGroupDataservice : IDataservice
    {
        public IEnumerable<string> GetSampleGroupCodes(string stratumCode);

        IEnumerable<SampleGroup> GetSampleGroups();

        IEnumerable<SampleGroup> GetSampleGroups(string stratumCode);

        SampleGroup GetSampleGroup(string stratumCode, string sampleGroupCode);

        void SetTallyBySubPop(bool tallyBySubpop, string stratumCode, string sampleGroupCode);

        string GetMethod(string stratumCode);

        void AddSampleGroup(SampleGroup sg);

        void UpdateSampleGroup(SampleGroup sg);

        void UpdateSampleGroupCode(SampleGroup sg);

        void DeleteSampleGroup(SampleGroup sampleGroup);
    }
}