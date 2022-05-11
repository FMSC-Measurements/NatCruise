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
            using (var database = init.CreateDatabase())
            {
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
        }
    }
}
