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
    public class TreeErrorDataService_Test : TestBase
    {
        public TreeErrorDataService_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTreeError_SpeciesMissing()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = (string)null;
            var liveDead = "L";
            //var countMeasure = "M";
            var treeCount = 1;

            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var treeDs = new TreeDataservice(database, init.CruiseID, init.DeviceID);
                var treeErrorDs = new TreeErrorDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = treeDs.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var treeErrors = treeErrorDs.GetTreeErrors(treeID).ToArray();

                treeErrors.Should().HaveCount(3);

                var speciesError = treeErrors.First();
                speciesError.Level.Should().Be("E");
                speciesError.Field.Should().Be(nameof(Models.Tree.SpeciesCode));
            }
        }
    }
}
