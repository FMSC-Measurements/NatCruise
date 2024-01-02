using FScruiser.Maui.ViewModels;

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
    }

    public AppShell(ShellViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }

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
        try
        {
            SetValue(Shell.NavBarIsVisibleProperty, false);
        }
        catch { /*do nothing*/ }
    }
}