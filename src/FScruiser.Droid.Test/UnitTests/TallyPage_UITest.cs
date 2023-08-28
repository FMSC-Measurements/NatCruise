using FScruiser.Droid.Test.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xunit.Abstractions;

namespace FScruiser.Droid.Test.UnitTests
{
    public class TallyPageFixture
    {
        public TallyPageFixture() { }

        bool _isSetup;



        public void Setup(IApp app, string saleName, string cruiseName, string unitCode)
        {
            if (_isSetup) { return; }

            var mainscreen = new MainScreen(app);
            mainscreen.OpenSelectSale();

            var saleListScreen = new SaleListScreen(app);
            saleListScreen.VerifyScreenDisplayed();
            var sales = saleListScreen.GetSales();
            saleListScreen.SelectSale(saleName);

            var cruiseListScreen = new CruiseListScreen(app);
            cruiseListScreen.VerifyScreenDisplayed();

            var cruises = cruiseListScreen.GetCruises();
            cruiseListScreen.OpenCruise(cruiseName);

            app.WaitForElement(mainscreen.blankPage);

            mainscreen.SelectCuttingUnit(unitCode);

            app.Tap(mainscreen.tallyNavButton);
        }
    }

    [Collection(nameof(UITestCollection))]
    public class TallyPage_UITest : UITestBase, IClassFixture<TallyPageFixture>
    {
        public TallyPage_UITest(ITestOutputHelper testOutput, AppFixture appFixture, TallyPageFixture tallyPageFixture) : base(testOutput, appFixture)
        {
            tallyPageFixture.Setup(appFixture.App, SaleName, CruiseName, UnitCode);
        }

        [Fact]
        public void TallyPage_ScreenShot()
        {
            var tallyPage = new TallyScreen(App);
            tallyPage.VerifyScreenDisplayed();

            ScreenShot("treeTallyPage");
        }

        [Fact]
        public void TallyEach()
        {
            var tallyPage = new TallyScreen(App);
            tallyPage.VerifyScreenDisplayed();

            var startTallyEntryCount = tallyPage.GetTallyEntryCount();

            var tallyPops = tallyPage.GetTallyPopulationItems();

            foreach(var tp in tallyPops)
            {
                App.TapCenter(tp);

                if(App.Query(tallyPage.alertDialog).Any())
                {
                    App.Tap(x => tallyPage.alertDialog(x).Descendant().Text("OK"));
                }
            }

            var afterTallyEntryCount = tallyPage.GetTallyEntryCount();

            afterTallyEntryCount.Should().Be(startTallyEntryCount + tallyPops.Count());

        }
    }
}
