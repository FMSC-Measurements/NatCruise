using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Services;
using System.Windows.Input;
using NatCruise.Async;
using FScruiser.Maui.Util;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class UtilitiesView : ContentPage
{
    private ICommand _showLimitingDistanceCommand;

    public UtilitiesView()
    {
        InitializeComponent();

        NavigationService = this.FindMauiContext(true).Services.GetRequiredService<ICruiseNavigationService>();
    }

    protected UtilitiesView(ICruiseNavigationService navigationService)
    {
        InitializeComponent();

        NavigationService = navigationService;
    }

    public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ??= new RelayCommand(() => NavigationService.ShowLimitingDistance().FireAndForget());
    public ICruiseNavigationService NavigationService { get; }
}