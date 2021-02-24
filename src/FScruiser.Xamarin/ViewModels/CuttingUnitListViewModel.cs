using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using FScruiser.XF.Services;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using NatCruise.Data;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _units;

        public bool IsFileNotOpen => Units == null;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        public IDataserviceProvider DatastoreProvider { get; }

        public CuttingUnitListViewModel(IDataserviceProvider datastoreProvider)
        {
            DatastoreProvider = datastoreProvider ?? throw new ArgumentNullException(nameof(datastoreProvider));

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED, (sender, path) =>
            {
                Refresh();
            });
        }

        public void SelectUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            MessagingCenter.Send<string>(unit.CuttingUnitCode, Messages.CUTTING_UNIT_SELECTED);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var datastore = DatastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            if (datastore != null)
            {
                Units = datastore.GetUnits();
                base.RaisePropertyChanged(nameof(IsFileNotOpen));
            }
        }
    }
}