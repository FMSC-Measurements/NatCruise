using Bogus;
using CruiseDAL;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test_Logs : Datastore_TestBase
    {
        public CuttingUnitDatastore_Test_Logs(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void InsertLog_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_guid = datastore.CreateMeasureTree("u1", "st1", "sg1");

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
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_guid = datastore.CreateMeasureTree("u1", "st1", "sg1");

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
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var treeID = datastore.CreateMeasureTree("u1", "st1", "sg1");

                var log = new Log() { TreeID = treeID, LogNumber = 1 };
                datastore.InsertLog(log);

                datastore.DeleteLog(log.LogID);

                var logAgain = datastore.GetLog(log.LogID);
                logAgain.Should().BeNull();
            }
        }
    }
}
