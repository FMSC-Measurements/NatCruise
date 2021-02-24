﻿using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class SalePageViewModel : ViewModelBase
    {
        private Sale _sale;

        public SalePageViewModel(IDataserviceProvider dataserviceProvider)
        {
            CruiseDataservice = dataserviceProvider.GetDataservice<ICruiseDataservice>();
            SetupinfoDataservice = dataserviceProvider.GetDataservice<ISetupInfoDataservice>();
        }

        private ICruiseDataservice CruiseDataservice { get; }
        public ISetupInfoDataservice SetupinfoDataservice { get; }

        public Sale Sale
        {
            get => _sale;
            set
            {
                OnSaleChanging(_sale, value);
                SetProperty(ref _sale, value);
                OnSaleChanged(value);
            }
        }

        private void OnSaleChanged(Sale newValue)
        {
            if (newValue != null)
            {
                newValue.PropertyChanged += Sale_PropertyChanged;
            }
        }

        private void OnSaleChanging(Sale oldValue, Sale newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= Sale_PropertyChanged;
            }
        }

        private void Sale_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var sale = sender as Sale;

            CruiseDataservice.UpdateSale(sale);
        }

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions
        {
            get => SetupinfoDataservice.GetForests(Sale?.Region ?? "");
        }

        protected override void Load()
        {
            Sale = CruiseDataservice.GetSale();
            RaisePropertyChanged(nameof(ForestOptions));
        }
    }
}