using FScruiser.XF.Services;
using NatCruise;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SaleSelectViewModel : ViewModelBase
    {
        private IEnumerable<Sale> _sales;

        public IEnumerable<Sale> Sales
        {
            get => _sales;
            set => SetProperty(ref _sales, value);
        }

        public ISaleDataservice SaleDataservice { get; }

        public ICruiseNavigationService NavigationService { get; }

        public ICommand ShowCruiseSelectCommand => new Command<Sale>((sale) => ShowCruiseSelect(sale.SaleNumber));

        public ICommand ShowImportCommand => new Command(() => NavigationService.ShowImport());

        public SaleSelectViewModel(ICruiseNavigationService navigationService, ISaleDataservice saleDataservice)
        {
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public void ShowCruiseSelect(string saleNumber)
        {
            NavigationService.ShowCruiseSelect(saleNumber);
        }

        public override void Load()
        {
            var sales = SaleDataservice.GetSales();
            Sales = sales;
        }
    }
}