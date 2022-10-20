using Bogus;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NatCruise.Data;
using NatCruise.Models;
using System;
using Xunit;

namespace NatCruise.Test.Data
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
                //log.ExportGrade = "1";
                log.Grade = "1";
                log.GrossBoardFoot = randomizer.Double();
                log.GrossCubicFoot = randomizer.Double();
                log.LargeEndDiameter = randomizer.Double();
                log.Length = randomizer.Int(min:0);
                log.LogNumber = randomizer.Int(min: 0);
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

                log.BarkThickness = randomizer.Double(0.0);
                log.BoardFootRemoved = randomizer.Double(0.0);
                log.CubicFootRemoved = randomizer.Double(0.0);
                log.DIBClass = randomizer.Double(0.0);
                log.ExportGrade = "";
                log.Grade = "0";
                log.GrossBoardFoot = randomizer.Double(0.0);
                log.GrossCubicFoot = randomizer.Double(0.0);
                log.LargeEndDiameter = randomizer.Double(0.0);
                log.Length = randomizer.Int(0);
                log.LogNumber = randomizer.Int(0);
                //log.ModifiedBy = randomizer.String(10);
                log.NetBoardFoot = randomizer.Double(0.0);
                log.NetCubicFoot = randomizer.Double(0.0);
                log.PercentRecoverable = randomizer.Double(0.0);
                log.SeenDefect = randomizer.Double(0.0);
                log.SmallEndDiameter = randomizer.Double(0.0);

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

        
    }
}