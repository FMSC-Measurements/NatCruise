using NatCruise.Data.Abstractions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Design.Validation;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class CruiseViewModel : ValidationViewModelBase
    {
        private Cruise _cruise;
        private IEnumerable<Purpose> _purposeOptions;

        public CruiseViewModel(ISaleDataservice saleDataservice, ISetupInfoDataservice setupInfo, CruiseValidator validator)
            : base(validator)
        {
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));

            PurposeOptions = SetupDataservice.GetPurposes();
        }

        private ISaleDataservice SaleDataservice { get; }

        protected ISetupInfoDataservice SetupDataservice { get; }

        public Cruise Cruise
        {
            get => _cruise;
            set
            {
                //OnCruiseChanging(_cruise, value);
                SetProperty(ref _cruise, value);

                RaisePropertyChanged(nameof(CruiseNumber));
                RaisePropertyChanged(nameof(Purpose));
                RaisePropertyChanged(nameof(Remarks));
                RaisePropertyChanged(nameof(UseCrossStrataPlotTreeNumbering));
                RaisePropertyChanged(nameof(DefaultUOM));

                //OnCruiseChanged(value);
            }
        }

        public string CruiseNumber
        {
            get => Cruise?.CruiseNumber;
            set => SetPropertyAndValidate(Cruise, value, (c, x) => c.CruiseNumber = x, crz => SaleDataservice.UpdateCruise(crz));
        }

        public string Purpose
        {
            get => Cruise?.Purpose;
            set => SetPropertyAndValidate(Cruise, value, (c, x) => c.Purpose = x, crz => SaleDataservice.UpdateCruise(crz));
        }

        public string Remarks
        {
            get => Cruise?.Remarks;
            set => SetPropertyAndValidate(Cruise, value, (c, x) => c.Remarks = x, crz => SaleDataservice.UpdateCruise(crz));
        }

        public bool? UseCrossStrataPlotTreeNumbering
        {
            get => Cruise?.UseCrossStrataPlotTreeNumbering;
            set => SetPropertyAndValidate(Cruise, value, (c, x) => c.UseCrossStrataPlotTreeNumbering = x, crz => SaleDataservice.UpdateCruise(crz));
        }

        public string DefaultUOM
        {
            get => Cruise?.DefaultUOM;
            set => SetPropertyAndValidate(Cruise, value, (c, x) => c.DefaultUOM = x, crz => SaleDataservice.UpdateCruise(crz));
        }

        public IEnumerable<Purpose> PurposeOptions
        {
            get => _purposeOptions;
            set => SetProperty(ref _purposeOptions, value);
        }

        //private void OnCruiseChanged(Cruise newValue)
        //{
        //    if (newValue != null)
        //    {
        //        newValue.PropertyChanged += Cruise_PropertyChanged;
        //    }
        //}

        //private void OnCruiseChanging(Cruise oldValue, Cruise newValue)
        //{
        //    if (oldValue != null)
        //    {
        //        oldValue.PropertyChanged -= Cruise_PropertyChanged;
        //    }
        //}

        //private void Cruise_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var cruise = sender as Cruise;
        //    SaleDataservice.UpdateCruise(cruise);
        //}



        public override void Load()
        {
            base.Load();

            Cruise = SaleDataservice.GetCruise();
        }
    }
}