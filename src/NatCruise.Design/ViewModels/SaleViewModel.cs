using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Design.ViewModels
{
    public class SaleViewModel : ValidationViewModelBase
    {
        private Sale _sale;
        private IEnumerable<Forest> _forestOptions;
        private bool _isLoadingForestOptions = false;

        public SaleViewModel(ISaleDataservice saleDataservice, ISetupInfoDataservice setupInfo, SaleValidator saleValidator)
            : base(saleValidator)
        {
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupinfoDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
        }

        protected ISaleDataservice SaleDataservice { get; }
        public ISetupInfoDataservice SetupinfoDataservice { get; }

        public Sale Sale
        {
            get => _sale;
            set
            {
                _sale = value;

                ValidateAll(value);
                RaisePropertyChanged(nameof(Sale));
                RaisePropertyChanged(nameof(SaleNumber));
                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(Region));
                RaisePropertyChanged(nameof(Forest));
                RaisePropertyChanged(nameof(District));
                RaisePropertyChanged(nameof(CalendarYear));
                RaisePropertyChanged(nameof(Remarks));
            }
        }

        public string SaleNumber
        {
            get => Sale?.SaleNumber;
            set
            {
                SetPropertyAndValidate(Sale, value, (s, x) => s.SaleNumber = x);
                SaleDataservice.UpdateSaleNumber(Sale);
            }
        }

        public string Name
        {
            get => Sale?.Name;
            set
            {
                SetPropertyAndValidate(Sale, value, (s, x) => s.Name = x);
                SaleDataservice.UpdateSale(Sale);
            }
        }

        public string Region
        {
            get => Sale?.Region;
            set
            {
                var sale = Sale;
                var curValue = sale?.Region;
                if (curValue != value)
                {
                    SetPropertyAndValidate(sale, value, (s, x) => s.Region = x);
                    SaleDataservice.UpdateSale(Sale);
                    UpdateForestOptions(sale);
                    Forest = null;
                }
            }
        }

        public string Forest
        {
            get => Sale?.Forest;
            set
            {
                if (_isLoadingForestOptions) { return; }
                SetPropertyAndValidate(Sale, value, (s, x) => s.Forest = x);
                SaleDataservice.UpdateSale(Sale);
            }
        }

        public string District
        {
            get => Sale?.District;
            set
            {
                SetPropertyAndValidate(Sale, value, (s, x) => s.District = x);
                SaleDataservice.UpdateSale(Sale);
            }
        }

        public int CalendarYear
        {
            get => Sale?.CalendarYear ?? default(int);
            set
            {
                SetPropertyAndValidate(Sale, value, (s, x) => s.CalendarYear = x);
                SaleDataservice.UpdateSale(Sale);
            }
        }

        public string Remarks
        {
            get => Sale?.Remarks;
            set
            {
                SetPropertyAndValidate(Sale, value, (s, x) => s.Remarks = x);
                SaleDataservice.UpdateSale(Sale);
            }
        }

        //public string DefaultUOM
        //{
        //    get => Sale?.DefaultUOM;
        //    set => SetPropertyAndValidate(Sale, value, (s, x) => s.DefaultUOM = x, s => SaleDataservice.UpdateSale(s));
        //}

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions
        {
            get => _forestOptions;
            set => SetProperty(ref _forestOptions, value);
        }

        protected void UpdateForestOptions(Sale sale)
        {
            _isLoadingForestOptions = true;
            try
            {
                var region = sale?.Region;
                ForestOptions = SetupinfoDataservice.GetForests(region ?? "");
            }
            finally
            {
                _isLoadingForestOptions = false;
            }
        }

        public override void Load()
        {
            base.Load();

            var sale = SaleDataservice.GetSale();
            UpdateForestOptions(sale);
            Sale = sale;
        }
    }
}