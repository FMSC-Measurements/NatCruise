using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;
using NatCruise.Data;
using NatCruise.Services;
using FScruiser.XF.Constants;
using NatCruise.Data.Abstractions;

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

    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit_Ex> _cuttingUnits;
        private CuttingUnit_Ex _selectedCuttingUnit;

        public ICruiseNavigationService NavigationService { get; }

        public ICommand ShowSelectSale => new Command(async () => await NavigationService.ShowSaleSelect());

        public ICommand ShowSaleCommand => new Command(() => NavigationService.ShowSale());

        public ICommand ShowUnitsCommand => new Command(() => NavigationService.ShowCuttingUnitList());

        public ICommand ShowTreesCommand => new Command(() => ShowTrees());

        public ICommand ShowPlotsCommand => new Command(() => ShowPlots());

        public ICommand ShowTallyCommand => new Command(() => ShowTally());

        public ICommand ShowSettingsCommand => new Command(async () => await NavigationService.ShowSettings());

        public ICommand ShowFeedbackCommand => new Command(async () => await NavigationService.ShowFeedback());

        public ICommand ShowSampleStateManagmentCommand => new Command(async () => await NavigationService.ShowSampleStateManagment());

        public CuttingUnit_Ex SelectedCuttingUnit
        {
            get => _selectedCuttingUnit;
            set
            {
                SetProperty(ref _selectedCuttingUnit, value);
                RaisePropertyChanged(nameof(IsCuttingUnitSelected));
                RaisePropertyChanged(nameof(IsCuttingUnitSelectedAndHasPlotStrata));
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
                if(cruiseID != null)
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

        public bool IsCuttingUnitSelectedAndHasPlotStrata
        {
            get { return IsCuttingUnitSelected && SelectedCuttingUnit.HasPlotStrata; } 
        }

        public bool IsCruiseSelectedAndHasPlotStrata { get; protected set; }

        public IDataserviceProvider DatastoreProvider { get; }
        public IAppInfoService AppInfo { get; }
        public ICruiseDialogService DialogService { get; }
        public IDeviceInfoService DeviceInfo { get; }
        public ICuttingUnitDatastore CuttingUnitDataservice { get; }
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

            if(datastoreProvider.CruiseID != null)
            {
                CuttingUnitDataservice = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
                SaleDataservice = datastoreProvider.GetDataservice<ISaleDataservice>();
            }

            //RefreshNavigation(null);
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

        //public void RefreshNavigation(CuttingUnit_Ex cuttingUnit)
        //{
        //    var datastore = DatastoreProvider.GetDataservice<ICuttingUnitDatastore>();

        //    var navigationItems = new List<NavigationListItem>();

        //    if (datastore != null)
        //    {
        //        navigationItems.Add(new NavigationListItem
        //        {
        //            Title = "Sale",
        //            NavigationPath = "Navigation/Sale",
        //        });

        //        navigationItems.Add(new NavigationListItem
        //        {
        //            Title = "Cutting Units",
        //            NavigationPath = "Navigation/CuttingUnits"
        //        });

        //        if (cuttingUnit != null)
        //        {
        //            if (cuttingUnit.HasTreeStrata)
        //            {
        //                navigationItems.Add(new NavigationListItem
        //                {
        //                    Title = "Tally",
        //                    NavigationPath = "Navigation/Tally",
        //                    GetParamaters = () => new NavigationParameters($"{NavParams.UNIT}={cuttingUnit.CuttingUnitCode}")
        //                });

        //                navigationItems.Add(new NavigationListItem
        //                {
        //                    Title = "Trees",
        //                    NavigationPath = "Navigation/Trees",
        //                    GetParamaters = () => new NavigationParameters($"{NavParams.UNIT}={cuttingUnit.CuttingUnitCode}")
        //                });
        //            }
        //            if (cuttingUnit.HasPlotStrata)
        //            {
        //                navigationItems.Add(new NavigationListItem
        //                {
        //                    Title = "Plots",
        //                    NavigationPath = "Navigation/Plots",
        //                    GetParamaters = () => new NavigationParameters($"{NavParams.UNIT}={cuttingUnit.CuttingUnitCode}")
        //                });
        //            }
        //        }

        //        navigationItems.Add(new NavigationListItem
        //        {
        //            Title = "Samplers",
        //            NavigationPath = "Navigation/SampleStateManagmentOther"
        //        });
        //    }

        //    //navigationItems.Add(new NavigationListItem
        //    //{
        //    //    Title = "Cruisers",
        //    //    NavigationPath = "Navigation/Cruisers"
        //    //});

        //    NavigationListItems = navigationItems;
        //}

        //public async System.Threading.Tasks.Task NavigateToAsync(NavigationListItem obj)
        //{
        //    var navParams = obj?.GetParamaters?.Invoke();

        //    try
        //    {
        //        var result = await NavigationService.NavigateAsync(obj.NavigationPath, navParams);

        //        var ex = result.Exception;
        //        if (ex != null)
        //        {
        //            Debug.WriteLine("ERROR::::" + ex);
        //            Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", obj.NavigationPath } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("ERROR::::" + ex);
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", obj.NavigationPath } });
        //    }
        //}

        //public void ShowFeedback()
        //{
        //    NavigationService.ShowFeedback();

        //    //NavigationService.NavigateAsync("Feedback", useModalNavigation: true);
        //}

        //public void ShowSettings()
        //{
        //    NavigationService.ShowSettings();

        //    //NavigationService.NavigateAsync("Settings", useModalNavigation: true);
        //}

        //public void ShowSampleStateManagment()
        //{
        //    NavigationService.ShowSampleStateManagment();

        //    //NavigationService.NavigateAsync("SampleStateManagment", useModalNavigation: true);
        //}

        //private async void SelectFileAsync(object obj)
        //{
        //    try
        //    {
        //        var filePath = await FileDialogService.SelectCruiseFileAsync();
        //        if (filePath == null) { return; }

        //        //var fileData = await CrossFilePicker.Current.PickFile(new string[] { ".cruise", ".crz3" });
        //        //if (fileData == null) { return; }//user canceled file picking

        //        //var filePath = fileData.FilePath;

        //        //Check path exists
        //        if (File.Exists(filePath) == false)
        //        {
        //            await DialogService.ShowMessageAsync(filePath, "File Doesn't Exist").ConfigureAwait(false);
        //            return;
        //        }

        //        MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_SELECTED, filePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error:::{ex.Message}");
        //        Debug.WriteLine(ex.StackTrace);
        //        Crashes.TrackError(ex);
        //    }
        //}

        //public override void OnNavigatedFrom(INavigationParameters parameters)
        //{
        //    base.OnNavigatedFrom(parameters);

        //    MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_OPENED);
        //    MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_SELECTED);
        //}

        //public override void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    base.OnNavigatedTo(parameters);

        //    MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED,
        //        MessagingCenter_CruiseFileOpened);

        //    MessagingCenter.Subscribe<string>(this, Messages.CUTTING_UNIT_SELECTED,
        //        MessagingCenter_CuttingUnitSelected);
        //}

        //private void MessagingCenter_CuttingUnitSelected(string unit)
        //{
        //    var datastore = DatastoreProvider.GetDataservice<ICuttingUnitDatastore>();
        //    var cuttingUnit = datastore.GetUnit(unit);

        //    RefreshNavigation(cuttingUnit);

        //    RaisePropertyChanged(nameof(NavigationListItems));
        //}

        //private async void MessagingCenter_CruiseFileOpened(object sender, string path)
        //{
        //    RefreshNavigation(null);

        //    RaisePropertyChanged(nameof(CurrentCruiseName));
        //    RaisePropertyChanged(nameof(NavigationListItems));
        //}

        protected override void Refresh(INavigationParameters parameters)
        {
            var cuttingUnitDataservice = CuttingUnitDataservice;
            if(cuttingUnitDataservice != null)
            {
                CuttingUnits = cuttingUnitDataservice.GetUnits();
            }

            //do nothing
        }
    }
}