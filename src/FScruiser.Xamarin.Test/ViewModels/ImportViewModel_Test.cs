using CruiseDAL;
using CruiseDAL.V3.Models;
using FluentAssertions;
using FScruiser.XF.Data;
using FScruiser.XF.Services;
using Moq;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.ViewModels
{
    public class ImportViewModel_Test : HostedTestBase
    {
        public ImportViewModel_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AnalizeCruise_newCruiseIntoEmptyDb()
        {
            var init = new DatastoreInitializer();

            var srcPath = GetTempFilePath(".crz3", "AnalizeCruise_newCruiseIntoEmptyDb_Src");
            var destPath = GetTempFilePath(".crz3", "AnalizeCruise_newCruiseIntoEmptyDb_Dest");

            using var destDb = new CruiseDatastore_V3(destPath, true);
            using var srcDb = init.CreateDatabase(srcPath);

            var mockFileDialogService = new Mock<IFileDialogService>();
            var mockFileSystemService = new Mock<IFileSystemService>();
            var mockNatCruiseDialogService = new Mock<INatCruiseDialogService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruiseNavService = new Mock<ICruiseNavigationService>();
            var deviceInfoService = new TestDeviceInfoService();


            DataContext.Database = destDb;


            var importVm = new ImportViewModel(DataContext, mockFileDialogService.Object, mockFileSystemService.Object, mockNatCruiseDialogService.Object,
                mockLoggingService.Object, mockCruiseNavService.Object, deviceInfoService);

            var analizeResult = importVm.AnalizeCruise(srcPath, init.CruiseID, out var errors);
            analizeResult.Should().BeTrue();

        }

        [Fact]
        public void AnalizeCruise_with_clonedDb()
        {
            var init = new DatastoreInitializer();

            var srcPath = GetTempFilePath(".crz3", "AnalizeCruise_with_clonedDb_Src");
            var destPath = GetTempFilePath(".crz3", "AnalizeCruise_with_clonedDb_Dest");

            using var destDb = init.CreateDatabase(destPath);
            using var srcDb = new CruiseDatastore_V3(srcPath, true);
            destDb.BackupDatabase(srcDb);

            var mockFileDialogService = new Mock<IFileDialogService>();
            var mockFileSystemService = new Mock<IFileSystemService>();
            var mockNatCruiseDialogService = new Mock<INatCruiseDialogService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruiseNavService = new Mock<ICruiseNavigationService>();
            var deviceInfoService = new TestDeviceInfoService();

            DataContext.Database = destDb;

            var importVm = new ImportViewModel(DataContext, mockFileDialogService.Object, mockFileSystemService.Object, mockNatCruiseDialogService.Object,
                mockLoggingService.Object, mockCruiseNavService.Object, deviceInfoService);

            var analizeResult = importVm.AnalizeCruise(srcPath, init.CruiseID, out var errors);
            analizeResult.Should().BeTrue();

        }


        // in FScruiser when a cruise is imported with the same sale number
        // as another it is possible for the saleID might end up different from
        // the saleID on the original cruise file. So this test for what happens
        // the sale ID is different on two files.
        [Fact]
        public void AnalizeCruise_SaleID_Mismatch()
        {
            var init = new DatastoreInitializer();

            var srcPath = GetTempFilePath(".crz3", "AnalizeCruise_SaleID_Mismatch_Src");
            var destPath = GetTempFilePath(".crz3", "AnalizeCruise_SaleID_Mismatch_Dest");

            using var destDb = init.CreateDatabase(destPath);
            using var srcDb = new CruiseDatastore_V3(srcPath, true);
            destDb.BackupDatabase(srcDb);

            var srcSale = srcDb.From<Sale>().Query().Single();
            srcSale.SaleID = Guid.NewGuid().ToString();
            srcDb.Update(srcSale);


            var mockFileDialogService = new Mock<IFileDialogService>();
            var mockFileSystemService = new Mock<IFileSystemService>();
            var mockNatCruiseDialogService = new Mock<INatCruiseDialogService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruiseNavService = new Mock<ICruiseNavigationService>();
            var deviceInfoService = new TestDeviceInfoService();

            DataContext.Database = destDb;

            var importVm = new ImportViewModel(DataContext, mockFileDialogService.Object, mockFileSystemService.Object, mockNatCruiseDialogService.Object,
                mockLoggingService.Object, mockCruiseNavService.Object, deviceInfoService);

            var analizeResult = importVm.AnalizeCruise(srcPath, init.CruiseID, out var errors);
            analizeResult.Should().BeTrue();

        }

        [Fact]
        public async void ImportCruise()
        {
            var init = new DatastoreInitializer();

            var srcPath = GetTempFilePath(".crz3", "ImportCruise_Src");
            var destPath = GetTempFilePath(".crz3", "ImportCruise_Dest");

            using var destDb = init.CreateDatabase(destPath);
            using var srcDb = new CruiseDatastore_V3(srcPath, true);
            destDb.BackupDatabase(srcDb);

            var mockFileDialogService = new Mock<IFileDialogService>();
            var mockFileSystemService = new Mock<IFileSystemService>();
            var mockNatCruiseDialogService = new Mock<INatCruiseDialogService>();
            var mockLoggingService = new Mock<ILoggingService>();
            var mockCruiseNavService = new Mock<ICruiseNavigationService>();
            var deviceInfoService = new TestDeviceInfoService();

            DataContext.Database = destDb;

            var importVm = new ImportViewModel(DataContext, mockFileDialogService.Object, mockFileSystemService.Object, mockNatCruiseDialogService.Object,
                mockLoggingService.Object, mockCruiseNavService.Object, deviceInfoService);

            var analizeResult = await importVm.ImportCruise(init.CruiseID, srcPath);
            analizeResult.Should().BeTrue();
        }
    }
}
