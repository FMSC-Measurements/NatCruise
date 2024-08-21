using FScruiser.Maui.Services;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views;

public partial class MainView : FlyoutPage
{
	//readonly NavigationPage _navigationPage;
	readonly MainViewModel _viewModel;

	public INavigation Navigation => _navigationPage.Navigation;

	protected MainView()
	{
		InitializeComponent();
	}

	public MainView(MainViewModel viewModel)
	{
        

		//Detail = _navigationPage = new NavigationPage();
        InitializeComponent();

		Detail = new NavigationPage(new BlankView());

        BindingContext = _viewModel = viewModel;
		//Flyout.BindingContext = _viewModel;
		//Detail.BindingContext = _viewModel;
		
		
	}
}