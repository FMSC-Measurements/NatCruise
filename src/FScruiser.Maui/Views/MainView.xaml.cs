using FScruiser.Maui.Services;
using FScruiser.Maui.ViewModels;
using NatCruise.Services;

namespace FScruiser.Maui.Views;

public partial class MainView : FlyoutPage
{
	//readonly NavigationPage _navigationPage;
	readonly MainViewModel _viewModel;

	public INavigation Navigation => _navigationPage.Navigation;
	public IApplicationSettingService AppSetting { get; }

	protected MainView()
	{
		InitializeComponent();
	}

	public MainView(MainViewModel viewModel, IApplicationSettingService appSettings)
	{
        

		//Detail = _navigationPage = new NavigationPage();
        InitializeComponent();

		Detail = new NavigationPage(new BlankView());
		AppSetting = appSettings;
        BindingContext = _viewModel = viewModel;
		//Flyout.BindingContext = _viewModel;
		//Detail.BindingContext = _viewModel;
		
		
	}

    private void _toggleDarkModeButton_Clicked(object sender, EventArgs e)
    {
		AppSetting.ToggleDarkMode();
    }
}