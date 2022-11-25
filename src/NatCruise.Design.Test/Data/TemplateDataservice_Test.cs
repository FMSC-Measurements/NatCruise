using FluentAssertions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Test;
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

            sp.ContractSpecies = Rand.Word();
            sp.FIACode = Rand.Word();

            ds.UpsertSpecies(sp);

            var spAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(speciesCode).Single();
            spAgain.Should().BeEquivalentTo(sp);
        }

        #endregion species

        #region StratumTemplate

        [Fact]
        public void UpsertStratumTemplate()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var st = new StratumTemplate()
            {
                StratumTemplateName = "something",
            };

            ds.UpsertStratumTemplate(st);

            db.From<StratumTemplate>().Count().Should().Be(1);
        }

        #endregion StratumTemplate

        #region StratumTemplateLogFieldSetup

        [Fact]
        public void UpsertStratumTemplateLogFieldSetup_Insert()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new TemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var st = new StratumTemplate()
            {
                StratumTemplateName = "something",
            };
            ds.UpsertStratumTemplate(st);

            var stlfs = new StratumTemplateLogFieldSetup()
            {
                Field = "Grade",
                FieldOrder = 0,
                StratumTemplateName = st.StratumTemplateName,
            };

            ds.UpsertStratumTemplateLogFieldSetup(stlfs);
            db.From<StratumTemplateLogFieldSetup>().Count().Should().Be(1);
        }

        #endregion StratumTemplateLogFieldSetup
    }
}