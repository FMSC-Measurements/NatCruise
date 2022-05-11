using FScruiser.XF.Services;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.Collections.Generic;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitListViewModel : XamarinViewModelBase
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

        protected override void Load(IParameters parameters)
        {
            Units = CuttingUnitDataservice.GetCuttingUnits();
        }
    }
}