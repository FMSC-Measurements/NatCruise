using FScruiser.XF.Services;
using FScruiser.XF.TestServices;
using Moq;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Test;
using Prism.Ioc;
using System;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    public class TestPlatformInitializer : Prism.IPlatformInitializer
    {
        public Xunit.Abstractions.ITestOutputHelper TestOutput { get; set; }
        public TestDialogService TestDialogService { get; private set; }

        public TestPlatformInitializer(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var mockSoundService = new Mock<ISoundService>();
            var mockDialogService = new Mock<IDialogService>();
            var mockFileDialogService = new Mock<IFileDialogService>();
            var mockTallySettingsDataservice = new Mock<ITallySettingsDataService>();
            var mockAppInfoService = new Mock<IAppInfoService>();
            //var mockNavService = new Mock<TestNavigationService>();

            containerRegistry.Register<ICruiseDialogService, XamarinCruiseDialogService>();

            containerRegistry.Register<ICruiseNavigationService, TestNavigationService>();
            //var navigationService = new TestNavigationService();
            //containerRegistry.RegisterInstance<INavigationService>(navigationService);
            //containerRegistry.RegisterInstance<PageNavigationService>(navigationService);

            containerRegistry.RegisterInstance<IAppInfoService>(mockAppInfoService.Object);
            containerRegistry.RegisterInstance<ITallySettingsDataService>(mockTallySettingsDataservice.Object);
            containerRegistry.RegisterInstance<ISoundService>(mockSoundService.Object);
            containerRegistry.RegisterInstance<IDialogService>(mockDialogService.Object);
            containerRegistry.RegisterInstance<IFileDialogService>(mockFileDialogService.Object);
            containerRegistry.RegisterInstance<ILoggingService>(new TestLogger(TestOutput));
            //containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new TestLogger(TestOutput));

            containerRegistry.RegisterInstance<IFileSystemService>(new TestFileSystemService());
            containerRegistry.RegisterInstance<IDataserviceProvider>(new TestDataserviceProvicer());
        }
    }
}