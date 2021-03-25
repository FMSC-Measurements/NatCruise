using CruiseDAL;
using FluentAssertions;
using NatCruise.Cruise.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Test;
using Prism.Navigation;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using FScruiser.XF.Data;
using NatCruise.Cruise.Data;
using NatCruise.Test;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel_test : TestBase
    {
        public FixCNTViewModel_test(ITestOutputHelper output) : base(output)
        {
        }

        private CruiseDatastore_V3 CreateDatabase(out string cruiseID)
        {
            var saleID = Guid.NewGuid().ToString();
            cruiseID = Guid.NewGuid().ToString();
            var database = new CruiseDatastore_V3();

            var cruise = new CruiseDAL.V3.Models.Cruise
            {
                CruiseID = cruiseID,
                SaleID = saleID,
            };
            database.Insert(cruise);

            database.Insert(new CruiseDAL.V3.Models.CuttingUnit
            {
                CruiseID = cruiseID,
                CuttingUnitCode = "u1",
                Area = 0,
            });

            var stratum = new CruiseDAL.V3.Models.Stratum()
            {
                CruiseID = cruiseID,
                StratumCode = "fixCnt1",
                Method = CruiseDAL.Schema.CruiseMethods.FIXCNT,
                FixCNTField = "DBH",
            };
            database.Insert(stratum);

            var sg = new CruiseDAL.V3.Models.SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupCode = "sgFixCnt",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01",
                StratumCode = stratum.StratumCode
            };
            database.Insert(sg);

            database.Execute($"INSERT INTO SpeciesCode (Species) values ('someSpecies');");

            var tdv = new CruiseDAL.V3.Models.TreeDefaultValue()
            {
                CruiseID = cruiseID,
                SpeciesCode = "someSpecies",
                PrimaryProduct = "01"
            };
            database.Insert(tdv);

            var sgTdv = new CruiseDAL.V3.Models.SubPopulation()
            {
                CruiseID = cruiseID,
                StratumCode = sg.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
                SpeciesCode = tdv.SpeciesCode,
                LiveDead = "L",
            };
            database.Insert(sgTdv);

            var fixCntTallyPop = new CruiseDAL.V3.Models.FixCNTTallyPopulation()
            {
                CruiseID = cruiseID,
                StratumCode = stratum.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
                SpeciesCode = tdv.SpeciesCode,
                LiveDead = "L",
                IntervalSize = 101,
                Min = 102,
                Max = 103
            };
            database.Insert(fixCntTallyPop);

            database.Insert(new CruiseDAL.V3.Models.Plot
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber = 1
            });

            return database;
        }

        [Fact]
        public void Refresh_test()
        {
            using (var database = CreateDatabase(out var cruiseID))
            {
                var viewModel = new FixCNTViewModel(
                    new DataserviceProvider(database, new TestDeviceInfoService())
                    {
                        CruiseID = cruiseID
                    });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");

                viewModel.Initialize(navParams);

                viewModel.TallyPopulations.Should().NotBeEmpty();

                foreach (var tp in viewModel.TallyPopulations)
                {
                    tp.Buckets.Should().NotBeEmpty();
                    foreach (var b in tp.Buckets)
                    {
                        b.Value.Should().NotBe(0.0);
                    }
                }
            }
        }

        [Fact]
        public void Tally()
        {
            using (var database = CreateDatabase(out var cruiseID))
            {
                var deviceInfo = new TestDeviceInfoService();
                var datastore = new CuttingUnitDatastore(database, cruiseID, deviceInfo.DeviceID);

                var viewModel = new FixCNTViewModel(
                    new DataserviceProvider(database, deviceInfo)
                    {
                        CruiseID = cruiseID
                    });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");
                viewModel.Initialize(navParams);

                var tallyPop = viewModel.TallyPopulations.First();

                tallyPop.Buckets.Should().NotBeEmpty();

                var bucket = tallyPop.Buckets.FirstOrDefault();
                bucket.Should().NotBeNull();

                viewModel.Tally(bucket);

                bucket.TreeCount.Should().Be(1);

                viewModel.Initialize(navParams);

                bucket = tallyPop.Buckets.FirstOrDefault();
                bucket.TreeCount.Should().Be(1);
            }
        }
    }
}