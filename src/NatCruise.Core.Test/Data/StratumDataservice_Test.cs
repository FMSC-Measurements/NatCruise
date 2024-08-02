using FluentAssertions;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using CuttingUnit_Stratum = CruiseDAL.V3.Models.CuttingUnit_Stratum;
using Tree = CruiseDAL.V3.Models.Tree;
using Plot = CruiseDAL.V3.Models.Plot;
using Plot_Stratum = CruiseDAL.V3.Models.Plot_Stratum;

namespace NatCruise.Test.Data
{
    public class StratumDataservice_Test
    {

        [Fact]
        public void DeleteStratum_WithTrees()
        {
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var trees = new[]
                {
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 1, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 2, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                };
                foreach (var tree in trees)
                { database.Insert(tree); }

                var treeDs = new TreeDataservice(database, init.CruiseID, init.DeviceID);
                treeDs.InsertManualTree("u1", "st1", "sg1");

                var plot = new Plot
                {
                    CruiseID = init.CruiseID,
                    CuttingUnitCode = "u1",
                    PlotID = Guid.NewGuid().ToString(),
                    PlotNumber = 1,
                };
                database.Insert(plot);

                var plot_st = new Plot_Stratum
                {
                    CruiseID = init.CruiseID,
                    PlotNumber = plot.PlotNumber,
                    CuttingUnitCode = plot.CuttingUnitCode,
                    StratumCode = "st1",
                };
                database.Insert(plot_st);

                var plotTrees = new[]
                {
                    new Tree {
                        CruiseID = init.CruiseID,
                        TreeID = Guid.NewGuid().ToString(),
                        TreeNumber = 1,
                        CuttingUnitCode = "u1",
                        StratumCode = "st1",
                        SampleGroupCode = "sg1",
                        PlotNumber =
                        plot.PlotNumber,
                    },
                    new Tree {
                        CruiseID = init.CruiseID,
                        TreeID = Guid.NewGuid().ToString(),
                        TreeNumber = 2,
                        CuttingUnitCode = "u1",
                        StratumCode = "st1",
                        SampleGroupCode = "sg1",
                        PlotNumber = plot.PlotNumber,
                    },
                };
                foreach (var tree in plotTrees)
                { database.Insert(tree); }


                var datastore = new StratumDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var results = datastore.GetStrata();

                results.Where(s => s.StratumCode == "st1").Single().HasTrees.Should().BeTrue();
                results.Where(s => s.StratumCode != "st1").All(x => x.HasTrees == false).Should().BeTrue();

                var stratum = results.Where(s => s.StratumCode == "st1").Single();
                datastore.DeleteStratum(stratum);

                database.From<Tree>().Where("StratumCode = 'st1'").Count().Should().Be(0);
                database.From<Plot_Stratum>().Where("StratumCode = 'st1'").Count().Should().Be(0);
            }
        }


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

        [Theory]
        [InlineData("u1", "st3", "st4")]
        [InlineData("u2", "st4")]
        public void GetStrataByUnitCode_Test(string unitCode, params string[] expectedStrataCodes)
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                var datastore = new StratumDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var strata = database.Query<Stratum>
                    ("select * from stratum;").ToArray();

                var stuff = database.QueryGeneric("select * from Stratum;").ToArray();

                var results = datastore.GetStrataByUnitCode(unitCode);
                results.All(x => !string.IsNullOrEmpty(x.StratumID)).Should().BeTrue();

                var strata_codes = results.Select(x => x.StratumCode);
                strata_codes.Should().Contain(expectedStrataCodes);
                strata_codes.Should().HaveSameCount(expectedStrataCodes);
            }
        }

        [Theory]
        [InlineData("u1", "st1", "st2", "st3")]
        [InlineData("u2", "st1", "st4")]
        public void GetStrata_WithUnitCode_Test(string unitCode, params string[] expectedStrataCodes)
        {
            var init = new DatastoreInitializer()
            {
                UnitStrata = new[]
                {
                    new CuttingUnit_Stratum { CuttingUnitCode = "u1", StratumCode = "st1" },
                    new CuttingUnit_Stratum { CuttingUnitCode = "u1", StratumCode = "st2" },
                    new CuttingUnit_Stratum { CuttingUnitCode = "u1", StratumCode = "st3" },

                    new CuttingUnit_Stratum { CuttingUnitCode = "u2", StratumCode = "st1" },
                    new CuttingUnit_Stratum { CuttingUnitCode = "u2", StratumCode = "st4" },
                }
            };

            using (var database = init.CreateDatabase())
            {
                var datastore = new StratumDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var results = datastore.GetStrata(unitCode);
                results.All(x => !string.IsNullOrEmpty(x.StratumID)).Should().BeTrue();

                var strata_codes = results.Select(x => x.StratumCode);
                strata_codes.Should().Contain(expectedStrataCodes);
                strata_codes.Should().HaveSameCount(expectedStrataCodes);
            }
        }

        [Fact]
        public void GetStrata_HasTrees_With_Trees()
        {
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var trees = new[]
                {
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 1, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 2, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                };
                foreach (var tree in trees)
                { database.Insert(tree); }


                var datastore = new StratumDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var results = datastore.GetStrata();

                results.Where(s => s.StratumCode == "st1").Single().HasTrees.Should().BeTrue();
                results.Where(s => s.StratumCode != "st1").All(x => x.HasTrees == false).Should().BeTrue();
                results.All(x => !string.IsNullOrEmpty(x.StratumID)).Should().BeTrue();
            }
        }


        [Fact]
        public void GetStrata_HasTrees_With_PlotsAndTrees()
        {
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                var trees = new[]
                {
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 1, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 2, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                };
                foreach (var tree in trees)
                { database.Insert(tree); }

                var plot = new Plot
                {
                    CruiseID = init.CruiseID,
                    CuttingUnitCode = "u1",
                    PlotID = Guid.NewGuid().ToString(),
                    PlotNumber = 1,
                };
                database.Insert(plot);

                var plot_st = new Plot_Stratum
                {
                    CruiseID = init.CruiseID,
                    PlotNumber = plot.PlotNumber,
                    CuttingUnitCode = plot.CuttingUnitCode,
                    StratumCode = "st1",
                };
                database.Insert(plot_st);


                var datastore = new StratumDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var results = datastore.GetStrata();

                results.Where(s => s.StratumCode == "st1").Single().HasTrees.Should().BeTrue();
                results.Where(s => s.StratumCode != "st1").All(x => x.HasTrees == false).Should().BeTrue();
                results.All(x => !string.IsNullOrEmpty(x.StratumID)).Should().BeTrue();
            }
        }

        [Fact]
        public void GetStratumUnitInfo()
        {
            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new StratumDataservice(db, init.CruiseID, init.DeviceID);

            var trees = new[]
                {
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 1, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                    new Tree { CruiseID = init.CruiseID, TreeID = Guid.NewGuid().ToString(), TreeNumber = 2, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1",  },
                };
            foreach (var tree in trees)
            { db.Insert(tree); }

            foreach (var st in init.Strata)
            {
                var units = ds.GetStratumUnitInfo(st.StratumCode);
                units.Should().NotBeNull();
                units.Should().HaveCountGreaterThan(0);
            }

            var unitsSt1 = ds.GetStratumUnitInfo("st1");
            unitsSt1.Where(x => x.CuttingUnitCode == "u1").Single().HasTrees.Should().BeTrue();

            var unitsSt2 = ds.GetStratumUnitInfo("st2");
            unitsSt2.Any( x => x.HasTrees).Should().BeFalse();
        }

        [Fact]
        public void RemoveStratumFromCuttingUnit()
        {
            var unitCode = "u1";
            var stCode = "st1";

            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                var stDs = new StratumDataservice(database, init.CruiseID, init.DeviceID);
                var unitDs = new CuttingUnitDataservice(database, init.CruiseID, init.DeviceID);

                var unitCodes = unitDs.GetCuttingUnitCodesByStratum(stCode);
                unitCodes.Should().Contain(unitCode);

                stDs.RemoveStratumFromCuttingUnit(unitCode, stCode);

                var unitCodesAgain = unitDs.GetCuttingUnitCodesByStratum(stCode);
                unitCodesAgain.Should().NotContain(unitCode);

            }
        }



        [Fact]
        public void RemoveStratumFromCuttingUnit_With_PlotStrata()
        {
            var unitCode = "u1";
            var stCode = "st1";

            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                // setup and validate setup
                var plotStDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);
                var plotDs = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                plotDs.AddNewPlot(unitCode);
                plotDs.GetPlotsByUnitCode(unitCode).Select(x => x.PlotNumber).Should().Contain(1);

                var plot_strata = plotStDs.GetPlot_Strata(unitCode, 1);
                plot_strata.Select(x => x.StratumCode).Should().Contain(stCode);

                var stDs = new StratumDataservice(database, init.CruiseID, init.DeviceID);
                var unitDs = new CuttingUnitDataservice(database, init.CruiseID, init.DeviceID);

                var unitCodes = unitDs.GetCuttingUnitCodesByStratum(stCode);
                unitCodes.Should().Contain(unitCode);

                // remove unit from stratum
                stDs.RemoveStratumFromCuttingUnit(unitCode, stCode);


                // validate
                var unitCodesAgain = unitDs.GetCuttingUnitCodesByStratum(stCode);
                unitCodesAgain.Should().NotContain(unitCode);

                var plot_strataAgian = plotStDs.GetPlot_Strata(unitCode, 1);
                plot_strataAgian.Select(x => x.StratumCode).Should().NotContain(stCode);

            }
        }
    }
}
