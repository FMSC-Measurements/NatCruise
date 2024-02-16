using Microsoft.Maui.UnitTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Backpack.Maui.UnitTests
{
    public class TestBase
    {
        protected ITestOutputHelper Output { get; }

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Output.WriteLine($"CodeBase: {System.Reflection.Assembly.GetExecutingAssembly().CodeBase}");

            DispatcherProvider.SetCurrent(new DispatcherProviderStub());
            //DeviceDisplay.SetCurrent(null);
            //DeviceInfo.SetCurrent(null);
            //AppInfo.SetCurrent(null);
        }
    }
}
