using FluentAssertions;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class FieldSetupDataservice_LogField_Test : TestBase
    {
        public FieldSetupDataservice_LogField_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetLogFieldSetupsByTreeID()
        {
            var init = new DatastoreInitializer();
            using var database = init.CreateDatabase();

            var stratum = "st1";
            var lfs = new CruiseDAL.V3.Models.LogFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = stratum,
                Field = "Grade",
                FieldOrder = 1,
            };
            database.Insert(lfs);

            var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);
            var fieldSetupDs = new FieldSetupDataservice(database, init.CruiseID, init.DeviceID);

            var tree_guid = treeDS.InsertManualTree("u1", "st1", "sg1");

            var logFields = fieldSetupDs.GetLogFieldSetupsByTreeID(tree_guid);
            logFields.Should().HaveCount(1);

        }

        [Fact]
        public void GetLogFieldSetups_ByStratum()
        {
            var init = new DatastoreInitializer();
            using var database = init.CreateDatabase();

            var stratum = "st1";
            var lfs = new CruiseDAL.V3.Models.LogFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = stratum,
                Field = "Grade",
                FieldOrder = 1,
            };
            database.Insert(lfs);

            var fieldSetupDs = new FieldSetupDataservice(database, init.CruiseID, init.DeviceID);

            var logFields = fieldSetupDs.GetLogFieldSetups(stratum);
            logFields.Should().HaveCount(1);
        }

        [Fact]
        public void GetLogFieldSetupsByCruise()
        {
            var init = new DatastoreInitializer();
            using var database = init.CreateDatabase();

            var lfs = new CruiseDAL.V3.Models.LogFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = "st1",
                Field = "Grade",
                FieldOrder = 1,
            };
            database.Insert(lfs);

            var lfs2 = new CruiseDAL.V3.Models.LogFieldSetup
            {
                CruiseID = init.CruiseID,
                StratumCode = "st2",
                Field = nameof(CruiseDAL.V3.Models.Log.BarkThickness),
                FieldOrder = 1,
            };
            database.Insert(lfs2);

            var fieldSetupDs = new FieldSetupDataservice(database, init.CruiseID, init.DeviceID);

            var logFields = fieldSetupDs.GetLogFieldSetupsByCruise();
            logFields.Should().HaveCount(2);
        }
    }
}
