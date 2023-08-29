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
    public class SelectSalePage_UITest : UITestBase
    {
        public SelectSalePage_UITest(ITestOutputHelper testOutput, AppFixture appFixture) : base(testOutput, appFixture)
        {
        }

        [Fact]
        public void SelectSale_ScreenShot()
        {
            var mainscreen = new MainScreen(App);
            mainscreen.OpenSelectSale();

            var saleListScreen = new SaleListScreen(App);
            saleListScreen.VerifyScreenDisplayed();

            ScreenShot("selectSale");
        }
    }
}
