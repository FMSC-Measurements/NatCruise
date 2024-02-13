using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Services;
using Moq;

namespace NatCruise.Test
{
    public class HostedTestBase : TestBase
    {
        private IHost _testHost;
        public IHost TestHost => _testHost ??= InitHost(ConfigureHost);
        protected IServiceProvider Services => TestHost.Services;
        public IDataContextService DataContext => Services.GetRequiredService<IDataContextService>();

        protected Action<IHostBuilder> ConfigureHost { get; }

        public Mock<IFileDialogService> FileDialogServiceMock { get; } = new Mock<IFileDialogService>();

        public HostedTestBase(ITestOutputHelper output, Action<IHostBuilder> configureHost = null) : base(output)
        {
            ConfigureHost = configureHost;
        }

        protected IHost InitHost(Action<IHostBuilder> configureHost = null)
        {
            var hostBuilder = new HostBuilder();

            configureHost?.Invoke(hostBuilder);

            var host = hostBuilder.ConfigureServices(x =>
                {
                    x.AddNatCruiseCoreDataservices();
                    x.AddSingleton<IDataContextService, DataContextService>();
                    x.AddTransient<IDeviceInfoService, TestDeviceInfoService>();
                    x.AddSingleton<ISetupInfoDataservice, SetupInfoDataservice>();
                    x.AddTransient<IFileDialogService>(x => FileDialogServiceMock.Object);
                })
                .ConfigureLogging(x =>
                {
                    x.AddDebug();
                })
                .Build();

            Task.Run(() => host.Run());
            return host;
        }
    }
}