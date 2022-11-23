using FScruiser.XF.Services;
using NatCruise;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _cuttingUnits;
        private CuttingUnit _selectedCuttingUnit;
        private CuttingUnitStrataSummary _selectedUnitStratumSummary;

        public ICommand ShowSelectSale => new Command(() => NavigationService.ShowSaleSelect().FireAndForget());

        public ICommand ShowSaleCommand => new Command(() => NavigationService.ShowSale(DatastoreProvider?.CruiseID).FireAndForget());

        public ICommand ShowUnitsCommand => new Command(() => NavigationService.ShowCuttingUnitList().FireAndForget());

        public ICommand ShowUnitInfoCommand => new Command(() => NavigationService.ShowCuttingUnitInfo(SelectedCuttingUnit?.CuttingUnitCode).FireAndForget());

        public ICommand ShowTreesCommand => new Command(() => ShowTrees().FireAndForget());

        public ICommand ShowPlotsCommand => new Command(() => ShowPlots().FireAndForget());

        public ICommand ShowPlotTreesCommand => new Command(() => NavigationService.ShowPlotTreeList(SelectedCuttingUnit?.CuttingUnitCode).FireAndForget());

        public ICommand ShowTallyCommand => new Command(() => ShowTally().FireAndForget());

        public ICommand ShowSettingsCommand => new Command(() => NavigationService.ShowSettings().FireAndForget());

        public ICommand ShowFeedbackCommand => new Command(() => NavigationService.ShowFeedback().FireAndForget());

        public ICommand ShowSampleStateManagmentCommand => new Command(() => NavigationService.ShowSampleStateManagment().FireAndForget());

        public ICommand ShowCruisersCommand => new Command(() => NavigationService.ShowManageCruisers().FireAndForget());

        public CuttingUnit SelectedCuttingUnit
        {
            get => _selectedCuttingUnit;
            set
            {
                SetProperty(ref _selectedCuttingUnit, value);
                if (value != null)
                {
                    var summary = CuttingUnitDataservice.GetCuttingUnitStrataSummary(value.CuttingUnitCode);
                    SelectedUnitStrataSummary = summary;
                }
                else { SelectedUnitStrataSummary = null; }

                RaisePropertyChanged(nameof(IsCuttingUnitSelected));
                NavigationService.ShowBlank();
            }
        }

        public CuttingUnitStrataSummary SelectedUnitStrataSummary
        {
            get => _selectedUnitStratumSummary;
            set
            {
                SetProperty(ref _selectedUnitStratumSummary, value);
                RaisePropertyChanged(nameof(HasPlotStrata));
                RaisePropertyChanged(nameof(HasTreeStrata));
            }
        }

        public IEnumerable<CuttingUnit> CuttingUnits
        {
            get => _cuttingUnits;
            protected set => SetProperty(ref _cuttingUnits, value);
        }

        public string CurrentCruiseName
        {
            get
            {
                var cruiseID = DatastoreProvider?.CruiseID;
                if (cruiseID != null)
                {
                    var cruise = SaleDataservice.GetCruise(cruiseID);
                    return cruise.ToString();
                }
                else
                {
                    return "Select Cruise";
                }
            }
        }

        public bool IsCruiseSelected => DatastoreProvider?.CruiseID != null;

        public bool IsCuttingUnitSelected
        {
            get { return IsCruiseSelected && SelectedCuttingUnit != null; }
        }

        public bool HasPlotStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasPlotStrata;

        public bool HasTreeStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasTreeStrata;

        public ICruiseNavigationService NavigationService { get; }
        public IDataserviceProvider DatastoreProvider { get; }
        public IAppInfoService AppInfo { get; }
        public INatCruiseDialogService DialogService { get; }
        public IDeviceInfoService DeviceInfo { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ISaleDataservice SaleDataservice { get; }

        public MainViewModel(
                ICruiseNavigationService navigationService,
                INatCruiseDialogService dialogService,
                IDataserviceProvider datastoreProvider,
                IDeviceInfoService deviceInfoService,
                IAppInfoService appInfo)
        {
            AppInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DialogService = dialogService;
            DatastoreProvider = datastoreProvider;
            DeviceInfo = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));

            if (datastoreProvider.CruiseID != null)
            {
                var cuttingUnitDataservice = CuttingUnitDataservice = datastoreProvider.GetDataservice<ICuttingUnitDataservice>();
                SaleDataservice = datastoreProvider.GetDataservice<ISaleDataservice>();
                if (cuttingUnitDataservice != null)
                {
                    CuttingUnits = cuttingUnitDataservice.GetCuttingUnits();
                }
            }
        }

        private Task ShowTrees()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowTreeList(selectedUnit.CuttingUnitCode);
        }

        private Task ShowPlots()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowPlotList(selectedUnit.CuttingUnitCode);
        }

        private Task ShowTally()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowTally(selectedUnit.CuttingUnitCode);
        }
    }
}