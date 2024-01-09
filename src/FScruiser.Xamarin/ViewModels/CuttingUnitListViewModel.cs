using FScruiser.XF.Services;
using NatCruise;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.Collections.Generic;

namespace FScruiser.XF.ViewModels
{
    // TODO delete unused view/viewmodel
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _units;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        public IDataserviceProvider DatastoreProvider { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ICruiseNavigationService NavigationService { get; }

        public CuttingUnitListViewModel(ICruiseNavigationService navigationService, ICuttingUnitDataservice cuttingUnitDataservice)
        {
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public void SelectUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            NavigationService.ShowCuttingUnitInfo(unit.CuttingUnitCode);
        }

        protected override void Load(IDictionary<string, object> parameters)
        {
            Units = CuttingUnitDataservice.GetCuttingUnits();
        }
    }
}