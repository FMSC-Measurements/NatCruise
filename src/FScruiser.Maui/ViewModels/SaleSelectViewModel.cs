using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Services;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class SaleSelectViewModel : ViewModelBase
{
    private IEnumerable<Sale>? _sales;

    public IEnumerable<Sale>? Sales
    {
        get => _sales;
        set => SetProperty(ref _sales, value);
    }

    public ISaleDataservice SaleDataservice { get; }

    public ICruiseNavigationService NavigationService { get; }

    public ICommand ShowCruiseSelectCommand => new RelayCommand<Sale>((sale) => ShowCruiseSelect(sale?.SaleNumber));

    public ICommand ShowImportCommand => new RelayCommand(() => NavigationService.ShowImport());

    public SaleSelectViewModel(ICruiseNavigationService navigationService, ISaleDataservice saleDataservice)
    {
        SaleDataservice = saleDataservice;
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    public void ShowCruiseSelect(string? saleNumber)
    {
        NavigationService.ShowCruiseSelect(saleNumber);
    }

    protected override void OnInitialize(IDictionary<string, object> parameters)
    {
        var sales = SaleDataservice.GetSales();
        Sales = sales;
    }
}