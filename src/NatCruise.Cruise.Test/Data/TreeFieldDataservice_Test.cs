using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Test;
using Xunit;

namespace NatCruise.Cruise.Test.Data
{
    public class TreeFieldDataservice_Test
    {
        [Fact]
        public void GetNonPlotTreeFields_Test()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var stCode1 = init.NonPlotStrata[0].StratumCode;
            var stCode2 = init.NonPlotStrata[1].StratumCode;
            var plotStCode = init.PlotStrata[0].StratumCode;

            var treeFieldSetups = new TreeFieldSetup[]
            {
                new TreeFieldSetup{ CruiseID = init.CruiseID, StratumCode = stCode1, Field = "DBH"},
                new TreeFieldSetup{ CruiseID = init.CruiseID, StratumCode = stCode1, Field = "TotalHeight"},
                new TreeFieldSetup{ CruiseID = init.CruiseID, StratumCode = stCode2, Field = "DBH"},
                new TreeFieldSetup{ CruiseID = init.CruiseID, StratumCode = plotStCode, Field = "DBH"},
                new TreeFieldSetup{ CruiseID = init.CruiseID, StratumCode = plotStCode, Field = "DRC"},
            };

            foreach (var tfs in treeFieldSetups)
            { db.Insert(tfs); }

            var dataservice = new TreeFieldDataservice(db, init.CruiseID, init.DeviceID);

            var unitCode = init.Units[0];
            var treeFields = dataservice.GetNonPlotTreeFields(unitCode);
            treeFields.Should().HaveCount(2);
        }
    }
}