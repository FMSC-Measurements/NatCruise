using FluentAssertions;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class PlotStrataDataservice_Test : TestBase
    {
        public PlotStrataDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetPlot_Strata()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                var plotStrata = plotStratumDs.GetPlot_Strata(unit, plot.PlotNumber);
                plotStrata.Should().HaveCount(2);
            }
        }

        [Fact]
        public void GetPlot_Strata_insertIfNotExists()
        {
            var init = new DatastoreInitializer();
            var unit = init.Units[0];
            using (var db = init.CreateDatabase())
            {
                var ds = new PlotDataservice(db, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(db, init.CruiseID, init.DeviceID);

                ds.AddNewPlot(unit);

                var plots = ds.GetPlotsByUnitCode(unit);
                plots.Should().HaveCount(1);
                var plot = plots.Single();

                db.Execute("DELETE FROM Plot_Stratum;");
                db.GetRowCount("Plot_Stratum", "").Should().Be(0);

                var plotStrata = plotStratumDs.GetPlot_Strata(unit, plot.PlotNumber);
                plotStrata.Should().HaveCount(2);
            }
        }

        [Fact]
        public void GetPlot_Strata_PreInitialized()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var strata = new string[] { "st1", "st2" };
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);

                datastore.GetPlotsByUnitCode(unitCode).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                foreach (var st in strata)
                {
                    database.Execute($"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{cruiseID}', '{unitCode}', {plotNumber}, '{st}');");
                }

                var stPlots = plotStratumDs.GetPlot_Strata(unitCode, plotNumber);

                stPlots.Should().HaveCount(strata.Count());

                foreach (var ps in stPlots)
                {
                    ValidatePlot_Stratum(ps, true);
                }
            }
        }

        [Fact]
        public void GetPlot_Stratum()
        {
            var units = new string[] { "u1" };
            var strata = new[]
            {
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st1", Method = "FIX" },
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st2", Method = "3PPNT", KZ3PPNT = 101  },
                new CruiseDAL.V3.Models.Stratum {StratumCode = "st3", Method = "FIX"},
            };
            var unit_strata = new[]
            {
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st1" },
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st2" },
                new CruiseDAL.V3.Models.CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st3" },
            };

            var init = new DatastoreInitializer()
            {
                Units = units,
                Strata = strata,
                UnitStrata = unit_strata,
                SampleGroups = null,
                Subpops = null,
                Species = null,
            };

            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);

                datastore.GetPlotsByUnitCode(units[0]).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{init.CruiseID}', '{plotID}', '{units[0]}', {plotNumber})");

                foreach (var st in strata)
                {
                    if (st.StratumCode == "st3") { continue; }

                    database.Execute($"INSERT INTO Plot_Stratum (CruiseID, CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{init.CruiseID}', '{units[0]}', {plotNumber}, '{st.StratumCode}');");
                }

                foreach (var st in strata)
                {
                    var plotStratum = plotStratumDs.GetPlot_Stratum(units[0], st.StratumCode, plotNumber);
                    ValidatePlot_Stratum(plotStratum, st.StratumCode != "st3");
                }

                var nonExistantPS = plotStratumDs.GetPlot_Stratum(units[0], "st3", plotNumber);
                ValidatePlot_Stratum(nonExistantPS, false);

                var tppnt = plotStratumDs.GetPlot_Stratum(units[0], "st2", plotNumber);
                tppnt.KZ3PPNT.Should().Be(101);

            }
        }

        [Fact]
        public void InsertStratumPlot()
        {
            var init = new DatastoreInitializer();
            var plotNumber = 1;
            var stratumCode = "st1";
            var unitCode = "u1";
            var isEmpty = true;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;
            //var remarks = "something";

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    //Remarks = remarks
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                plotStratumDs.InsertPlot_Stratum(stratumPlot);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var plotStratumAgain = plotStratumDs.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                plotStratumAgain.Should().NotBeNull();
                plotStratumAgain.PlotNumber.Should().Be(plotNumber);
                //ourStratumPlot.Remarks.Should().Be(remarks);
                plotStratumAgain.IsEmpty.Should().Be(isEmpty);
                plotStratumAgain.StratumCode.Should().Be(stratumCode);
            }
        }

        [Fact]
        public void Insert3PPNT_Plot_Stratum()
        {
            var init = new DatastoreInitializer();
            var plotNumber = 1;
            var stratumCode = "st1";
            var unitCode = "u1";
            var isEmpty = true;
            var kpi = 101;
            var treeCount = 11;
            var averageHeight = 12;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;
            //var remarks = "something";

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);


                
                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    KPI = kpi,
                    TreeCount = treeCount,
                    AverageHeight = averageHeight,

                    //Remarks = remarks
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                plotStratumDs.Insert3PPNT_Plot_Stratum(stratumPlot);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var plotStratumAgain = plotStratumDs.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                plotStratumAgain.Should().NotBeNull();
                plotStratumAgain.PlotNumber.Should().Be(plotNumber);
                //ourStratumPlot.Remarks.Should().Be(remarks);
                plotStratumAgain.KPI.Should().Be(kpi);
                plotStratumAgain.IsEmpty.Should().Be(isEmpty);
                plotStratumAgain.StratumCode.Should().Be(stratumCode);
            }
        }

        [Fact]
        public void UpdatePlot_Stratum()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var stratumCode = "st1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, cruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = false,
                };

                database.Execute($"INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{cruiseID}', '{plotID}', '{unitCode}', {plotNumber})");

                plotStratumDs.InsertPlot_Stratum(stratumPlot);

                stratumPlot.IsEmpty = true;
                plotStratumDs.UpdatePlot_Stratum(stratumPlot);

                var ourStratumPlot = plotStratumDs.GetPlot_Stratum(unitCode, stratumCode, plotNumber);

                ourStratumPlot.IsEmpty.Should().Be(true);
            }
        }

        [Fact]
        public void DeletePlot_Stratum()
        {
            var init = new DatastoreInitializer();
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var plotNumber = 1;
            var plotID = Guid.NewGuid().ToString();

            using (var database = init.CreateDatabase())
            {
                var datastore = new PlotDataservice(database, init.CruiseID, init.DeviceID);
                var plotStratumDs = new PlotStratumDataservice(database, init.CruiseID, init.DeviceID);
                var plotTreeDs = new PlotTreeDataservice(database, init.CruiseID, init.DeviceID,
                    new SamplerStateDataservice(database, init.CruiseID, init.DeviceID));

                var stratumPlot = new Models.Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                };

                database.Execute("INSERT INTO Plot (CruiseID, PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{init.CruiseID}', '{plotID}', '{unitCode}', {plotNumber});");

                plotStratumDs.InsertPlot_Stratum(stratumPlot);
                plotTreeDs.InsertTree(new PlotTreeEntry
                {
                    CuttingUnitCode = unitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    PlotNumber = plotNumber,
                    TreeCount = 1,
                }, null);

                var echo = plotStratumDs.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                echo.Should().NotBeNull("where's my echo");

                database.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Count(init.CruiseID, unitCode, plotNumber)
                    .Should().Be(1);

                plotStratumDs.DeletePlot_Stratum(echo.CuttingUnitCode, echo.StratumCode, echo.PlotNumber);

                database.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Count(init.CruiseID, unitCode, plotNumber)
                    .Should().Be(0);
            }
        }

        private void ValidatePlot_Stratum(Plot_Stratum ps, bool inCruise)
        {
            ps.CuttingUnitCode.Should().NotBeNullOrWhiteSpace();
            ps.StratumCode.Should().NotBeNullOrWhiteSpace();
            ps.PlotNumber.Should().BeGreaterThan(0);
            ps.InCruise.Should().Be(inCruise);
        }
    }
}
