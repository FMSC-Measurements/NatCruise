using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
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
        public ICruiseNavigationService NavigationService { get; }

        public CuttingUnitListViewModel(IDataserviceProvider datastoreProvider, ICruiseNavigationService navigationService)
        {
            DatastoreProvider = datastoreProvider ?? throw new ArgumentNullException(nameof(datastoreProvider));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public void SelectUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            NavigationService.ShowCuttingUnitInfo(unit.CuttingUnitCode);
        }

        protected override void Load(IParameters parameters)
        {
            var datastore = DatastoreProvider.GetDataservice<ICuttingUnitDataservice>();
            if (datastore != null)
            {
                Units = datastore.GetUnits();
            }
        }
    }
}