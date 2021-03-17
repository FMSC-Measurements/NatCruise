using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Data;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using NatCruise.Services;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitDetailViewModel : BindableBase
    {
        private CuttingUnit _cuttingUnit;

        public CuttingUnitDetailViewModel(IDataserviceProvider dataserviceProvider, ILoggingService loggingService)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            LoggingService = loggingService;
            var unitDataservice = dataserviceProvider.GetDataservice<ICuttingUnitDataservice>();
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
        }

        protected ILoggingService LoggingService { get; }
        protected ICuttingUnitDataservice UnitDataservice { get; }

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