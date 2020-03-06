using CruiseDAL;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Design.Data
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
            return new Product[]
            {
                new Product{ ProductCode = "01", FriendlyName="Sawtimber"},
                new Product{ ProductCode = "02", FriendlyName="Pulpwood"},
                new Product{ ProductCode = "03", FriendlyName="Poles"},
                new Product{ ProductCode = "04", FriendlyName="Pilings"},
                new Product{ ProductCode = "05", FriendlyName="Mine Props"},
                new Product{ ProductCode = "06", FriendlyName="Posts"},
                new Product{ ProductCode = "07", FriendlyName="Fuelwood"},
                new Product{ ProductCode = "08", FriendlyName="Non-sawtimber"},
                new Product{ ProductCode = "09", FriendlyName="Ties"},
                new Product{ ProductCode = "10", FriendlyName="Coop Bolts"},
                new Product{ ProductCode = "11", FriendlyName="Acid/Dist."},
                new Product{ ProductCode = "12", FriendlyName="Float Logs"},
                new Product{ ProductCode = "13", FriendlyName="Trap Float"},
                new Product{ ProductCode = "14", FriendlyName="Misc-Conv."},
                new Product{ ProductCode = "15", FriendlyName="Christmas Trees"},
                new Product{ ProductCode = "16", FriendlyName="Nav Stores"},
                new Product{ ProductCode = "17", FriendlyName="Non Conv."},
                new Product{ ProductCode = "18", FriendlyName="Cull Logs"},
                new Product{ ProductCode = "19", FriendlyName="Sm Rnd Wd"},
                new Product{ ProductCode = "20", FriendlyName="Grn Bio Cv"},
                new Product{ ProductCode = "21", FriendlyName="Dry Bio Cv"},
                new Product{ ProductCode = "26", FriendlyName="Sp Wood Pr"},
            };
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