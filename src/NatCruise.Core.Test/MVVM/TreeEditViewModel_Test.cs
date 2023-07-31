using FluentAssertions;
using Moq;
using NatCruise.Data;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Test.MVVM
{
    public class TreeEditViewModel_Test : TestBase
    {
        public TreeEditViewModel_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Load_With_TreeID()
        {
            var init = new DatastoreInitializer();
            var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeDataService = dsp.GetDataservice<ITreeDataservice>();

            var mockDialogService = new Mock<INatCruiseDialogService>();
            var mockNavService = new Mock<INatCruiseNavigationService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruisersService = new Mock<ICruisersDataservice>();

            var treeEditVM = new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                treeDataService,
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                mockDialogService.Object,
                mockNavService.Object,
                mockCruisersService.Object,
                mockLoggingService.Object);


            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            treeEditVM.Tree.Should().NotBeNull();
        }

        [Fact]
        public void EditTreeRemarks()
        {
            var init = new DatastoreInitializer();
            var db = init.CreateDatabase();

            var dsp = new DataserviceProviderBase(db, new TestDeviceInfoService());
            dsp.CruiseID = init.CruiseID;

            var treeDataService = dsp.GetDataservice<ITreeDataservice>();

            var mockDialogService = new Mock<INatCruiseDialogService>();
            var mockNavService = new Mock<INatCruiseNavigationService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruisersService = new Mock<ICruisersDataservice>();

            var treeEditVM = new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                treeDataService,
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                mockDialogService.Object,
                mockNavService.Object,
                mockCruisersService.Object,
                mockLoggingService.Object);


            var treeID = treeDataService.InsertManualTree("u1", "st1", "sg1");

            treeEditVM.Load(treeID);

            var remarksValue = "something";
            treeEditVM.Remarks = remarksValue;

            var tree = db.From<CruiseDAL.V3.Models.Tree>().Query().Single();
            var treeMeasurment = db.From<CruiseDAL.V3.Models.TreeMeasurment>().Query().Single();
            var tallyLedger = db.From<CruiseDAL.V3.Models.TallyLedger>().Query().Single();
            treeMeasurment.Remarks.Should().Be(remarksValue);
        }

        [Fact]
        public void Issue_107()
        {
            var init = new DatastoreInitializer();
            var path = base.GetTempFilePath(".crz3");
            var db = init.CreateDatabase(path);

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
            foreach(var tar in treeAuditRules)
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

            var treeDataService = dsp.GetDataservice<ITreeDataservice>();

            var mockDialogService = new Mock<INatCruiseDialogService>();
            var mockNavService = new Mock<INatCruiseNavigationService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruisersService = new Mock<ICruisersDataservice>();

            var treeEditVM = new TreeEditViewModel(dsp.GetDataservice<IStratumDataservice>(),
                dsp.GetDataservice<ISampleGroupDataservice>(),
                dsp.GetDataservice<ISubpopulationDataservice>(),
                treeDataService,
                dsp.GetDataservice<ITreeErrorDataservice>(),
                dsp.GetDataservice<ITreeFieldValueDataservice>(),
                mockDialogService.Object,
                mockNavService.Object,
                mockCruisersService.Object,
                mockLoggingService.Object);


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
