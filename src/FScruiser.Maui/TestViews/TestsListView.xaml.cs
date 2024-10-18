using FScruiser.Maui.Services;
using NatCruise.Navigation;

namespace FScruiser.Maui.TestViews;

public partial class TestsListView : ContentPage
{
	public TestsListView()
	{
		InitializeComponent();
	}

    public TestsListView(ICruiseNavigationService navigationService, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        NavigationService = navigationService as MauiNavigationService;
        Services = serviceProvider;
    }

    private MauiNavigationService? NavigationService { get; }

    public IServiceProvider Services { get; }

    private void _testControlsButton_Clicked(object sender, EventArgs e)
    {
        NavigationService.ShowView<TestControlsViews>();
    }

    private void _testDialogServicesButton_Clicked(object sender, EventArgs e)
    {
        NavigationService.ShowView<TestDialogServiceView>();
    }
}