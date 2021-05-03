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

        private void InitializeFizCNT(CruiseDatastore_V3 database, string cruiseID)
        {
            var stratum = new CruiseDAL.V3.Models.Stratum()
            {
                CruiseID = cruiseID,
                StratumID = Guid.NewGuid().ToString(),
                StratumCode = "fixCnt1",
                Method = CruiseDAL.Schema.CruiseMethods.FIXCNT,
                FixCNTField = "DBH",
            };
            database.Insert(stratum);

            var sg = new CruiseDAL.V3.Models.SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = Guid.NewGuid().ToString(),
                SampleGroupCode = "sgFixCnt",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01",
                StratumCode = stratum.StratumCode
            };
            database.Insert(sg);

            database.Execute($"INSERT INTO Species (SpeciesCode, CruiseID) values ('someSpecies', '{cruiseID}');");

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
                SubPopulationID = Guid.NewGuid().ToString(),
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
        }

        [Fact]
        public void Refresh_test()
        {
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                InitializeFizCNT(database, init.CruiseID);

                var viewModel = new FixCNTTallyViewModel(
                    new DataserviceProvider(database, new TestDeviceInfoService())
                    {
                        CruiseID = init.CruiseID
                    });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");

                viewModel.Initialize(navParams);
                viewModel.Load();

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
            var init = new DatastoreInitializer();

            using (var database = init.CreateDatabase())
            {
                InitializeFizCNT(database, init.CruiseID);

                var deviceInfo = new TestDeviceInfoService();
                var datastore = new CuttingUnitDatastore(database, init.CruiseID, deviceInfo.DeviceID,new SamplerInfoDataservice(database, init.CruiseID, init.DeviceID));

                var viewModel = new FixCNTTallyViewModel(
                    new DataserviceProvider(database, deviceInfo)
                    {
                        CruiseID = init.CruiseID,
                    });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");
                viewModel.Initialize(navParams);
                viewModel.Load();

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