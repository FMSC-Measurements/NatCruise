using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Services;
using System.Windows.Input;
using NatCruise.Async;
using FScruiser.Maui.Util;
using FScruiser.Maui.Controls;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class UtilitiesView : BasePage
{
    private ICommand? _showLimitingDistanceCommand;

    protected UtilitiesView()
    {
        InitializeComponent();
    }

    public UtilitiesView(ICruiseNavigationService navigationService) : this()
    {
        InitializeComponent();

        NavigationService = navigationService;
    }

    public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ??= new RelayCommand(() => NavigationService.ShowLimitingDistance().FireAndForget());
    public ICruiseNavigationService NavigationService { get; }
}