using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Services;
using Prism.Mvvm;
using System;
using System.ComponentModel;

namespace NatCruise.Wpf.ViewModels
{
    public class CuttingUnitDetailPageViewModel : BindableBase
    {
        private CuttingUnit _cuttingUnit;

        public CuttingUnitDetailPageViewModel(IDataserviceProvider dataserviceProvider, ILoggingService loggingService)
        {
            LoggingService = loggingService;
            var unitDataservice = dataserviceProvider.GetDataservice<ICuttingUnitDataservice>();
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
        }

        private ILoggingService LoggingService { get; }

        private ICuttingUnitDataservice UnitDataservice { get; }

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            set
            {
                OnCuttingUnitChanging(_cuttingUnit);
                SetProperty(ref _cuttingUnit, value);
                OnCuttingUnitChanged(value);
            }
        }

        private void OnCuttingUnitChanged(CuttingUnit value)
        {
            if (value == null) { return; }
            value.PropertyChanged += CuttingUnit_PropertyChanged;
        }

        private void OnCuttingUnitChanging(CuttingUnit oldCuttingUnit)
        {
            if (oldCuttingUnit == null) { return; }
            oldCuttingUnit.PropertyChanged -= CuttingUnit_PropertyChanged;
        }

        private void CuttingUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cuttingUnit = CuttingUnit;
            if (object.ReferenceEquals(cuttingUnit, sender) == false)
            {
                LoggingService?.LogEvent("cuttingUnit property changed target doesn't match VM cuttingUnit");
                return;
            }

            UnitDataservice.UpdateCuttingUnit(cuttingUnit);
        }
    }
}