using FScruiser.XF.Data;
using FScruiser.XF.Services;
using NatCruise.Async;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public IApplicationSettingService AppSettings { get; }
        public ITallySettingsDataService TallySettings { get; }
        public INatCruiseDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }

        public ICommand ShowDatabaseUtilitiesCommand => new Command(() => NavigationService.ShowDatabaseUtilities().FireAndForget());

        public IFileDialogService FileDialogService { get; }
        public ICruiseNavigationService NavigationService { get; }
        public ILoggingService LoggingService { get; }

        public SettingsViewModel(INatCruiseDialogService dialogService,
                                 IFileSystemService fileSystemService,
                                 IFileDialogService fileDialogService,
                                 ICruiseNavigationService navigationService,
                                 ITallySettingsDataService tallySettingsDataService,
                                 ILoggingService loggingService)
        {
            AppSettings = new XamarinApplicationSettingService();
            TallySettings = tallySettingsDataService ?? throw new ArgumentNullException(nameof(tallySettingsDataService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

            AppSettings.PropertyChanged += AppSettings_PropertyChanged;
        }

        private void AppSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            if (propName == nameof(IApplicationSettingService.SelectPrevNextTreeSkipsCountTrees))
            {
                var value = AppSettings.SelectPrevNextTreeSkipsCountTrees.ToString();
                LoggingService.LogEvent(nameof(SettingsViewModel) + ":" + nameof(AppSettings.SelectPrevNextTreeSkipsCountTrees) + " changed",
                    new Dictionary<string, string> { { "value", value } });
            }
            else if (propName == nameof(IApplicationSettingService.UseNewLimitingDistanceCalculator))
            {
                var value = AppSettings.UseNewLimitingDistanceCalculator.ToString();
                LoggingService.LogEvent(nameof(SettingsViewModel) + ":" + nameof(AppSettings.UseNewLimitingDistanceCalculator) + " changed",
                    new Dictionary<string, string> { { "value", value } });
            }
        }
    }
}