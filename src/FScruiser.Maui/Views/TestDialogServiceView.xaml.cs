using NatCruise.Navigation;

namespace FScruiser.Maui.Views;

public partial class TestDialogServiceView : ContentPage
{
	public TestDialogServiceView(INatCruiseDialogService dialogService)
	{
		InitializeComponent();

		DialogService = dialogService;
	}

    public INatCruiseDialogService DialogService { get; }

    private void _showNotificationButton_Clicked(object sender, EventArgs e)
    {
		DialogService.ShowNotification("Some Message", title: "Some Title");
    }

    private async void _showAskKpiButton_Clicked(object sender, EventArgs e)
    {
        var result = await DialogService.AskKPIAsync(30, 1);
        DialogService.ShowNotification(result?.ToString() ?? "Null", "Result");
    }

    private async void _showAskTreeCountButton_Clicked(object sender, EventArgs e)
    {
        var result = await DialogService.AskTreeCount(30);
        DialogService.ShowNotification(result?.TreeCount?.ToString() ?? "Null", "Result");
    }

    private void _darkModeButton_Clicked(object sender, EventArgs e)
    {
        App.Current.UserAppTheme = AppTheme.Dark;
    }

    private void _lightModeButton_Clicked(object sender, EventArgs e)
    {

        App.Current.UserAppTheme = AppTheme.Light;
    }
}