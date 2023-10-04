using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Services;
using System;
using System.Collections.Generic;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitDetailViewModel : ValidationViewModelBase
    {
        private CuttingUnit _cuttingUnit;
        private IEnumerable<LoggingMethod> _loggingMethodOptions;

        public CuttingUnitDetailViewModel(IDataserviceProvider dataserviceProvider, ILoggingService loggingService, ISetupInfoDataservice setupInfoDataservice, CuttingUnitValidator validator)
            : base(validator)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            LoggingService = loggingService;
            var unitDataservice = dataserviceProvider.GetDataservice<ICuttingUnitDataservice>();
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
            SetupDataservice = setupInfoDataservice ?? throw new ArgumentNullException(nameof(setupInfoDataservice));
        }

        protected ILoggingService LoggingService { get; }
        protected ICuttingUnitDataservice UnitDataservice { get; }
        protected ISetupInfoDataservice SetupDataservice { get; }

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            set
            {
                SetProperty(ref _cuttingUnit, value);
                ValidateAll(value);

                OnPropertyChanged(nameof(CuttingUnitCode));
                OnPropertyChanged(nameof(Area));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(LoggingMethod));
                OnPropertyChanged(nameof(PaymentUnit));
                OnPropertyChanged(nameof(Remarks));
                OnPropertyChanged(nameof(Rx));
            }
        }

        //private void OnCuttingUnitChanged(CuttingUnit value)
        //{
        //    if (value == null) { return; }
        //    value.PropertyChanged += CuttingUnit_PropertyChanged;
        //}

        //private void OnCuttingUnitChanging(CuttingUnit oldCuttingUnit)
        //{
        //    if (oldCuttingUnit == null) { return; }
        //    oldCuttingUnit.PropertyChanged -= CuttingUnit_PropertyChanged;
        //}

        public string CuttingUnitCode
        {
            get => CuttingUnit?.CuttingUnitCode;
            set
            {
                var origValue = CuttingUnit?.CuttingUnitCode;

                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.CuttingUnitCode = x);

                try
                {
                    UnitDataservice.UpdateCuttingUnitCode(CuttingUnit);
                }
                catch (FMSC.ORM.UniqueConstraintException)
                {
                    CuttingUnit.CuttingUnitCode = origValue;
                    OnPropertyChanged(nameof(CuttingUnitCode));
                    //DialogService.ShowNotification("Unit Code Already Exists");
                }
            }
        }

        public double Area
        {
            get => CuttingUnit?.Area ?? default(double);
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Area = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public string Description
        {
            get => CuttingUnit?.Description;
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Description = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public string LoggingMethod
        {
            get => CuttingUnit?.LoggingMethod;
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.LoggingMethod = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public string PaymentUnit
        {
            get => CuttingUnit?.PaymentUnit;
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.PaymentUnit = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public string Remarks
        {
            get => CuttingUnit?.Remarks;
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Remarks = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public string Rx
        {
            get => CuttingUnit?.Rx;
            set
            {
                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Rx = x);
                UnitDataservice.UpdateCuttingUnit(CuttingUnit);
            }
        }

        public IEnumerable<LoggingMethod> LoggingMethodOptions
        {
            get => _loggingMethodOptions;
            set => SetProperty(ref _loggingMethodOptions, value);
        }

        public override void Load()
        {
            base.Load();

            LoggingMethodOptions = SetupDataservice.GetLoggingMethods();
        }

        //private void CuttingUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var cuttingUnit = CuttingUnit;
        //    if (object.ReferenceEquals(cuttingUnit, sender) == false)
        //    {
        //        LoggingService?.LogEvent("cuttingUnit property changed target doesn't match VM cuttingUnit");
        //        return;
        //    }

        //    UnitDataservice.UpdateCuttingUnit(cuttingUnit);
        //}
    }
}