using CruiseDAL;
using FluentAssertions;
using FScruiser.Data;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test_Tree_Methods : Datastore_TestBase
    {
        public CuttingUnitDatastore_Test_Tree_Methods(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("u1", "st1", null, "", "", Skip = "sampleGroup is required now")]
        [InlineData("u1", "st1", "sg1", "sp1", "L")]
        public void GetTreeStub(string unitCode, string stratumCode, string sgCode, string species, string liveDead)
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_GUID = datastore.CreateMeasureTree(unitCode, stratumCode, sgCode, species, liveDead);

                var tree = datastore.GetTreeStub(tree_GUID);
                tree.Should().NotBeNull();

                tree.TreeID.Should().Be(tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                //tree.CountOrMeasure.Should().Be(countMeasure);
            }
        }

        [Fact]
        public void CreateMeasureTree()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            //var countMeasure = "C";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var treeID = datastore.CreateMeasureTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();

                //tree.CuttingUnit_CN.Should().Be(1);
                tree.TreeID.Should().Be(treeID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                //tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);

                var tds = new TallyDataservice(database, new TestDeviceInfoService());

                var tallyLedger = tds.GetTallyEntry(treeID);
                tallyLedger.Should().NotBeNull();
                tallyLedger.CountOrMeasure.Should().Be("M");
            }
        }



        [Fact]
        public void UpdateTree()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var treeID = datastore.CreateMeasureTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();
                tree.CuttingUnitCode.Should().Be(unitCode);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                tree.TreeNumber.Should().Be(1);
                tree.TreeID.Should().Be(treeID);

                //unitCode = "u2"; // tree should not be able to change units
                stratumCode = "st2";
                sgCode = "sg2";
                species = "sp2";
                liveDead = "D";


                tree.CuttingUnitCode = unitCode;
                tree.StratumCode = stratumCode;
                tree.SampleGroupCode = sgCode;
                tree.Species = species;
                tree.LiveDead = liveDead;

                datastore.UpdateTree(tree);

                var treeAgain = datastore.GetTree(treeID);

                treeAgain.CuttingUnitCode.Should().Be(unitCode);
                treeAgain.StratumCode.Should().Be(stratumCode);
                treeAgain.SampleGroupCode.Should().Be(sgCode);
                treeAgain.Species.Should().Be(species);
                treeAgain.LiveDead.Should().Be(liveDead);
            }
        }

        [Fact]
        public void DeleteTree()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var treeID = datastore.CreateMeasureTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();

                datastore.DeleteTree(treeID);

                tree = datastore.GetTree(treeID);
                tree.Should().BeNull();
            }
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

            using (var database = CreateDatabase())
            {

                var datastore = new CuttingUnitDatastore(database);
                


                var treeID = datastore.CreateMeasureTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var treeErrors = datastore.GetTreeErrors(treeID).ToArray();

                treeErrors.Should().HaveCount(1);

                var speciesError = treeErrors.First();
                speciesError.Level.Should().Be("E");
                speciesError.Field.Should().Be(nameof(Models.Tree.Species));
            }
        }
    }
}
