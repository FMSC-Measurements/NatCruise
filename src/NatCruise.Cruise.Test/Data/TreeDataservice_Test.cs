using NatCruise.Cruise.Data;
using NatCruise.Cruise.Services;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class TreeDataservice_Test : TestBase
    {
        public TreeDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTreeFieldValues()
        {
            var initializer = new DatastoreInitializer();

            using var db = initializer.CreateDatabase();

            var dataservice = new CuttingUnitDatastore(db, initializer.CruiseID, initializer.DeviceID, new SamplerInfoDataservice(db, initializer.CruiseID, initializer.DeviceID));

            var treeID = Guid.NewGuid().ToString();
            dataservice.GetTreeFieldValues(treeID);

        }
    }
}
