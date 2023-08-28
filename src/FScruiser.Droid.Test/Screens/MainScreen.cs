using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test.Screens
{
    public class MainScreen : ScreenBase
    {
        public readonly Query openNavMenuButton = x => x.Marked("Open Navigation Menu");
        public readonly Query navMenu = x => x.Marked("Navigation Panel");

        // nav menu items
        public readonly Query selectSaleButton = x => x.Marked("Select Cruise");
        public readonly Query selectCuttingUnitButton = x => x.Marked("Select Cutting Unit");
        public readonly Query treesNavButton = x => x.Marked("Trees Navigation");
        public readonly Query tallyNavButton = x => x.Marked("Tally Navigation");
        public readonly Query unitNavButton = x => x.Marked("Cutting Unit Navigation");
        public readonly Query saleNavButton = x => x.Marked("Sale Navigation");
        public readonly Query strataNavButton = x => x.Marked("Strata Navigation");
        public readonly Query auditRulesNavButton = x => x.Marked("Audit Rules Navigation");
        public readonly Query cruisersNavButton = x => x.Marked("Cruisers Navigation");
        public readonly Query utilsNavButton = x => x.Marked("Utilities Navigation");

        public readonly Query settingNavButton = x => x.Marked("Settings Page Navigation");
        public readonly Query aboutNavButton = x => x.Marked("About Page Navigation");


        public readonly Query blankPage = x => x.Marked("Blank Page. A Blank Page");

        public readonly Query selectUnitDialogPanel = x => x.Class("AlertDialogLayout")
        .Descendant("DialogTitle").Marked("Select Cutting Unit")
        .Parent("AlertDialogLayout");

        public MainScreen(IApp app) : base(app)
        {
            page = x => x.Marked("Main Page");
        }

        public void OpenSelectSale()
        {
            App.Tap(selectSaleButton);
        }

        public IEnumerable<string> GetCuttingUnits()
        {
            App.Tap(selectCuttingUnitButton);
            App.WaitForElement(selectUnitDialogPanel);

            var unitButtons = App.Query(x => selectUnitDialogPanel(x).Descendant("AppCompatTextView"));
            return unitButtons.Select(x => x.Text).ToArray();
        }

        public void SelectCuttingUnit(string unit)
        {
            App.Tap(selectCuttingUnitButton);
            App.WaitForElement(selectUnitDialogPanel);

            var unitButtons = App.Query(x => selectUnitDialogPanel(x).Descendant("AppCompatTextView"));
            var unitButton = unitButtons.Where(x => x.Text.StartsWith(unit + ":")).Single();

            App.TapCoordinates(unitButton.Rect.CenterX, unitButton.Rect.CenterY);
            
        }
    }
}
