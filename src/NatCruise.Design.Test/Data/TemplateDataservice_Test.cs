using FluentAssertions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Test;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Design.Test.Data
{
    public class TemplateDataservice_Test : TestBase
    {
        public TemplateDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        #region species

        [Fact]
        public void AddSpecies()
        {
            var speciesCode = "ABC";
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var sp = new Species
            {
                SpeciesCode = speciesCode,
            };

            ds.AddSpecies(sp);

            db.ExecuteScalar<int>("SELECT count(*) FROM Species WHERE SpeciesCode = @p1", speciesCode)
                .Should().Be(1);
        }

        [Fact]
        public void GetSpecies()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var species = ds.GetSpecies();
            species.Should().NotBeEmpty();
        }

        [Fact]
        public void UpsertSpecies_Insert()
        {
            var speciesCode = "ABC";
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var sp = new Species
            {
                SpeciesCode = speciesCode,
            };

            ds.UpsertSpecies(sp);

            db.ExecuteScalar<int>("SELECT count(*) FROM Species WHERE SpeciesCode = @p1", speciesCode)
                .Should().Be(1);

            sp.ContractSpecies = Rand.String();
            sp.FIACode = Rand.Word();

            ds.UpsertSpecies(sp);

            var spAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(speciesCode).Single();
            spAgain.Should().BeEquivalentTo(sp);
        }

        #endregion species

        #region SampleGroupDefaults

        [Fact]
        public void GetSampleGroupDefaults()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var sgds = ds.GetSampleGroupDefaults();
        }

        [Fact]
        public void AddSampleGroupDefault()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var newSgd = new SampleGroupDefault()
            {
                SampleGroupDefaultID = Guid.NewGuid().ToString(),
                Description = Rand.Word(),
            };

            ds.AddSampleGroupDefault(newSgd);

            var sgdAgian = ds.GetSampleGroupDefaults().Single();
            sgdAgian.Should().BeEquivalentTo(newSgd);
        }

        [Fact]
        public void UpdateSampleGroupDefault()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var newSgd = new SampleGroupDefault()
            {
                SampleGroupDefaultID = Guid.NewGuid().ToString(),
                Description = Rand.Word(),
            };

            ds.AddSampleGroupDefault(newSgd);

            newSgd.Description = Rand.Word();
            ds.UpdateSampleGroupDefault(newSgd);

            var sgdAgain = ds.GetSampleGroupDefaults().Single();

            sgdAgain.Should().BeEquivalentTo(newSgd);
        }

        #endregion SampleGroupDefaults
    }
}