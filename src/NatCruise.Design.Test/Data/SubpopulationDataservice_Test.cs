﻿using FluentAssertions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Test;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Design.Test.Data
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
    }
}