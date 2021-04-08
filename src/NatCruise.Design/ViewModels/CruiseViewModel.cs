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
    public class CruiseViewModel : ViewModelBase
    {
        private Cruise _cruise;
        private IEnumerable<Purpose> _purposeOptions;

        public CruiseViewModel(ISaleDataservice saleDataservice, ISetupInfoDataservice setupInfo)
        {
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
        }

        private ISaleDataservice SaleDataservice { get; }

        protected ISetupInfoDataservice SetupDataservice { get; }

        public Cruise Cruise
        {
            get => _cruise;
            set
            {
                OnCruiseChanging(_cruise, value);
                SetProperty(ref _cruise, value);
                OnCruiseChanged(value);
            }
        }

        private void OnCruiseChanged(Cruise newValue)
        {
            if (newValue != null)
            {
                newValue.PropertyChanged += Cruise_PropertyChanged;
            }
        }

        private void OnCruiseChanging(Cruise oldValue, Cruise newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= Cruise_PropertyChanged;
            }
        }

        private void Cruise_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cruise = sender as Cruise;
            SaleDataservice.UpdateCruise(cruise);
        }

        public IEnumerable<Purpose> PurposeOptions
        {
            get => _purposeOptions;
            set => SetProperty(ref _purposeOptions, value);
        }

        public override void Load()
        {
            base.Load();

            Cruise = SaleDataservice.GetCruise();
            PurposeOptions = SetupDataservice.GetPurposes();
        }
    }
}