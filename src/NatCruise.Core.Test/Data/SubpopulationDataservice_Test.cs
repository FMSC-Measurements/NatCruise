using FluentAssertions;
using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class SubpopulationDataservice_Test : TestBase
    {
        public SubpopulationDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AddSubpopulation()
        {
            var init = new DatastoreInitializer();
            init.Subpops = new CruiseDAL.V3.Models.SubPopulation[0];
            var db = init.CreateDatabase();

            var ds = new SubpopulationDataservice(db, init.CruiseID, init.DeviceID);

            var sg = init.SampleGroups[0];
            var newSubpop = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = init.Species[0],
                LiveDead = "L",
            };

            ds.AddSubpopulation(newSubpop);

            var subPops = ds.GetSubpopulations(sg.StratumCode, sg.SampleGroupCode);
            var subPopAgain = subPops.Single();
            subPopAgain.Should().BeEquivalentTo(newSubpop);
        }

        [Fact]
        public void DeleteSubpopulation()
        {
            var init = new DatastoreInitializer();
            init.Subpops = new CruiseDAL.V3.Models.SubPopulation[0];
            var db = init.CreateDatabase();

            var ds = new SubpopulationDataservice(db, init.CruiseID, init.DeviceID);

            var sg = init.SampleGroups[0];
            var newSubpop = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = init.Species[0],
                LiveDead = "L",
            };

            ds.AddSubpopulation(newSubpop);

            ds.DeleteSubpopulation(newSubpop);

            var subPops = ds.GetSubpopulations(sg.StratumCode, sg.SampleGroupCode);
            subPops.Should().BeEmpty();
        }

        [Fact]
        public void Exists()
        {
            var init = new DatastoreInitializer();
            init.Subpops = new CruiseDAL.V3.Models.SubPopulation[0];
            var db = init.CreateDatabase();

            var ds = new SubpopulationDataservice(db, init.CruiseID, init.DeviceID);

            var sg = init.SampleGroups[0];
            var newSubpop = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = init.Species[0],
                LiveDead = "L",
            };

            ds.Exists(newSubpop.StratumCode, newSubpop.SampleGroupCode, newSubpop.SpeciesCode, newSubpop.LiveDead)
                .Should().BeFalse();

            ds.AddSubpopulation(newSubpop);

            ds.Exists(newSubpop.StratumCode, newSubpop.SampleGroupCode, newSubpop.SpeciesCode, newSubpop.LiveDead)
                .Should().BeTrue();

            ds.Exists(newSubpop.StratumCode, newSubpop.SampleGroupCode, newSubpop.SpeciesCode, "D")
                .Should().BeFalse();

            var subPop2 = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = init.Species[1],
                LiveDead = "L",
            };

            ds.Exists(subPop2.StratumCode, subPop2.SampleGroupCode, subPop2.SpeciesCode, subPop2.LiveDead)
                .Should().BeFalse();

            ds.AddSubpopulation(subPop2);

            ds.Exists(subPop2.StratumCode, subPop2.SampleGroupCode, subPop2.SpeciesCode, subPop2.LiveDead)
                .Should().BeTrue();

            ds.Exists(subPop2.StratumCode, subPop2.SampleGroupCode, subPop2.SpeciesCode, "D")
                .Should().BeFalse();
        }

        [Fact]
        public void GetSubpopulations()
        {
            var init = new DatastoreInitializer();
            init.Subpops = new CruiseDAL.V3.Models.SubPopulation[0];
            var db = init.CreateDatabase();

            var ds = new SubpopulationDataservice(db, init.CruiseID, init.DeviceID);

            var sg = init.SampleGroups[0];
            var newSubpop = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = init.Species[0],
                LiveDead = "L",
            };

            ds.AddSubpopulation(newSubpop);

            var subpopsAgain = ds.GetSubpopulations(sg.StratumCode, sg.SampleGroupCode);
            var subpopAgain = subpopsAgain.Single();

            subpopAgain.Should().BeEquivalentTo(newSubpop);
            subpopAgain.HasFieldData.Should().BeFalse();
        }


        [Fact]
        public void GetSubpopulations_WithFieldData()
        {
            var init = new DatastoreInitializer();
            init.Subpops = new CruiseDAL.V3.Models.SubPopulation[0];
            var db = init.CreateDatabase();

            var ds = new SubpopulationDataservice(db, init.CruiseID, init.DeviceID);
            var treeds = new TreeDataservice(db, init.CruiseID, init.DeviceID);

            var sg = init.SampleGroups[0];
            var sp = init.Species[0];
            var ld = "L";
            var newSubpop = new Subpopulation
            {
                SubpopulationID = Guid.NewGuid().ToString(),
                SampleGroupCode = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                SpeciesCode = sp,
                LiveDead = ld,
            };

            ds.AddSubpopulation(newSubpop);

            treeds.InsertManualTree("u1", sg.StratumCode, sg.SampleGroupCode, species: sp, liveDead: ld);

            var subpopsAgain = ds.GetSubpopulations(sg.StratumCode, sg.SampleGroupCode);
            var subpopAgain = subpopsAgain.Single();

            subpopAgain.Should().BeEquivalentTo(newSubpop,
                config: opt => opt.Excluding(x => x.HasFieldData));

            subpopAgain.HasFieldData.Should().BeTrue();
        }
    }
}