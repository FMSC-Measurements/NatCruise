using CruiseDAL.V3.Models;
using FluentAssertions;
using NatCruise.Data;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class TreeDataservice_Test : TestBase
    {
        public TreeDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTrees()
        {
            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";
            var sp = "sp1";

            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
            };
            db.Insert(tree);

            var treeDs = new TreeDataservice(db, init.CruiseID, init.DeviceID);
            var treesAgain = treeDs.GetTrees(unit, stratum, sg, sp);
            treesAgain.Should().HaveCount(1);

            var treeAgain = treesAgain.Single();
            treeAgain.TreeID.Should().Be(treeID);
        }

        [Fact]
        public void GetTrees_MultipleTallyLedger()
        {
            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";
            var sp = "sp1";

            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
            };
            db.Insert(tree);

            var tallyLedger = new TallyLedger()
            {
                TallyLedgerID = Guid.NewGuid().ToString(),
                CruiseID = init.CruiseID,
                TreeID = treeID,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
                TreeCount = 1,
            };
            db.Insert(tallyLedger);

            var tallyLedger2 = new TallyLedger()
            {
                TallyLedgerID = Guid.NewGuid().ToString(),
                CruiseID = init.CruiseID,
                TreeID = treeID,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
                TreeCount = 1,
            };
            db.Insert(tallyLedger2);

            var treeDs = new TreeDataservice(db, init.CruiseID, init.DeviceID);
            var treesAgain = treeDs.GetTrees(unit, stratum, sg, sp);
            treesAgain.Should().HaveCount(1);

            var treeAgain = treesAgain.Single();
            treeAgain.TreeID.Should().Be(treeID);
        }

        [Fact]
        public void GetTrees_MultipleTallyLedger_3PTree()
        {
            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";
            var sp = "sp1";

            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();
            var treeDs = new TreeDataservice(db, init.CruiseID, init.DeviceID);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
            };
            db.Insert(tree);

            var tallyLedger = new TallyLedger()
            {
                TallyLedgerID = Guid.NewGuid().ToString(),
                CruiseID = init.CruiseID,
                TreeID = treeID,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
                TreeCount = 1,
                
            };
            db.Insert(tallyLedger);


            Validate(0,false);
            

            //need to sleep for 1sec because time stamps resolution is only down to the second
            System.Threading.Thread.Sleep(1000);

            var tallyLedger2 = new TallyLedger()
            {
                TallyLedgerID = Guid.NewGuid().ToString(),
                CruiseID = init.CruiseID,
                TreeID = treeID,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
                KPI = 102,
                STM = true,
            };
            db.Insert(tallyLedger2);


            Validate(102, true);

            void Validate(int kpi, bool stm)
            {
                var treesAgain = treeDs.GetTrees(unit, stratum, sg, sp);
                treesAgain.Should().HaveCount(1);

                var treeAgain = treesAgain.Single();
                treeAgain.TreeID.Should().Be(treeID);
                treeAgain.KPI.Should().Be(kpi);
                treeAgain.STM.Should().Be(stm);
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

                var tds = new TallyDataservice(database, init.CruiseID, init.DeviceID, new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));

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