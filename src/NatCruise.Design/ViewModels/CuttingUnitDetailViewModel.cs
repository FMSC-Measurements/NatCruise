using NatCruise.Data;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using NatCruise.Services;
using NatCruise.Design.Validation;
using System.Collections.Generic;
using NatCruise.Models;

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

                RaisePropertyChanged(nameof(CuttingUnitCode));
                RaisePropertyChanged(nameof(Area));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(LoggingMethod));
                RaisePropertyChanged(nameof(PaymentUnit));
                RaisePropertyChanged(nameof(Remarks));
                RaisePropertyChanged(nameof(Rx));
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

                SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.CuttingUnitCode = x, cu => UpdateCuttingUnitCode(cu));

                void UpdateCuttingUnitCode(CuttingUnit cu)
                {
                    try
                    {
                        UnitDataservice.UpdateCuttingUnitCode(cu);
                    }
                    catch (FMSC.ORM.UniqueConstraintException)
                    {
                        CuttingUnit.CuttingUnitCode = origValue;
                        RaisePropertyChanged(nameof(CuttingUnitCode));
                        //DialogService.ShowNotification("Unit Code Already Exists");
                    }
                }
            }
        }

        public double Area
        {
            get => CuttingUnit?.Area ?? default(double);
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Area = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
        }

        public string Description
        {
            get => CuttingUnit?.Description;
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Description = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
        }

        public string LoggingMethod
        {
            get => CuttingUnit?.LoggingMethod;
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.LoggingMethod = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
        }

        public string PaymentUnit
        {
            get => CuttingUnit?.PaymentUnit;
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.PaymentUnit = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
        }

        public string Remarks
        {
            get => CuttingUnit?.Remarks;
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Remarks = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
        }

        public string Rx
        {
            get => CuttingUnit?.Rx;
            set => SetPropertyAndValidate(CuttingUnit, value, (m, x) => m.Rx = x, cu => UnitDataservice.UpdateCuttingUnit(cu));
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