using FScruiser.XF.Constants;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitInfoViewModel : ViewModelBase
    {
        private CuttingUnit_Ex _cuttingUnit;

        public ICuttingUnitDatastore CuttingUnitDataservice { get; }

        public CuttingUnit_Ex CuttingUnit
        {
            get => _cuttingUnit;
            set
            {
                if(_cuttingUnit != null)
                { _cuttingUnit.PropertyChanged -= cuttingUnit_PropertyChanged; }
                SetProperty(ref _cuttingUnit, value);
                value.PropertyChanged += cuttingUnit_PropertyChanged;
            }
        }

        private void cuttingUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cuttingUnit = CuttingUnit;
            if(cuttingUnit == null) { return; }
            CuttingUnitDataservice.UpdateCuttingUnit(cuttingUnit);
        }

        public CuttingUnitInfoViewModel(IDataserviceProvider dataserviceProvider)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            CuttingUnitDataservice = dataserviceProvider.GetDataservice<ICuttingUnitDatastore>();
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var unit = CuttingUnitDataservice.GetUnit(unitCode);
            CuttingUnit = unit;
        }
    }
}
