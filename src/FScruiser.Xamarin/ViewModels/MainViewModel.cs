using FScruiser.XF.Services;
using NatCruise.Core.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    //public class NavigationListItem
    //{
    //    public string Title { get; set; }

    //    public string NavigationPath { get; set; }

    //    public bool CanShow
    //    {
    //        get
    //        {
    //            return CanShowAction?.Invoke() ?? true;
    //        }
    //    }

    //    public Func<NavigationParameters> GetParamaters { get; set; }

    //    public Func<bool> CanShowAction { get; set; }
    //}

    public class MainViewModel : XamarinViewModelBase
    {
        private IEnumerable<CuttingUnit_Ex> _cuttingUnits;
        private CuttingUnit_Ex _selectedCuttingUnit;

        public ICruiseNavigationService NavigationService { get; }

        public ICommand ShowSelectSale => new Command(async () => await NavigationService.ShowSaleSelect());

        public ICommand ShowSaleCommand => new Command(() => NavigationService.ShowSale(DatastoreProvider?.CruiseID));

        public ICommand ShowUnitsCommand => new Command(() => NavigationService.ShowCuttingUnitList());

        public ICommand ShowUnitInfoCommand => new Command(() => NavigationService.ShowCuttingUnitInfo(SelectedCuttingUnit?.CuttingUnitCode));

        public ICommand ShowTreesCommand => new Command(() => ShowTrees());

        public ICommand ShowPlotsCommand => new Command(() => ShowPlots());

        public ICommand ShowPlotTreesCommand => new Command(() => NavigationService.ShowPlotTreeList(SelectedCuttingUnit?.CuttingUnitCode));

        public ICommand ShowTallyCommand => new Command(() => ShowTally());

        public ICommand ShowSettingsCommand => new Command(async () => await NavigationService.ShowSettings());

        public ICommand ShowFeedbackCommand => new Command(async () => await NavigationService.ShowFeedback());

        public ICommand ShowSampleStateManagmentCommand => new Command(async () => await NavigationService.ShowSampleStateManagment());

        public ICommand ShowCruisersCommand => new Command(async () => await NavigationService.ShowManageCruisers());

        public CuttingUnit_Ex SelectedCuttingUnit
        {
            get => _selectedCuttingUnit;
            set
            {
                SetProperty(ref _selectedCuttingUnit, value);
                RaisePropertyChanged(nameof(IsCuttingUnitSelected));
                RaisePropertyChanged(nameof(HasPlotStrata));
                RaisePropertyChanged(nameof(HasTreeStrata));
                NavigationService.ShowBlank();
            }
        }

        public IEnumerable<CuttingUnit_Ex> CuttingUnits
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

        public bool HasPlotStrata => IsCuttingUnitSelected && SelectedCuttingUnit.HasPlotStrata;

        public bool HasTreeStrata => IsCuttingUnitSelected && SelectedCuttingUnit.HasTreeStrata;

        public IDataserviceProvider DatastoreProvider { get; }
        public IAppInfoService AppInfo { get; }
        public ICruiseDialogService DialogService { get; }
        public IDeviceInfoService DeviceInfo { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ISaleDataservice SaleDataservice { get; }

        public MainViewModel(
                ICruiseNavigationService navigationService,
                ICruiseDialogService dialogService,
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
                    CuttingUnits = cuttingUnitDataservice.GetUnits();
                }
            }
        }

        private void ShowTrees()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return; }

            NavigationService.ShowTreeList(selectedUnit.CuttingUnitCode);
        }

        private void ShowPlots()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return; }

            NavigationService.ShowPlotList(selectedUnit.CuttingUnitCode);
        }

        private void ShowTally()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return; }

            NavigationService.ShowTally(selectedUnit.CuttingUnitCode);
        }

        //public override void Load()
        //{
        //    base.Load();

        //}

        protected override void OnInitialize(INavigationParameters parameters)
        {

            base.OnInitialize(parameters);
        }
    }
}