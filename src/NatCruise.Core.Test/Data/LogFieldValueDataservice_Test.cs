using FluentAssertions;
using FMSC.ORM.Core;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

using Tree = CruiseDAL.V3.Models.Tree;

namespace NatCruise.Test.Data
{
    public class LogFieldValueDataservice_Test : TestBase
    {
        public LogFieldValueDataservice_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetLogFieldValues()
        {
            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";
            var sp = "sp1";

            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new LogFieldValueDataservice(db, init.CruiseID, init.DeviceID);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
            };
            db.Insert(tree);

            var logFields = new string[]
            {
                "Grade", "ExportGrade", "SeenDefect", "PercentRecoverable", "SmallEndDiameter", "LargeEndDiameter",
                "GrossBoardFoot", "NetBoardFoot", "GrossCubicFoot", "NetCubicFoot", "BoardFootRemoved",
                "CubicFootRemoved", "DIBClass", "BarkThickness"
            };

            SetupLogFields(db, tree, logFields);

            var log = new CruiseDAL.V3.Models.Log()
            {
                CruiseID = init.CruiseID,
                LogID = Guid.NewGuid().ToString(),
                TreeID = tree.TreeID,
                Grade = "1",
                ExportGrade = "1",
                BoardFootRemoved = 1,
                BarkThickness = 1,
                CubicFootRemoved = 1,
                DIBClass = 1,
                GrossBoardFoot = 1,
                GrossCubicFoot = 1,
                LargeEndDiameter = 1,
                NetBoardFoot = 1,
                NetCubicFoot = 1,
                PercentRecoverable = 1,
                SeenDefect = 1,
                SmallEndDiameter = 1,
                LogNumber = "1",
                Length = 1,
            };

            db.Insert(log);

            var logFieldValues = ds.GetLogFieldValues(log.LogID).ToArray();

            logFieldValues.Should().HaveCount(logFields.Length);
            logFieldValues.Should().OnlyContain(x => x.LogID == log.LogID);

        }

        [Fact]
        public void UpdateLogFieldValue()
        {
            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";
            var sp = "sp1";

            var init = new DatastoreInitializer();

            using var db = init.CreateDatabase();

            var ds = new LogFieldValueDataservice(db, init.CruiseID, init.DeviceID);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = unit,
                StratumCode = stratum,
                SampleGroupCode = sg,
                SpeciesCode = sp,
            };
            db.Insert(tree);

            var logFields = new string[]
            {
                "Grade", "ExportGrade", "SeenDefect", "PercentRecoverable", "SmallEndDiameter", "LargeEndDiameter",
                "GrossBoardFoot", "NetBoardFoot", "GrossCubicFoot", "NetCubicFoot", "BoardFootRemoved",
                "CubicFootRemoved", "DIBClass", "BarkThickness"
            };

            SetupLogFields(db, tree, logFields);

            var log = new CruiseDAL.V3.Models.Log()
            {
                CruiseID = init.CruiseID,
                LogID = Guid.NewGuid().ToString(),
                TreeID = tree.TreeID,
                Grade = "1",
                ExportGrade = "1",
                BoardFootRemoved = 1,
                BarkThickness = 1,
                CubicFootRemoved = 1,
                DIBClass = 1,
                GrossBoardFoot = 1,
                GrossCubicFoot = 1,
                LargeEndDiameter = 1,
                NetBoardFoot = 1,
                NetCubicFoot = 1,
                PercentRecoverable = 1,
                SeenDefect = 1,
                SmallEndDiameter = 1,
                LogNumber = "1",
                Length = 1,
            };

            db.Insert(log);

            var logFieldValues = ds.GetLogFieldValues(log.LogID).ToArray();

            var strFields = new[] { "Grade", "ExportGrade" };

            foreach(var lfv in logFieldValues)
            {
                var field = lfv.Field;
                if(strFields.Contains(field))
                {
                    lfv.Value = 2.ToString();
                }
                else
                {
                    lfv.Value = 2.0;
                }

                ds.UpdateLogFieldValue(lfv);
            }

            var logFieldValuesAgain = ds.GetLogFieldValues(log.LogID).ToArray();
            logFieldValuesAgain.Should().OnlyContain(x => x.Value.ToString() == "2");
        }

        private void SetupLogFields(Datastore db, Tree tree, IEnumerable<string> fields)
        {
            foreach(var (f,i) in fields.Select((f,i) => (f, i)))
            {
                var lfs = new CruiseDAL.V3.Models.LogFieldSetup()
                {
                    CruiseID = tree.CruiseID,
                    Field = f,
                    FieldOrder = 1,
                    StratumCode = tree.StratumCode,
                };
                db.Insert(lfs);


                if(i % 2 == 0) { continue;}

                var lfh = new CruiseDAL.V3.Models.LogFieldHeading()
                {
                    CruiseID = tree.CruiseID,
                    Field = f,
                    Heading = f + "Heading",
                };
                db.Insert(lfh);
            }
        }
    }
}
