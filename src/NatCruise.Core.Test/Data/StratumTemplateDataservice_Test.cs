using FluentAssertions;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using StratumTemplate = NatCruise.Models.StratumTemplate;
using StratumTemplateLogFieldSetup = NatCruise.Models.StratumTemplateLogFieldSetup;

namespace NatCruise.Test.Data
{
    public class StratumTemplateDataservice_Test : TestBase
    {
        public StratumTemplateDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpsertStratumTemplate()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new StratumTemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

            var st = new NatCruise.Models.StratumTemplate()
            {
                StratumTemplateName = "something",
            };

            ds.UpsertStratumTemplate(st);

            db.From<StratumTemplate>().Count().Should().Be(1);
        }

        #region StratumTemplateLogFieldSetup

        [Fact]
        public void UpsertStratumTemplateLogFieldSetup_Insert()
        {
            var initializer = new DatastoreInitializer();
            using var db = initializer.CreateDatabase();
            var ds = new StratumTemplateDataservice(db, initializer.CruiseID, initializer.DeviceID);

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
