using FScruiser.XF.Constants;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.ComponentModel;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitInfoViewModel : XamarinViewModelBase
    {
        private CuttingUnit _cuttingUnit;

        public ICuttingUnitDataservice CuttingUnitDataservice { get; }

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            set
            {
                if (_cuttingUnit != null)
                { _cuttingUnit.PropertyChanged -= cuttingUnit_PropertyChanged; }
                SetProperty(ref _cuttingUnit, value);
                value.PropertyChanged += cuttingUnit_PropertyChanged;
            }
        }

        private void cuttingUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cuttingUnit = CuttingUnit;
            if (cuttingUnit == null) { return; }
            CuttingUnitDataservice.UpdateCuttingUnit(cuttingUnit);
        }

        public CuttingUnitInfoViewModel(ICuttingUnitDataservice cuttingUnitDataservice)
        {
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var unit = CuttingUnitDataservice.GetCuttingUnit(unitCode);
            CuttingUnit = unit;
        }
    }
}