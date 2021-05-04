using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class SaleViewModel : ViewModelBase
    {
        private Sale _sale;

        public SaleViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupInfo)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            SaleDataservice = dataserviceProvider.GetDataservice<ISaleDataservice>();
            SetupinfoDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
        }

        protected ISaleDataservice SaleDataservice { get; }
        public ISetupInfoDataservice SetupinfoDataservice { get; }

        public Sale Sale
        {
            get => _sale;
            set
            {
                if(_sale != null) { _sale.PropertyChanged -= Sale_PropertyChanged; }
                _sale = value;
                // update forest options after seting _sale but before raising sale property changed
                // otherwise it causes binding issues
                RaisePropertyChanged(nameof(ForestOptions)); 
                RaisePropertyChanged();
                if(value != null) { _sale.PropertyChanged += Sale_PropertyChanged; }
            }
        }

        private void Sale_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var sale = sender as Sale;

            if (ValidateSale(sale))
            {
                SaleDataservice.UpdateSale(sale);
            }
            if(e.PropertyName == nameof(Sale.Region))
            {
                RaisePropertyChanged(nameof(ForestOptions));
            }
        }

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions => SetupinfoDataservice.GetForests(Sale?.Region ?? "");

        public override void Load()
        {
            base.Load();

            Sale = SaleDataservice.GetSale();
        }

        public bool ValidateSale(Sale sale)
        {
            return string.IsNullOrWhiteSpace(sale.Name) == false
                && string.IsNullOrWhiteSpace(sale.SaleNumber) == false;
        }
    }
}