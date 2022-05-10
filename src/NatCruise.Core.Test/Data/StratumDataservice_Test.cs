using FluentAssertions;
using NatCruise.Data;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NatCruise.Test.Data
{
    public class StratumDataservice_Test
    {
        [Theory]
        [InlineData("FIX")]
        [InlineData("PCM")]
        [InlineData("FCM")]
        [InlineData("FIXCNT")]
        [InlineData("F3P")]
        [InlineData("P3P")]
        public void GetPlotStrata(string method)
        {
            var init = new DatastoreInitializer()
            {
                Strata = null,
                UnitStrata = null,
                SampleGroups = null,
                Species = null,
                Subpops = null,
            };
            var unit = init.Units.First();
            var newStratumCode = "01";

            using (var database = init.CreateDatabase())
            {
                var datastore = new StratumDataservice(database, init.CruiseID, init.DeviceID);

                datastore.GetPlotStrata(unit).Should().HaveCount(0);

                var stratumID = Guid.NewGuid().ToString();
                database.Execute($"INSERT INTO Stratum (CruiseID, StratumID, StratumCode, Method) VALUES ('{init.CruiseID}', '{stratumID}', '{newStratumCode}', '{method}');");
                database.Execute($"INSERT INTO CuttingUnit_Stratum (CruiseID, CuttingUnitCode, StratumCode) VALUES ('{init.CruiseID}', '{unit}', '{newStratumCode}')");

                var results = datastore.GetPlotStrata(unit).ToArray();

                results.Should().HaveCount(1);
            }
        }
    }
}
