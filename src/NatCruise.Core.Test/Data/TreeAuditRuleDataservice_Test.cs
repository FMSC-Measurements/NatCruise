using FluentAssertions;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.Data
{
    public class TreeAuditRuleDataservice_Test : TestBase
    {
        public TreeAuditRuleDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void UpsertTreeAuditRule()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();


            var ds = new TreeAuditRuleDataservice(db, init.CruiseID, init.DeviceID);

            var tarRaw = new CruiseDAL.V3.Models.TreeAuditRule()
            {
                TreeAuditRuleID = Guid.NewGuid().ToString(),
                CruiseID = init.CruiseID,
                Field = "DBH",
                Min = 10,
                Max = 20,
            };
            db.Insert(tarRaw);

            var tar = ds.GetTreeAuditRule(tarRaw.TreeAuditRuleID);

            tar.Min = 15;

            ds.UpsertTreeAuditRule(tar);

            var tarAgain = ds.GetTreeAuditRule(tar.TreeAuditRuleID);

            tarAgain.Should().BeEquivalentTo(tar);
        }
    }
}
