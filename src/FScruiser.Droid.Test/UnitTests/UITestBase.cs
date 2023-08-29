using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xunit.Abstractions;
using System.Runtime.CompilerServices;

namespace FScruiser.Droid.Test.UnitTests
{
    [CollectionDefinition(nameof(UITestCollection))]
    public class UITestCollection : ICollectionFixture<AppFixture>
    { /* empty */ }

    public class UITestBase
    {
        public AppFixture AppFixture { get; }
        public string SaleName { get; }
        public string CruiseName { get; }
        public string UnitCode { get; }

        public IApp App => AppFixture.App;

        public ITestOutputHelper TestOutput { get; }

        public UITestBase(ITestOutputHelper testOutput, AppFixture appFixture)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
            AppFixture = appFixture ?? throw new ArgumentNullException(nameof(appFixture));

            SaleName = "12345 NewMultiTest_V2.0";
            CruiseName = "NewMultiTest_V2.0 12345 Timber Sale";
            UnitCode = "01";
        }

        public void ScreenShot([CallerMemberName] string title = null)
        {
            App.ScreenshotEx(TestOutput, title);
        }

    }
}
