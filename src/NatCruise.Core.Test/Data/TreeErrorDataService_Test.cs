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


        [Fact]
        public void SetTreeAuditResolution()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            //var countMeasure = "M";
            var treeCount = 1;

            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var treeDs = new TreeDataservice(db, init.CruiseID, init.DeviceID);
            var treeErrorDs = new TreeErrorDataservice(db, init.CruiseID, init.DeviceID);
            var treeAuditRuleDs = new TreeAuditRuleDataservice(db, init.CruiseID, init.DeviceID);
            var treeFieldDs = new TreeFieldDataservice(db, init.CruiseID, init.DeviceID);
            var fieldSetupDs = new FieldSetupDataservice(db, init.CruiseID, init.DeviceID);




            // setup tree audit rules
            // we need to add the field to field setup so that it will be audited
            var dbhField = treeFieldDs.GetTreeFields().Single(x => x.Field == "DBH");
            fieldSetupDs.UpsertTreeFieldSetup(new Models.TreeFieldSetup { StratumCode = stratumCode, Field = dbhField, });

            var tar = new Models.TreeAuditRule()
            {
                TreeAuditRuleID = Guid.NewGuid().ToString(),
                Field = "DBH",
                Min = 1,
                Max = 2,
            };
            treeAuditRuleDs.AddTreeAuditRule(tar);

            var tarSels = new[]
            {
                new Models.TreeAuditRuleSelector{ TreeAuditRuleID = tar.TreeAuditRuleID, SpeciesCode = species }
            };

            foreach(var tarSel in tarSels)
            {
                treeAuditRuleDs.AddRuleSelector(tarSel);
            }
            

            
            // add a tree
            var treeID = treeDs.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);


            // validate tree errors
            var treeErrors = treeErrorDs.GetTreeErrors(treeID).ToArray();
            treeErrors.Where(x => x.Level == "W" && !x.IsResolved).Should().HaveCount(1);

            treeErrorDs.SetTreeAuditResolution(treeID, tar.TreeAuditRuleID, "something", "bc");

            // validate tree errors again
            var treeErrorsAgain = treeErrorDs.GetTreeErrors(treeID).ToArray();
            treeErrorsAgain.Where(x => x.Level == "W" && x.IsResolved).Should().HaveCount(1);
        }
    }
}
