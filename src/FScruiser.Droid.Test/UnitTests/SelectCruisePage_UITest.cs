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
    public class SelectCruisePage_UITest : UITestBase
    {
        public SelectCruisePage_UITest(ITestOutputHelper testOutput, AppFixture appFixture) : base(testOutput, appFixture)
        {
        }

        [Fact]
        public void SelectCruise_ScreenShot()
        {
            var mainscreen = new MainScreen(App);
            mainscreen.OpenSelectSale();

            var saleListScreen = new SaleListScreen(App);
            saleListScreen.VerifyScreenDisplayed();
            var sales = saleListScreen.GetSales();
            saleListScreen.SelectSale(sales.First());

            var cruiseListScreen = new CruiseListScreen(App);
            cruiseListScreen.VerifyScreenDisplayed();
            ScreenShot("slectCruise");

            var cruises = cruiseListScreen.GetCruises();
            cruiseListScreen.OpenCruise(cruises.First());

            App.WaitForElement(mainscreen.blankPage);

            ScreenShot("onCruiseSelected");
            
        }
    }
}
