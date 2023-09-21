using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class CruiseViewModel : ValidationViewModelBase
    {
        private Cruise _cruise;
        private IEnumerable<Purpose> _purposeOptions;
        private IEnumerable<UOM> _uomOptions;

        public CruiseViewModel(ISaleDataservice saleDataservice, ISetupInfoDataservice setupInfo, CruiseValidator validator)
            : base(validator)
        {
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));

            PurposeOptions = SetupDataservice.GetPurposes();
            UOMOptions = SetupDataservice.GetUOMCodes();
        }

        private ISaleDataservice SaleDataservice { get; }

        protected ISetupInfoDataservice SetupDataservice { get; }

        public Cruise Cruise
        {
            get => _cruise;
            set
            {
                SetProperty(ref _cruise, value);
                ValidateAll(value);

                OnPropertyChanged(nameof(CruiseNumber));
                OnPropertyChanged(nameof(Purpose));
                OnPropertyChanged(nameof(Remarks));
                OnPropertyChanged(nameof(UseCrossStrataPlotTreeNumbering));
                OnPropertyChanged(nameof(DefaultUOM));
            }
        }

        public string CruiseNumber
        {
            get => Cruise?.CruiseNumber;
            set
            {
                SetPropertyAndValidate(Cruise, value, (c, x) => c.CruiseNumber = x);
                SaleDataservice.UpdateCruise(Cruise);
            }
        }

        public string Purpose
        {
            get => Cruise?.Purpose;
            set
            {
                SetPropertyAndValidate(Cruise, value, (c, x) => c.Purpose = x);
                SaleDataservice.UpdateCruise(Cruise);
            }
        }

        public string Remarks
        {
            get => Cruise?.Remarks;
            set
            {
                SetPropertyAndValidate(Cruise, value, (c, x) => c.Remarks = x);
                SaleDataservice.UpdateCruise(Cruise);
            }
        }

        public bool? UseCrossStrataPlotTreeNumbering
        {
            get => Cruise?.UseCrossStrataPlotTreeNumbering;
            set
            {
                SetPropertyAndValidate(Cruise, value, (c, x) => c.UseCrossStrataPlotTreeNumbering = x);
                SaleDataservice.UpdateCruise(Cruise);
            }
        }

        public string DefaultUOM
        {
            get => Cruise?.DefaultUOM;
            set
            {
                SetPropertyAndValidate(Cruise, value, (c, x) => c.DefaultUOM = x);
                SaleDataservice.UpdateCruise(Cruise);
            }
        }

        public IEnumerable<UOM> UOMOptions
        {
            get => _uomOptions;
            set => SetProperty(ref _uomOptions, value);
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
        }
    }
}