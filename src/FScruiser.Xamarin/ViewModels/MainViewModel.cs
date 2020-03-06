using FScruiser.Data;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class NavigationListItem
    {
        public string Title { get; set; }

        public string NavigationPath { get; set; }

        public bool CanShow
        {
            get
            {
                return CanShowAction?.Invoke() ?? true;
            }
        }

        public Func<NavigationParameters> GetParamaters { get; set; }

        public Func<bool> CanShowAction { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private Command _selectFileCommand;
        private Command<NavigationListItem> _navigateCommand;
        private Command _showSettingsCommand;
        private Command _showFeedbackCommand;
        private Command _showSampleStateManagmentCommand;

        public ICommand SelectFileCommand => _selectFileCommand ?? (_selectFileCommand = new Command(SelectFileAsync));

        public ICommand ShowSettingsCommand => _showSettingsCommand ?? (_showSettingsCommand = new Command(ShowSettings));

        public ICommand ShowFeedbackCommand => _showFeedbackCommand ?? (_showFeedbackCommand = new Command(ShowFeedback));

        public ICommand ShowSampleStateManagmentCommand => _showSampleStateManagmentCommand ?? (_showSampleStateManagmentCommand = new Command(ShowSampleStateManagment));

        public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new Command<NavigationListItem>(async (x) => await NavigateToAsync(x)));

        public IEnumerable<NavigationListItem> NavigationListItems { get; set; }

        public CuttingUnit SelectedCuttingUnit { get; protected set; }

        public string CurrentFilePath
        {
            get
            {
                var filePath = DatastoreProvider.CruisePath;
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return "Open File";
                }
                else
                {
                    return Path.GetFileName(filePath);
                }
            }
        }

        public IDataserviceProvider DatastoreProvider { get; }
        public IAppInfoService AppInfo { get; }
        protected IDialogService DialogService { get; set; }

        protected IFilePickerService FilePickerService { get; }
        public IDeviceInfoService DeviceInfo { get; }

        public MainViewModel(INavigationService navigationService
            , IDialogService dialogService
            , IDataserviceProvider datastoreProvider
            , IDeviceInfoService deviceInfoService
            , IAppInfoService appInfo
            , IFilePickerService filePickerService) : base(navigationService)
        {
            AppInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            DialogService = dialogService;
            DatastoreProvider = datastoreProvider;
            FilePickerService = filePickerService ?? throw new ArgumentNullException(nameof(filePickerService));
            DeviceInfo = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));

            RefreshNavigation(null);
        }

        public void RefreshNavigation(CuttingUnit_Ex cuttingUnit)
        {
            var datastore = DatastoreProvider.Get<ICuttingUnitDatastore>();

            var navigationItems = new List<NavigationListItem>();

            if (datastore != null)
            {
                navigationItems.Add(new NavigationListItem
                {
                    Title = "Sale",
                    NavigationPath = "Navigation/Sale",
                });

                navigationItems.Add(new NavigationListItem
                {
                    Title = "Cutting Units",
                    NavigationPath = "Navigation/CuttingUnits"
                });

                if (cuttingUnit != null)
                {
                    if (cuttingUnit.HasTreeStrata)
                    {
                        navigationItems.Add(new NavigationListItem
                        {
                            Title = "Tally",
                            NavigationPath = "Navigation/Tally",
                            GetParamaters = () => new NavigationParameters($"UnitCode={cuttingUnit.Code}")
                        });

                        navigationItems.Add(new NavigationListItem
                        {
                            Title = "Trees",
                            NavigationPath = "Navigation/Trees",
                            GetParamaters = () => new NavigationParameters($"UnitCode={cuttingUnit.Code}")
                        });
                    }
                    if (cuttingUnit.HasPlotStrata)
                    {
                        navigationItems.Add(new NavigationListItem
                        {
                            Title = "Plots",
                            NavigationPath = "Navigation/Plots",
                            GetParamaters = () => new NavigationParameters($"UnitCode={cuttingUnit.Code}")
                        });
                    }
                }

                navigationItems.Add(new NavigationListItem
                {
                    Title = "Samplers",
                    NavigationPath = "Navigation/SampleStateManagmentOther"
                });
            }

            //navigationItems.Add(new NavigationListItem
            //{
            //    Title = "Cruisers",
            //    NavigationPath = "Navigation/Cruisers"
            //});

            NavigationListItems = navigationItems;
        }

        public async System.Threading.Tasks.Task NavigateToAsync(NavigationListItem obj)
        {
            var navParams = obj?.GetParamaters?.Invoke();

            try
            {
                var result = await NavigationService.NavigateAsync(obj.NavigationPath, navParams);

                var ex = result.Exception;
                if (ex != null)
                {
                    Debug.WriteLine("ERROR::::" + ex);
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", obj.NavigationPath } });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR::::" + ex);
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", obj.NavigationPath } });
            }
        }

        public void ShowFeedback()
        {
            NavigationService.NavigateAsync("Feedback", useModalNavigation: true);
        }

        public void ShowSettings()
        {
            NavigationService.NavigateAsync("Settings", useModalNavigation: true);
        }

        public void ShowSampleStateManagment()
        {
            NavigationService.NavigateAsync("SampleStateManagment", useModalNavigation: true);
        }

        private async void SelectFileAsync(object obj)
        {
            try
            {
                var filePath = await FilePickerService.PickCruiseFileAsync();
                if (filePath == null) { return; }

                //var fileData = await CrossFilePicker.Current.PickFile(new string[] { ".cruise", ".crz3" });
                //if (fileData == null) { return; }//user canceled file picking

                //var filePath = fileData.FilePath;

                //Check path exists
                if (File.Exists(filePath) == false)
                {
                    await DialogService.ShowMessageAsync(filePath, "File Doesn't Exist").ConfigureAwait(false);
                    return;
                }

                MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_SELECTED, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error:::{ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                Crashes.TrackError(ex);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_OPENED);
            MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_SELECTED);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED,
                MessagingCenter_CruiseFileOpened);

            MessagingCenter.Subscribe<string>(this, Messages.CUTTING_UNIT_SELECTED,
                MessagingCenter_CuttingUnitSelected);
        }

        private void MessagingCenter_CuttingUnitSelected(string unit)
        {
            var datastore = DatastoreProvider.Get<ICuttingUnitDatastore>();
            var cuttingUnit = datastore.GetUnit(unit);

            RefreshNavigation(cuttingUnit);

            RaisePropertyChanged(nameof(NavigationListItems));
        }

        private async void MessagingCenter_CruiseFileOpened(object sender, string path)
        {
            SelectedCuttingUnit = null;

            RefreshNavigation(null);

            RaisePropertyChanged(nameof(this.CurrentFilePath));
            RaisePropertyChanged(nameof(NavigationListItems));
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            //do nothing
        }
    }
}