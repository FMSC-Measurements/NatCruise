using Bogus;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NatCruise.Cruise.Test.Data
{
    public class LogDataservice_Test
    {
        [Fact]
        public void InsertLog_test()
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {

                var datastore = new LogDataservice(database, init.CruiseID, init.DeviceID);
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var tree_guid = treeDS.InsertManualTree("u1", "st1", "sg1");

                var log = new Log() { TreeID = tree_guid };

                var randomizer = new Randomizer(8675309);

                log.BarkThickness = randomizer.Double();
                log.BoardFootRemoved = randomizer.Double();
                log.CubicFootRemoved = randomizer.Double();
                log.DIBClass = randomizer.Double();
                log.ExportGrade = randomizer.String();
                log.Grade = randomizer.String();
                log.GrossBoardFoot = randomizer.Double();
                log.GrossCubicFoot = randomizer.Double();
                log.LargeEndDiameter = randomizer.Double();
                log.Length = randomizer.Int();
                log.LogNumber = randomizer.Int();
                //log.ModifiedBy = randomizer.String();
                log.NetBoardFoot = randomizer.Double();
                log.NetCubicFoot = randomizer.Double();
                log.PercentRecoverable = randomizer.Double();
                log.SeenDefect = randomizer.Double();
                log.SmallEndDiameter = randomizer.Double();

                datastore.InsertLog(log);

                log.LogID.Should().NotBeNullOrWhiteSpace();
                Guid.TryParse(log.LogID, out Guid log_guid).Should().BeTrue();
                log_guid.Should().NotBe(Guid.Empty);
            }
        }

        [Fact]
        public void UpdateLog_test()
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {

                var datastore = new LogDataservice(database, init.CruiseID, init.DeviceID);
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var tree_guid = treeDS.InsertManualTree("u1", "st1", "sg1");

                var log = new Log() { TreeID = tree_guid, LogNumber = 1 };
                datastore.InsertLog(log);

                var randomizer = new Randomizer(8675309);

                log.BarkThickness = randomizer.Double();
                log.BoardFootRemoved = randomizer.Double();
                log.CubicFootRemoved = randomizer.Double();
                log.DIBClass = randomizer.Double();
                log.ExportGrade = randomizer.String();
                log.Grade = randomizer.String();
                log.GrossBoardFoot = randomizer.Double();
                log.GrossCubicFoot = randomizer.Double();
                log.LargeEndDiameter = randomizer.Double();
                log.Length = randomizer.Int();
                log.LogNumber = randomizer.Int();
                //log.ModifiedBy = randomizer.String(10);
                log.NetBoardFoot = randomizer.Double();
                log.NetCubicFoot = randomizer.Double();
                log.PercentRecoverable = randomizer.Double();
                log.SeenDefect = randomizer.Double();
                log.SmallEndDiameter = randomizer.Double();

                datastore.UpdateLog(log);

                var logAgain = datastore.GetLog(log.LogID);

                var eqivConfig = new EquivalencyAssertionOptions<Log>();
                eqivConfig.Excluding(x => x.CreatedBy);

                logAgain.Should().BeEquivalentTo(log, config: x => x.Excluding(l => l.CreatedBy));
            }
        }

        [Fact]
        public void DeleteLog_test()
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {

                var datastore = new LogDataservice(database, init.CruiseID, init.DeviceID);
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var treeID = treeDS.InsertManualTree("u1", "st1", "sg1");

                var log = new Log() { TreeID = treeID, LogNumber = 1 };
                datastore.InsertLog(log);

                datastore.DeleteLog(log.LogID);

                var logAgain = datastore.GetLog(log.LogID);
                logAgain.Should().BeNull();
            }
        }

        [Fact]
        public void GetLogFields()
        {
            var init = new DatastoreInitializer();
            using (var database = init.CreateDatabase())
            {
                var stratum = "st1";
                var lfs = new CruiseDAL.V3.Models.LogFieldSetup
                {
                    CruiseID = init.CruiseID,
                    StratumCode = stratum,
                    Field = "Grade",
                    FieldOrder = 1,
                };
                database.Insert(lfs);

                var datastore = new LogDataservice(database, init.CruiseID, init.DeviceID);
                var treeDS = new TreeDataservice(database, init.CruiseID, init.DeviceID);

                var tree_guid = treeDS.InsertManualTree("u1", "st1", "sg1");

                var logFields = datastore.GetLogFields(tree_guid);
                logFields.Should().HaveCount(1);

            }
        }
    }
}
