using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Data;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class TreeFieldValueDataservice_Test : TestBase
    {
        public TreeFieldValueDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTreeFieldValues_CheckFieldOrder()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var cruiseID = init.CruiseID;
            var stCode = "st1";
            var treeFieldSetups = new[]
            {
                new TreeFieldSetup
                {
                    CruiseID = cruiseID,
                    StratumCode = stCode,
                    Field = nameof(TreeMeasurment.DBH),
                },
                new TreeFieldSetup
                {
                    CruiseID = cruiseID,
                    StratumCode = stCode,
                    Field = nameof(TreeMeasurment.Initials),
                },
                new TreeFieldSetup
                {
                    CruiseID = cruiseID,
                    StratumCode = stCode,
                    Field = nameof(TreeMeasurment.DRC),
                }
            };

            // add fields in reverse order to insure insertion order doesn't effect
            // order TFV come out
            foreach (var (tfs, i) in treeFieldSetups.Select((x, i) => (x, i)).Reverse())
            {
                tfs.FieldOrder = i;
                db.Insert(tfs);
            }

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree
            {
                CruiseID = cruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = stCode,
                SampleGroupCode = "sg1",
            };
            db.Insert(tree);

            var ds = new TreeFieldValueDataservice(db, cruiseID, "devID");

            var tfvs = ds.GetTreeFieldValues(treeID);
            tfvs.Should().HaveSameCount(treeFieldSetups);

            var tfvFields = tfvs.Select(x => x.Field.ToLower());
            var tfsFields = treeFieldSetups.Select(x => x.Field.ToLower());
            tfvFields.Should().BeEquivalentTo(tfsFields);
        }

        //[Fact]
        //public void Issue_TreeFieldsNotInCorrectOrder()
        //{
        //    var testFile = GetTestFile("Issue_TreeFieldsNotInCorrectOrder.crz3db");

        //    var db = new CruiseDatastore_V3(testFile);

        //    var cruiseID = db.From<Cruise>().Query().Single().CruiseID;

        //    var tree = db.From<Tree>().Query().Single();
        //    var ds = new TreeFieldValueDataservice(db, cruiseID, "devID");

        //    var tfs = db.From<TreeFieldSetup>().Where("StratumCode = @p1 AND SampleGroupCode IS NULL")
        //        .OrderBy("FieldOrder")
        //        .Query(tree.StratumCode).ToArray();

        //    var tfvs = ds.GetTreeFieldValues(tree.TreeID);

        //    tfvs.Should().HaveSameCount(tfs);

        //    var tfsFields = tfs.Select(x => x.Field.ToLower());
        //    var tfvFields = tfvs.Select(y => y.Field.ToLower());

        //    tfvFields.Should().BeEquivalentTo(tfsFields);
        //}
    }
}