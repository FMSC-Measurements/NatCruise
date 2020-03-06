using CruiseDAL;
using FluentAssertions;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Test;
using Prism.Navigation;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel_test : TestBase
    {
        public FixCNTViewModel_test(ITestOutputHelper output) : base(output)
        {
        }

        private CruiseDatastore_V3 CreateDatabase()
        {
            var database = new CruiseDatastore_V3();

            database.Insert(new CruiseDAL.V3.Models.CuttingUnit
            {
                Code = "u1",
                Area = 0,
            });

            var stratum = new Models.Stratum()
            {
                Code = "fixCnt1",
                Method = CruiseDAL.Schema.CruiseMethods.FIXCNT
            };
            database.Insert(stratum);

            var sg = new CruiseDAL.V3.Models.SampleGroup_V3()
            {
                SampleGroupCode = "sgFixCnt",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01",
                StratumCode = stratum.Code
            };
            database.Insert(sg);

            database.Execute($"INSERT INTO SpeciesCode (Species) values ('someSpecies');");

            var tdv = new CruiseDAL.V3.Models.TreeDefaultValue()
            {
                Species = "someSpecies",
                LiveDead = "L",
                PrimaryProduct = "01"
            };
            database.Insert(tdv);

            var sgTdv = new CruiseDAL.V3.Models.Subpopulation()
            {
                StratumCode = sg.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
                Species = tdv.Species,
                LiveDead = tdv.LiveDead,
            };
            database.Insert(sgTdv);

            var fixCntTallyClass = new CruiseDAL.V3.Models.FixCNTTallyClass_V3()
            {
                Field = "DBH",
                StratumCode = stratum.Code
            };
            database.Insert(fixCntTallyClass);
            //database.Execute($"Update FixCNTTallyClass set FieldName = 'DBH';");

            var fixCntTallyPop = new CruiseDAL.V3.Models.FixCNTTallyPopulation_V3()
            {
                StratumCode = stratum.Code,
                SampleGroupCode = sg.SampleGroupCode,
                Species = tdv.Species,
                LiveDead = tdv.LiveDead,
                IntervalSize = 101,
                Min = 102,
                Max = 103
            };
            database.Insert(fixCntTallyPop);

            database.Insert(new CruiseDAL.V3.Models.Plot_V3
            {
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber = 1
            });

            return database;
        }

        [Fact]
        public void Refresh_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var viewModel = new FixCNTViewModel((INavigationService)null,
                    new Services.DataserviceProvider(App) { CuttingUnitDatastore = datastore });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");

                viewModel.OnNavigatedTo(navParams);

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
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var viewModel = new FixCNTViewModel((INavigationService)null,
                    new Services.DataserviceProvider(App) { CuttingUnitDatastore = datastore });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");
                viewModel.OnNavigatedTo(navParams);

                var tallyPop = viewModel.TallyPopulations.First();

                tallyPop.Buckets.Should().NotBeEmpty();

                var bucket = tallyPop.Buckets.FirstOrDefault();
                bucket.Should().NotBeNull();

                viewModel.Tally(bucket);

                bucket.TreeCount.Should().Be(1);

                viewModel.OnNavigatedTo(navParams);

                bucket = tallyPop.Buckets.FirstOrDefault();
                bucket.TreeCount.Should().Be(1);
            }
        }
    }
}