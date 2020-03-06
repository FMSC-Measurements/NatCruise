using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _units;

        public bool IsFileNotOpen => Units == null;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetValue(ref _units, value); }
        }

        public IDataserviceProvider DatastoreProvider { get; }

        public CuttingUnitListViewModel(IDataserviceProvider datastoreProvider, INavigationService navigationService) : base(navigationService)
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

            MessagingCenter.Send<string>(unit.Code, Messages.CUTTING_UNIT_SELECTED);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var datastore = DatastoreProvider.Get<ICuttingUnitDatastore>();
            if (datastore != null)
            {
                Units = datastore.GetUnits();
                base.RaisePropertyChanged(nameof(IsFileNotOpen));
            }
        }
    }
}