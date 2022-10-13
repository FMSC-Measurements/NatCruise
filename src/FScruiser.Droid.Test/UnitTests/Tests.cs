using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test
{
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests()
        {
            platform = Platform.Android;
            app = AppInitializer.StartApp(platform);
        }


        [Fact]
        public void AppLaunches()
        {

            app.WaitForElement(x => x.Id("alertTitle").Text("Send Diagnostic Data"));
            var askDiagnosticDialog = app.Screenshot("askDianosticDialog");
            app.Tap(x => x.Button("Enable"));

            app.WaitForElement(c => c.Marked("Select Cruise"));

            app.Screenshot("onLoad");

            app.Repl();
        }

        [Fact]
        public void SelectCruise()
        {
            var saleName = "21123 Bishop";
            var cruiseName = "Bishop 21123 Timber Sale";

            app.WaitForElement(x => x.Id("alertTitle").Text("Send Diagnostic Data"));
            app.Tap(x => x.Button("Enable"));

            app.WaitForElement(c => c.Marked("Select Cruise"));
            app.Tap(x => x.Text("Select Cruise"));

            app.Screenshot("selectSale");
            app.WaitForElement(x => x.Marked("Sale List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Sale List").Descendant("TextView").Text(saleName));

            app.Screenshot("selectCruise");
            app.WaitForElement(x => x.Marked("Cruise List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Cruise List").Descendant("TextView").Text(cruiseName));

            app.Tap(x => x.Button("Open"));

            app.WaitForElement(x => x.Marked("Select Cruise"));
            app.Query(x => x.Marked("Select Cruise").Descendant("LabelRenderer")).Should().Contain(x => x.Text == cruiseName);
            app.Query(x => x.Marked("Select Cruise").Descendant("PickerEditText").Text("Select CuttingUnit")).Should().NotBeEmpty();

            app.Screenshot("onCruiseSelected");
        }


        [Fact]
        public void SelectCuttingUnit()
        {
            var saleName = "21123 Bishop";
            var cruiseName = "Bishop 21123 Timber Sale";

            var cuttingUnitTitle = "01: Mixed Conifer ITM";

            app.WaitForElement(x => x.Id("alertTitle").Text("Send Diagnostic Data"));
            app.Tap(x => x.Button("Enable"));

            app.WaitForElement(c => c.Marked("Select Cruise"));
            app.Tap(x => x.Text("Select Cruise"));

            app.WaitForElement(x => x.Marked("Sale List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Sale List").Descendant("TextView").Text(saleName));

            app.WaitForElement(x => x.Marked("Cruise List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Cruise List").Descendant("TextView").Text(cruiseName));

            app.Tap(x => x.Button("Open"));

            app.WaitForElement(x => x.Marked("Select Cruise"));
            app.Query(x => x.Marked("Select Cruise").Descendant("LabelRenderer")).Should().Contain(x => x.Text == cruiseName);
            app.Query(x => x.Marked("Select Cruise").Descendant("PickerEditText").Text("Select CuttingUnit")).Should().NotBeEmpty();

            app.Tap(x => x.Marked("Select Cruise").Descendant("PickerEditText").Text("Select CuttingUnit"));

            app.WaitForElement(x => x.Class("AlertDialogLayout").Descendant("DialogTitle").Text("Select Cutting Unit"));
            app.Tap(x => x.Class("AlertDialogLayout").Descendant().Text(cuttingUnitTitle));

        }

        [Fact]
        public void GoToTallyPage()
        {
            var saleName = "21123 Bishop";
            var cruiseName = "Bishop 21123 Timber Sale";

            var unitCode = "01";
            var unitDescription = "Mixed Conifer ITM";
            var cuttingUnitTitle = $"{unitCode}: {unitDescription}";
            var tallyPageTitle = $"Unit {unitCode} - {unitDescription}";

            //app.WaitForElement(x => x.Id("alertTitle").Text("Send Diagnostic Data"));
            //app.Tap(x => x.Button("Enable"));

            // when I select the cruise
            app.WaitForElement(c => c.Marked("Select Cruise"));
            app.Tap(x => x.Text("Select Cruise"));

            app.WaitForElement(x => x.Marked("Sale List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Sale List").Descendant("TextView").Text(saleName));

            app.WaitForElement(x => x.Marked("Cruise List").Child()).Should().HaveCount(1);
            app.Tap(x => x.Marked("Cruise List").Descendant("TextView").Text(cruiseName));

            app.Tap(x => x.Button("Open"));

            app.WaitForElement(x => x.Marked("Select Cruise"));
            // selected cruise should display cruise name
            app.Query(x => x.Marked("Select Cruise").Descendant("LabelRenderer")).Should().Contain(x => x.Text == cruiseName);

            // and select cutting unit
            app.Query(x => x.Marked("Select Cruise").Descendant("PickerEditText").Text("Select CuttingUnit")).Should().NotBeEmpty();
            app.Tap(x => x.Marked("Select Cruise").Descendant("PickerEditText").Text("Select CuttingUnit"));

            // and tap the cutting unit 
            app.WaitForElement(x => x.Class("AlertDialogLayout").Descendant("DialogTitle").Text("Select Cutting Unit"));
            app.Tap(x => x.Class("AlertDialogLayout").Descendant().Text(cuttingUnitTitle));

            // and open the tally page
            app.WaitForElement(x => x.Class("ButtonRenderer").Text("Tally"));
            app.Tap(x => x.Class("ButtonRenderer").Text("Tally"));

            // then the toolbar displays the current unit and unit description
            app.WaitForElement(x => x.Class("Toolbar").Child(1).Text(tallyPageTitle));

            // and take a screenshot
            app.Screenshot("treeTallyPage_empty");
        }
    }
}
