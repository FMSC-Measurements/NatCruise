using FScruiser.XF.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SaleSelectViewModel : XamarinViewModelBase
    {
        private IEnumerable<Sale> _sales;

        public IEnumerable<Sale> Sales
        {
            get => _sales;
            set => SetProperty(ref _sales, value);
        }

        public ISaleDataservice SaleDataservice { get; }

        public ICruiseNavigationService NavigationService { get; }

        public ICommand ShowCruiseSelectCommand => new Command<string>((saleID) => ShowCruiseSelect(saleID));

        public ICommand ShowImportCommand => new Command(() => NavigationService.ShowImport());

        public SaleSelectViewModel(ICruiseNavigationService navigationService, IDataserviceProvider dataServiceprovider)
        {
            if (dataServiceprovider is null) { throw new ArgumentNullException(nameof(dataServiceprovider)); }

            SaleDataservice = dataServiceprovider.GetDataservice<ISaleDataservice>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public void ShowCruiseSelect(string saleID)
        {
            NavigationService.ShowCruiseSelect(saleID);
        }

        protected override void Load(IParameters parameters)
        {
            var sales = SaleDataservice.GetSales();
            Sales = sales;
        }
    }
}