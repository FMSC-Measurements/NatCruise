﻿using FluentAssertions;
using FMSC.Sampling;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Test;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Data
{
    public class TallyDataservice_Tests : TestBase
    {
        public TallyDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetTallyEntriesByUnitCode()
        {
            var init = new DatastoreInitializer();

            var unit = init.Units.First();
            var subpop = init.Subpops[0];
            var stratum = subpop.StratumCode;
            var sampleGroup = subpop.SampleGroupCode;
            var species = subpop.SpeciesCode;
            var liveDead = subpop.LiveDead;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TallyDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));
                var tlds = new TallyLedgerDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var tpds = new TallyPopulationDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var pop = tpds.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

                // insert entry using InsertTallyAction
                datastore.InsertTallyAction(new TallyAction(unit, pop) { STM = true, KPI = 123 });
                var tallyEntries = datastore.GetTallyEntriesByUnitCode(unit);
                tallyEntries.Should().HaveCount(1);
                tallyEntries.Should().Contain(x => x.STM == true && x.KPI == 123);

                // add another entry using insertTallyLedger
                tlds.InsertTallyLedger(new TallyLedger(unit, pop));
                tallyEntries = datastore.GetTallyEntriesByUnitCode(unit);
                tallyEntries.Should().HaveCount(2);

                // inset a tally ledger with plot number
                // and conferm that GetTallyEntriesByUnitCode doesn't return plot tally entries
                plotds.AddNewPlot(unit);
                datastore.InsertTallyAction(new TallyAction(unit, 1, pop));
                tallyEntries = datastore.GetTallyEntriesByUnitCode(unit);
                tallyEntries.Should().HaveCount(2);
            }
        }

        //[Fact]
        //public void GetTallyEntriesByUnitCodeIncludeUntallied()
        //{
        //    var init = new DatastoreInitializer();

        //    var unit = init.Units.First();
        //    var subpop = init.Subpops[0];
        //    var stratum = subpop.StratumCode;
        //    var sampleGroup = subpop.SampleGroupCode;
        //    var species = subpop.SpeciesCode;
        //    var liveDead = subpop.LiveDead;

        //    var path = base.GetTempFilePath(".crz3");
        //    using (var database = init.CreateDatabase(path))
        //    {
        //        var datastore = new TallyDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));
        //        var tlds = new TallyLedgerDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
        //        var tpds = new TallyPopulationDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
        //        var plotds = new PlotDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

        //        var pop = tpds.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

        //        // insert entry using InsertTallyAction
        //        datastore.InsertTallyAction(new TallyAction(unit, pop) { STM = true, KPI = 123 });
        //        var tallyEntries = datastore.GetTallyEntriesByUnitCodeIncludeUntallied(unit);
        //        tallyEntries.Should().HaveCount(1);
        //        tallyEntries.Should().Contain(x => x.STM == true && x.KPI == 123);

        //        System.Threading.Thread.Sleep(1000); // sleep so that createdTS values aren't all the same

        //        // add another entry using insertTallyLedger
        //        tlds.InsertTallyLedger(new TallyLedger(unit, pop));
        //        tallyEntries = datastore.GetTallyEntriesByUnitCodeIncludeUntallied(unit);
        //        tallyEntries.Should().HaveCount(2);

        //        System.Threading.Thread.Sleep(1000); // sleep so that createdTS values aren't all the same

        //        // inset a tally ledger with plot number
        //        // and conferm that GetTallyEntriesByUnitCode doesn't return plot tally entries
        //        plotds.AddNewPlot(unit);
        //        datastore.InsertTallyAction(new TallyAction(unit, 1, pop));
        //        tallyEntries = datastore.GetTallyEntriesByUnitCodeIncludeUntallied(unit);
        //        tallyEntries.Should().HaveCount(2);

        //        tallyEntries.Count(x => x.IsDeleted == true).Should().Be(0);

        //        var toDeleteTLid = tallyEntries.First().TallyLedgerID;
        //        datastore.DeleteTallyEntry(toDeleteTLid);
        //        tallyEntries = datastore.GetTallyEntriesByUnitCodeIncludeUntallied(unit);
        //        tallyEntries.Should().HaveCount(2);
        //        tallyEntries.First().IsDeleted.Should().Be(true);
        //        tallyEntries.Count(x =>  x.IsDeleted == true).Should().Be(1);
        //    }
        //}

        [Theory]
        [InlineData("st4", "sg2", null, null, SampleResult.C)]// non sample, null values
        [InlineData("st4", "sg2", null, null, SampleResult.M)]// sample, null values
        [InlineData("st4", "sg2", "", "", SampleResult.C)]//non sample, not tally by subpop
        [InlineData("st3", "sg1", "sp1", "L", SampleResult.C)]//non sample, tally by subpop
        [InlineData("st4", "sg2", "", "", SampleResult.M)]//sample, not tally by subpop
        [InlineData("st3", "sg1", "sp1", "L", SampleResult.M)]//sample, tally by subpop
        [InlineData("st4", "sg2", "", "", SampleResult.I)]// not tally by subpop - insurance
        [InlineData("st3", "sg1", "sp1", "L", SampleResult.I)]// tally by subpop - insurance
        public void InsertTallyAction(string stratumCode, string sgCode, string species, string liveDead, SampleResult sampleResult)
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var treeCount = 50;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TallyDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));
                var tpds = new TallyPopulationDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var treeds = new TreeDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tallyPops = database.QueryGeneric($"Select * from TallyPopulation WHERE StratumCode = '{stratumCode}';")
                    .ToArray();

                var pop = tpds.GetTallyPopulation(unitCode, stratumCode, sgCode, species, liveDead);

                pop.Should().NotBeNull();

                var tallyAction = new TallyAction(unitCode, pop)
                {
                    SampleResult = sampleResult,
                    TreeCount = treeCount,
                };

                var entry = datastore.InsertTallyAction(tallyAction);

                entry.TallyLedgerID.Should().NotBeEmpty();

                ValidateTallyEntry(entry, sampleResult == SampleResult.M || sampleResult == SampleResult.I);

                var entryAgain = datastore.GetTallyEntry(entry.TallyLedgerID);

                ValidateTallyEntry(entryAgain, sampleResult == SampleResult.M || sampleResult == SampleResult.I);

                //var tree = database.From<Tree>().Where("TreeID = @p1").Query(entry.TreeID).FirstOrDefault();

                if (sampleResult == SampleResult.M || sampleResult == SampleResult.I)
                {
                    var tree = treeds.GetTree(entry.TreeID);

                    tree.Should().NotBeNull();

                    tree.TreeID.Should().Be(entry.TreeID);
                    tree.StratumCode.Should().Be(stratumCode);
                    tree.SampleGroupCode.Should().Be(sgCode);
                    tree.SpeciesCode.Should().Be(pop.SpeciesCode);
                    tree.CountOrMeasure.Should().Be(sampleResult.ToString());

                    if (string.IsNullOrEmpty(pop.LiveDead))
                    { tree.LiveDead.Should().Be(pop.DefaultLiveDead); }
                    else
                    { tree.LiveDead.Should().Be(pop.LiveDead); }
                }

                var tallyPopulate = tpds.GetTallyPopulationsByUnitCode(unitCode).Where(x => (x.SpeciesCode ?? "") == (species ?? "")).Single();

                tallyPopulate.TreeCount.Should().Be(treeCount);
            }
        }

        private void ValidateTallyEntry(TallyEntry entry, bool isSample, string entryType = "tally")
        {
            entry.Should().NotBeNull();
            entry.TallyLedgerID.Should().NotBeNull();

            if (isSample)
            {
                entry.TreeNumber.Should().NotBeNull();
            }
            else
            {
                entry.TreeNumber.Should().BeNull();
            }

            entry.EntryType.Should().BeEquivalentTo(entryType);
        }

        [Fact]
        public void InsertTallyLedger()
        {
            var init = new DatastoreInitializer();

            string unitCode = init.UnitStrata[0].CuttingUnitCode;
            string stratum = init.UnitStrata[0].StratumCode;
            string sampleGroup = init.Subpops[0].SampleGroupCode;
            string species = init.Subpops[0].SpeciesCode;
            string liveDead = init.Subpops[0].LiveDead;

            int treeCountDiff = 1;
            int kpi = 1;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TallyDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));
                var tpds = new TallyPopulationDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);
                var tlds = new TallyLedgerDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var pop = tpds.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();
                VerifyTallyPopulation(pop);
                pop.TreeCount.Should().Be(0);
                pop.SumKPI.Should().Be(0);

                var tallyLedger = new TallyLedger(unitCode, pop);
                tallyLedger.TreeCount = treeCountDiff;
                tallyLedger.KPI = 1;

                tlds.InsertTallyLedger(tallyLedger);

                database.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger;").Should().Be(1);
                database.ExecuteScalar<int>("SELECT sum(TreeCount) FROM TallyLedger;").Should().Be(treeCountDiff);

                var popAfter = tpds.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                popAfter.TreeCount.Should().Be(treeCountDiff);
                popAfter.SumKPI.Should().Be(kpi);
            }
        }

        private static void VerifyTallyPopulation(TallyPopulation result, string species = null)
        {
            if (species != null)
            {
                result.SpeciesCode.Should().Be(species);
            }

            result.SampleGroupCode.Should().NotBeNullOrEmpty();
            result.StratumCode.Should().NotBeNullOrEmpty();

            result.Frequency.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DeleteTallyEntry()
        {
            var init = new DatastoreInitializer();

            var unitCode = "u1";
            var stratumCode = "st3";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var tree_guid = Guid.NewGuid().ToString();
            var tallyLedgerID = Guid.NewGuid().ToString();
            var treeCount = 1;
            var cruiseID = init.CruiseID;

            using (var database = init.CreateDatabase())
            {
                var datastore = new TallyDataservice(database, cruiseID, TestDeviceInfoService.TEST_DEVICEID, new SamplerStateDataservice(database, init.CruiseID, TestDeviceInfoService.TEST_DEVICEID));
                var tpds = new TallyPopulationDataservice(database, cruiseID, TestDeviceInfoService.TEST_DEVICEID);

                var tallyPop = tpds.GetTallyPopulation(unitCode, stratumCode, sgCode, species, liveDead);

                tallyPop.Should().NotBeNull("tallyPop");

                var tallyEntry = new TallyAction(unitCode, tallyPop)
                {
                    TreeCount = treeCount
                };

                var entry = datastore.InsertTallyAction(tallyEntry);

                datastore.DeleteTallyEntry(entry.TallyLedgerID);

                var tallyPopAgain = tpds.GetTallyPopulationsByUnitCode(unitCode)
                    .Where(x => x.StratumCode == stratumCode
                        && x.SampleGroupCode == sgCode
                        && x.SpeciesCode == species).Single();

                tallyPopAgain.TreeCount.Should().Be(0, "TreeCount");
                tallyPopAgain.SumKPI.Should().Be(0, "SumKPI");

                database.ExecuteScalar<int>("SELECT count(*) FROM Tree WHERE TreeID = @p1", entry.TreeID).Should().Be(0, "tree should be deleted");
            }
        }
    }
}