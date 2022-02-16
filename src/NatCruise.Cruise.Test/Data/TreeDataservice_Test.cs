using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Cruise.Data;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void GetTreeFieldValues_nonExistantTree()
        {
            var initializer = new DatastoreInitializer();

            using var db = initializer.CreateDatabase();

            var dataservice = new TreeDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var treeID = Guid.NewGuid().ToString();
            var tfvs = dataservice.GetTreeFieldValues(treeID);
            tfvs.Should().BeEmpty();
        }

        [Fact]
        public void GetTreeFieldValues_NoFieldSetup()
        {
            var initializer = new DatastoreInitializer();

            using var db = initializer.CreateDatabase();

            var dataservice = new TreeDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var treeID = dataservice.InsertManualTree("u1", "st1", "sg1");

            var tfvs = dataservice.GetTreeFieldValues(treeID);
            tfvs.Should().BeEmpty();
        }

        [Fact]
        public void GetTreeFieldValues()
        {
            var rand = new Bogus.Randomizer(8675309);
            var initializer = new DatastoreInitializer();

            using var db = initializer.CreateDatabase();

            var treeFields = db.From<TreeField>().Query().ToArray();

            var treeFieldSetupList = new List<TreeFieldSetup>();
            foreach(var tf in treeFields)
            {
                var tfs = new CruiseDAL.V3.Models.TreeFieldSetup
                {
                    CruiseID = initializer.CruiseID,
                    Field = tf.Field,
                    StratumCode = "st1",
                };

                switch(tf.DbType)
                {
                    case "REAL": tfs.DefaultValueReal = rand.Double(); break;
                    case "TEXT": tfs.DefaultValueText = rand.Word(); break;
                    case "BOOLEAN": tfs.DefaultValueBool = rand.Bool(); break;
                    case "INTEGER": tfs.DefaultValueInt = rand.Int(); break;
                }
                db.Insert(tfs);

                treeFieldSetupList.Add(tfs);
            }

            var dataservice = new TreeDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var treeID = dataservice.InsertManualTree("u1", "st1", "sg1");

            var tfvs = dataservice.GetTreeFieldValues(treeID);
            tfvs.Should().HaveSameCount(treeFieldSetupList);

            foreach(var tf in treeFieldSetupList)
            {
                var tfv = tfvs.FirstOrDefault(x => string.Equals(x.Field, tf.Field, StringComparison.OrdinalIgnoreCase));
                tfv.Should().NotBeNull(tf.Field);
                tfv.DefaultValueReal.Should().Be(tf.DefaultValueReal, tf.Field);
                tfv.DefaultValueText.Should().Be(tf.DefaultValueText, tf.Field);
                tfv.DefaultValueInt.Should().Be(tf.DefaultValueInt, tf.Field);
                tfv.DefaultValueBool.Should().Be(tf.DefaultValueBool, tf.Field);
            }

        }

        [Fact]
        public void IsTreeNumberAvalible_noPlot()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];

            using var db = init.CreateDatabase();

            var ds = new TreeDataservice(db, init.CruiseID, init.DeviceID);

            ds.IsTreeNumberAvalible(unit, 1).Should().BeTrue();
        }

        [Fact]
        public void IsTreeNumberAvalible_Plot_nummberAcrossStrata()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            var stratum = init.PlotStrata[0].StratumCode;
            var altStratum = init.PlotStrata[1].StratumCode;
            var plotNumber = 1;

            using var db = init.CreateDatabase();

            db.Execute("UPDATE Cruise SET UseCrossStrataPlotTreeNumbering = 1 WHERE CruiseID = @p1;", init.CruiseID);

            var ds = new TreeDataservice(db, init.CruiseID, init.DeviceID);

            var newPlot = new Plot()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = plotNumber,
                PlotID = Guid.NewGuid().ToString(),
            };
            db.Insert(newPlot);

            var newPlotStratum = new Plot_Stratum()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = newPlot.PlotNumber,
                StratumCode = stratum,
            };
            db.Insert(newPlotStratum);

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeTrue();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();

            db.Insert(new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = init.SampleGroups[0].SampleGroupCode,
                PlotNumber = plotNumber,
            });

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeFalse();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeFalse();
        }

        [Fact]
        public void IsTreeNumberAvalible_Plot_dontNummberAcrossStrata()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            var stratum = init.PlotStrata[0].StratumCode;
            var altStratum = init.PlotStrata[1].StratumCode;
            var plotNumber = 1;

            using var db = init.CreateDatabase();

            db.Execute("UPDATE Cruise SET UseCrossStrataPlotTreeNumbering = 0 WHERE CruiseID = @p1;", init.CruiseID);

            var ds = new TreeDataservice(db, init.CruiseID, init.DeviceID);

            var newPlot = new Plot()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = plotNumber,
                PlotID = Guid.NewGuid().ToString(),
            };
            db.Insert(newPlot);

            var newPlotStratum = new Plot_Stratum()
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = unit,
                PlotNumber = newPlot.PlotNumber,
                StratumCode = stratum,
            };
            db.Insert(newPlotStratum);

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeTrue();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();

            db.Insert(new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = init.SampleGroups[0].SampleGroupCode,
                PlotNumber = plotNumber,
            });

            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, newPlotStratum.StratumCode).Should().BeFalse();
            ds.IsTreeNumberAvalible(unit, 1, newPlotStratum.PlotNumber, altStratum).Should().BeTrue();
        }

        [Fact]
        public void InsertManualTree()
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            //var countMeasure = "C";
            var treeCount = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = datastore.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();

                //tree.CuttingUnit_CN.Should().Be(1);
                tree.TreeID.Should().Be(treeID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                //tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);

                var tds = new TallyDataservice(database, init.CruiseID, init.DeviceID, new SamplerInfoDataservice(database, init.CruiseID, init.DeviceID));

                var tallyLedger = tds.GetTallyEntry(treeID);
                tallyLedger.Should().NotBeNull();
                tallyLedger.CountOrMeasure.Should().Be("M");
            }
        }

        [Fact]
        public void UpdateTree()
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var treeCount = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = datastore.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();
                tree.CuttingUnitCode.Should().Be(unitCode);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
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
                tree.SpeciesCode = species;
                tree.LiveDead = liveDead;

                datastore.UpdateTree(tree);

                var treeAgain = datastore.GetTree(treeID);

                treeAgain.CuttingUnitCode.Should().Be(unitCode);
                treeAgain.StratumCode.Should().Be(stratumCode);
                treeAgain.SampleGroupCode.Should().Be(sgCode);
                treeAgain.SpeciesCode.Should().Be(species);
                treeAgain.LiveDead.Should().Be(liveDead);
            }
        }

        [Fact]
        public void DeleteTree()
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var treeCount = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = datastore.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();

                datastore.DeleteTree(treeID);

                tree = datastore.GetTree(treeID);
                tree.Should().BeNull();
            }
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", 101)]
        public void GetTreesByUnitCode(string unitCode, string stratumCode, string sgCode, string species, string liveDead, int kpi)
        {
            var init = new DatastoreInitializer();
            using (var db = init.CreateDatabase())
            {
                var tfs = new TreeFieldSetup
                {
                    CruiseID = init.CruiseID,
                    StratumCode = stratumCode,
                    //SampleGroupCode = sgCode,
                    Field = "DBH",
                    DefaultValueReal = 103.0,
                };
                db.Insert(tfs);

                var ds = new TreeDataservice(db, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tree_GUID = ds.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, kpi: kpi);

                db.Execute("UPDATE TreeMeasurment  SET DBH = @p2 WHERE TreeID = @p1;", tree_GUID, 107.0);

                var trees = ds.GetTreesByUnitCode(unitCode).ToArray();
                trees.Should().HaveCount(1);

                var tree = trees.First();
                tree.KPI.Should().Be(kpi);

                tree.DBH.Should().Be(107.0);
            }
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", 101)]
        public void GetTreesByUnitCode_With_StratumFieldSetupDefaultValue(string unitCode, string stratumCode, string sgCode, string species, string liveDead, int kpi)
        {
            var init = new DatastoreInitializer();
            using (var db = init.CreateDatabase())
            {
                var tfs = new TreeFieldSetup
                {
                    CruiseID = init.CruiseID,
                    StratumCode = stratumCode,
                    //SampleGroupCode = sgCode,
                    Field = "DBH",
                    DefaultValueReal = 103.0,
                };
                db.Insert(tfs);

                var ds = new TreeDataservice(db, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tree_GUID = ds.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, kpi: kpi);

                var trees = ds.GetTreesByUnitCode(unitCode).ToArray();
                trees.Should().HaveCount(1);

                var tree = trees.First();
                tree.KPI.Should().Be(kpi);

                tree.DBH.Should().Be(103.0);
            }
        }

        [Theory]
        [InlineData("u1", "st1", null, "", "", Skip = "sampleGroup is required now")]
        [InlineData("u1", "st1", "sg1", "sp1", "L")]
        public void GetTreeStub(string unitCode, string stratumCode, string sgCode, string species, string liveDead)
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                var datastore = new TreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tree_GUID = datastore.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead);

                var tree = datastore.GetTreeStub(tree_GUID);
                tree.Should().NotBeNull();

                tree.TreeID.Should().Be(tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
                //tree.CountOrMeasure.Should().Be(countMeasure);
            }
        }

        [Theory]
        [InlineData("u1", "st1", null, "", "", Skip = "sampleGroup is required now")]
        [InlineData("u1", "st1", "sg1", "sp1", "L")]
        [InlineData("u1", "st1", "sg1", null, null)]
        public void GetTreeStubByUnitCode(string unitCode, string stratumCode, string sgCode, string species, string liveDead)
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                var datastore = new TreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tree_GUID = datastore.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead);
                //datastore.GetTreeStub(tree_GUID).Should().NotBeNull();

                var trees = datastore.GetTreeStubsByUnitCode(unitCode).ToArray();

                trees.Should().HaveCount(1);

                var tree = trees.First();
                tree.TreeID.Should().Be(tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.SpeciesCode.Should().Be(species);
                //tree.CountOrMeasure.Should().Be(countMeasure);
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

            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = treeDS.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var treeErrors = treeDS.GetTreeErrors(treeID).ToArray();

                treeErrors.Should().HaveCount(3);

                var speciesError = treeErrors.First();
                speciesError.Level.Should().Be("E");
                speciesError.Field.Should().Be(nameof(Models.Tree.SpeciesCode));
            }
        }

        [Fact]
        public void UpdateTreeInitials()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = (string)null;
            var liveDead = "L";
            //var countMeasure = "M";
            var treeCount = 1;
            var initials = "something";

            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = treeDS.InsertManualTree(unitCode, stratumCode, sgCode, species, liveDead, treeCount);

                var stuff = database.QueryGeneric("SELECT * FROM TreeMeasurment;");

                treeDS.UpdateTreeInitials(treeID, initials);

                database.ExecuteScalar<string>("SELECT Initials FROM TreeMeasurment WHERE TreeID = @p1", treeID).Should().Be(initials);

                var tree = treeDS.GetTree(treeID);
                tree.Initials.Should().Be(initials);

                treeDS.UpdateTreeInitials(treeID, "somethingElse");
                var treeAgain = treeDS.GetTree(treeID);
                treeAgain.Initials.Should().Be("somethingElse");
            }
        }
    }
}