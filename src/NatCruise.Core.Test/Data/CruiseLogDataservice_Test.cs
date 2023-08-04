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
    public class CruiseLogDataservice_Test : TestBase
    {
        public CruiseLogDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Log_Test()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var ds = new CruiseLogDataservice(db, init.CruiseID, init.DeviceID);

            ds.Log("something");
        }
    }
}
