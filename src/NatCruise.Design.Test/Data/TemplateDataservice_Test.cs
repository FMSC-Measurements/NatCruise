using FluentAssertions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Test;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

using Species = NatCruise.Models.Species;

namespace NatCruise.Design.Test.Data
{
    public class TemplateDataservice_Test : TestBase
    {
        public TemplateDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

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