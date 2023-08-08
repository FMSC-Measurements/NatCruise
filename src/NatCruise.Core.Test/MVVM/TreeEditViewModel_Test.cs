﻿using FluentAssertions;
using Moq;
using NatCruise.Data;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

using CuttingUnit_Stratum = CruiseDAL.V3.Models.CuttingUnit_Stratum;
using SampleGroup = CruiseDAL.V3.Models.SampleGroup;
using Stratum = CruiseDAL.V3.Models.Stratum;
using SubPopulation = CruiseDAL.V3.Models.SubPopulation;

namespace NatCruise.Test.MVVM
{
    public class TreeEditViewModel_Test : TestBase
    {
        public TreeEditViewModel_Test(ITestOutputHelper output) : base(output)
        {
        }

        protected TreeEditViewModel MakeTreeEditViewModel(IDataserviceProvider dsp,
            INatCruiseDialogService dialogServ = null,
            INatCruiseNavigationService navServ = null,
            ILoggingService logServ = null,
            ICruisersDataservice cruisersDataservice = null)
        {
            dialogServ ??= new Mock<INatCruiseDialogService>().Object;
            navServ ??= new Mock<INatCruiseNavigationService>().Object;
            logServ ??= new Mock<ILoggingService>().Object;
            cruisersDataservice ??= new Mock<ICruisersDataservice>().Object;

            return new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISpeciesDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                dsp.GetDataservice<ITreeDataservice>(),
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                dsp.GetDataservice<ICruiseLogDataservice>(),
                cruisersDataservice,
                dialogServ,
                navServ,
                logServ);
        }

        [Fact]
        public void Load_With_TreeID()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeEditVM = MakeTreeEditViewModel(dsp);
            var treeDataService = treeEditVM.TreeDataservice;

            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            treeEditVM.Tree.Should().NotBeNull();
        }

        [Fact]
        public void EditTreeRemarks()
        {
            var init = new DatastoreInitializer();
            using var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeEditVM = MakeTreeEditViewModel(dsp);
            var treeDataService = treeEditVM.TreeDataservice;

            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            var remarksValue = "something";
            treeEditVM.Remarks = remarksValue;

            var tree = db.From<CruiseDAL.V3.Models.Tree>().Query().Single();
            var treeMeasurment = db.From<CruiseDAL.V3.Models.TreeMeasurment>().Query().Single();
            var tallyLedger = db.From<CruiseDAL.V3.Models.TallyLedger>().Query().Single();
            treeMeasurment.Remarks.Should().Be(remarksValue);
        }

        [Theory]    //st          //sg          //sp                            //ld                            //expect sg dialog  //expect sp dialog
        [InlineData("st1", "st2", "sg1", "sg1", (string)null,   (string)null,   (string)null,   (string)null,   false,              true)]
        [InlineData("st1", "st2", "sg1", "sg1", "sp1",          "sp1",          "L",            "L",            false,              false)] //just change st
        [InlineData("st1", "st2", "sg1", "sg2", "sp1",          "sp1",          "L",            "L",            false,              false)] //

        public void EditStratum(
            string stratumBefore,
            string straumAfter,
            string sgBefore,
            string sgAfter,
            string speciesBefore,
            string speciesAfter,
            string ldBefore,
            string ldAfter,
            bool expectSgDialog,
            bool expectSpDialog)
        {
            var spDialogTitle = "Select Species";
            var sgDialogTitle = "Select Sample Group";

            var init = new DatastoreInitializer()
            {
                Units = new[] { "u1" },
                Strata = new[]
                {
                    new Stratum{ StratumCode = "st1", Method = "PNT" },
                    new Stratum{ StratumCode = "st2", Method = "PCM" },
                },
                UnitStrata = new[] {
                    new CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st1"},
                    new CuttingUnit_Stratum {CuttingUnitCode = "u1", StratumCode = "st2"},
                },
                SampleGroups = new[]
                {
                    new SampleGroup{SampleGroupCode = "sg1", StratumCode = "st1"},
                    new SampleGroup{SampleGroupCode = "sg2", StratumCode = "st1"},
                    new SampleGroup{SampleGroupCode = "sg1", StratumCode = "st2"},
                },
                Subpops = new[]
                {
                    new SubPopulation {StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L"},
                    new SubPopulation {StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp2", LiveDead = "L"},
                    new SubPopulation {StratumCode = "st1", SampleGroupCode = "sg2", SpeciesCode = "sp1", LiveDead = "L"},

                    new SubPopulation {StratumCode = "st2", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L"},
                    //new SubPopulation {StratumCode = "st2", SampleGroupCode = "sg2", SpeciesCode = "sp1", LiveDead = "L"},
                }
            };
            using var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            // set up mock dialog service
            var mockDialogService = new Mock<INatCruiseDialogService>();
            if (expectSgDialog)
            { mockDialogService.Setup(x => x.AskValueAsync(It.Is<string>(x => x == spDialogTitle), It.IsAny<string[]>())).ReturnsAsync(speciesAfter); }
            if (expectSgDialog)
            { mockDialogService.Setup(x => x.AskValueAsync(It.Is<string>(x => x == sgDialogTitle), It.IsAny<string[]>())).ReturnsAsync(sgAfter); }

            var treeEditVM = MakeTreeEditViewModel(dsp, dialogServ:mockDialogService.Object);
            var treeDataService = treeEditVM.TreeDataservice;

            var treeID = treeDataService.InsertManualTree("u1", stratumBefore, sgBefore, speciesBefore, ldBefore);
            //var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1", "sp1", "L");
            //var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            var newSt = "st2";
            treeEditVM.StratumCode = newSt;

            var tree = db.From<CruiseDAL.V3.Models.Tree>().Query().Single();
            var treeMeasurment = db.From<CruiseDAL.V3.Models.TreeMeasurment>().Query().Single();
            var tallyLedger = db.From<CruiseDAL.V3.Models.TallyLedger>().Query().Single();
            tree.StratumCode.Should().Be(newSt);

            // verify AskValueAsync calls
            if (expectSgDialog)
            { mockDialogService.Verify(x => x.AskValueAsync(It.Is<string>(x => x == sgDialogTitle), It.IsAny<string[]>()), Times.Once); }
            else
            { mockDialogService.Verify(x => x.AskValueAsync(It.Is<string>(x => x == sgDialogTitle), It.IsAny<string[]>()), Times.Never); }
            if (expectSpDialog)
            { mockDialogService.Verify(x => x.AskValueAsync(It.Is<string>(x => x == spDialogTitle), It.IsAny<string[]>()), Times.Once); }
            else
            { mockDialogService.Verify(x => x.AskValueAsync(It.Is<string>(x => x == spDialogTitle), It.IsAny<string[]>()), Times.Never); }
        }

        [Fact]
        public void Issue_107()
        {
            var init = new DatastoreInitializer();
            var path = base.GetTempFilePath(".crz3");
            using var db = init.CreateDatabase(path);

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeFieldSetup = new CruiseDAL.V3.Models.TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                Field = "TotalHeight",
                StratumCode = "st1",
            };
            db.Insert(treeFieldSetup);

            var treeAuditRules = new[]
            {
                new CruiseDAL.V3.Models.TreeAuditRule
                {
                    TreeAuditRuleID = Guid.NewGuid().ToString(),
                    CruiseID = init.CruiseID,
                    Field = "TotalHeight",
                    Min = 10.0,
                    Max = 100.0,
                },

                new CruiseDAL.V3.Models.TreeAuditRule
                {
                    TreeAuditRuleID = Guid.NewGuid().ToString(),
                    CruiseID = init.CruiseID,
                    Field = "TotalHeight",
                    Min = 20.0,
                    Max = 100.0,
                },
            };
            foreach (var tar in treeAuditRules)
            {
                db.Insert(tar);

                db.Insert(new CruiseDAL.V3.Models.TreeAuditRuleSelector
                {
                    CruiseID = init.CruiseID,
                    TreeAuditRuleID = tar.TreeAuditRuleID,
                    SpeciesCode = "sp1",
                    LiveDead = null,
                    PrimaryProduct = null,
                });
            }

            var treeEditVM = MakeTreeEditViewModel(dsp);
            var treeDataService = treeEditVM.TreeDataservice;

            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1", "sp1");
            var treeMeasurment = db.From<CruiseDAL.V3.Models.TreeMeasurment>().Query().Single();
            treeMeasurment.TotalHeight = 20;
            treeMeasurment.DBH = 10;
            db.Update(treeMeasurment);

            treeEditVM.Load(treeID);
            treeEditVM.RefreshErrorsAndWarnings();
            treeEditVM.ErrorsAndWarnings.Should().BeEmpty();

            treeEditVM.Tree.TotalHeight = 10;
            treeDataService.UpdateTree(treeEditVM.Tree);
            treeEditVM.RefreshErrorsAndWarnings();
            treeEditVM.ErrorsAndWarnings.Should().HaveCount(1);

            treeEditVM.Tree.TotalHeight = 9;
            treeDataService.UpdateTree(treeEditVM.Tree);
            treeEditVM.RefreshErrorsAndWarnings();
            treeEditVM.ErrorsAndWarnings.Should().HaveCount(2);
        }
    }
}