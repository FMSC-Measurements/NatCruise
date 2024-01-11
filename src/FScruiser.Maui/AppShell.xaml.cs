using FScruiser.Maui.ViewModels;
using FScruiser.Maui.Views;

namespace FScruiser.Maui;

public partial class AppShell : Shell
{
    protected AppShell()
    {
        InitializeComponent();

        // setting automation id on hamburger menu button
        // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/accessibility/automation-properties#flyoutpage
        // see btnMDPAutomationID_open and btnMDPAutomationID_close values
        // in src\FScruiser.Droid\Resources\values\strings.xml
        //IconImageSource.AutomationId = "btnMDPAutomationID";
        //FlyoutIcon.AutomationId = "btnMDPAutomationID";

        Routing.RegisterRoute("Import", typeof(ImportView));
        Routing.RegisterRoute("SaleSelect", typeof(SaleSelectView));
        Routing.RegisterRoute("CruiseSelect", typeof(CruiseSelectView));
        Routing.RegisterRoute("DatabaseUtilities", typeof(DatabaseUtilitiesView));
        Routing.RegisterRoute("LimitingDistance", typeof(LimitingDistanceView));
    }

    public AppShell(ShellViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }

    //protected override void OnNavigated(ShellNavigatedEventArgs args)
    //{
    //    base.OnNavigated(args);
    //}

    //protected override void OnNavigating(ShellNavigatingEventArgs args)
    //{
    //    base.OnNavigating(args);
    //}

    private void _cuttingUnitPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //var vm = BindingContext as MainViewModel;
        //var selectedIndex = _cuttingUnitPicker.SelectedIndex;
        //var selectedItem = _cuttingUnitPicker.ItemsSource[selectedIndex] as CuttingUnit_Ex;
        //vm.SelectedCuttingUnit = selectedItem;
    }

    private void OnNavButtonClicked(object sender, EventArgs e)
    {
        // hide flyout when nav button clicked
        FlyoutIsPresented = false;

        //try
        //{
        //    SetValue(Shell.NavBarIsVisibleProperty, false);
        //}
        //catch { /*do nothing*/ }
    }
}