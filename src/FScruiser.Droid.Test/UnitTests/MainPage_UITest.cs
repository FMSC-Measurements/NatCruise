using FScruiser.Droid.Test.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FScruiser.Droid.Test.UnitTests
{
    [Collection(nameof(UITestCollection))]
    public class MainPage_UITest : UITestBase
    {
        public MainPage_UITest(ITestOutputHelper testOutput, AppFixture appFixture) : base(testOutput, appFixture)
        {
        }



        [Fact]
        public void ScreenShot_MainPage()
        {
            var mainScreen = new MainScreen(App);

            mainScreen.VerifyScreenDisplayed();
            ScreenShot("mainPage");
        }
    }
}
