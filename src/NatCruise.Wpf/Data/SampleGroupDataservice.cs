using CruiseDAL;
using NatCruise.Wpf.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public class SampleGroupDataservice : ISampleGroupDataservice
    {
        public CruiseDatastore Database { get; }

        public SampleGroupDataservice(string path)
        {
            Database = new CruiseDatastore(path);
        }

        public void AddSampleGroup(SampleGroup sampleGroup)
        {
            Database.Insert(sampleGroup);
        }

        public void DeleteSampleGroup(SampleGroup sampleGroup)
        {
            Database.Execute("DELETE FROM SampleGroup_V3 WHERE SampleGroup_CN = @p1;", sampleGroup.SampleGroup_CN);
        }

        public string GetMethod(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT Method FROM Stratum WHERE Code = @p1;", stratumCode);
        }

        public IEnumerable<Product> GetProducts()
        {
            return new Product[0];
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
            return Database.Query<SampleGroup>("SELECT sg.* FROM SampleGroup_V3 AS sg WHERE StratumCode = @p1;", stratumCode);
        }

        public void SetTallyBySubPop(bool tallyBySubpop, string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
        }

        public void UpdateSampleGroup(SampleGroup sampleGroup)
        {
            Database.Update(sampleGroup);
        }
    }
}