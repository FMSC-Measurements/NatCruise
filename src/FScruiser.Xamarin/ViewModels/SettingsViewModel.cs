using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CruiseDAL;
using FScruiser.XF.Data;
using FScruiser.XF.Services;
using NatCruise.Data;
using NatCruise.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : XamarinViewModelBase, INavigatedAware
    {
        public IApplicationSettingService AppSettings { get; }
        public IDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDataserviceProvider DataserviceProvider { get; }

        public ICommand ResetDatabaseCommand => new Command(() => ResetDatabase());
        public ICommand BackupDatabaseCommand => new Command(async () => await BackupDatabase());
        public ICommand LoadDatabaseCommand => new Command(LoadDatabase);
        public ICommand ShowUserAgreementCommand => new Command(() => NavigationService.ShowUserAgreement());
        public ICommand ShowPrivacyPolicyCommand => new Command(() => NavigationService.ShowPrivacyPolicy());

        public IFileDialogService FileDialogService { get; }
        public ICruiseNavigationService NavigationService { get; }

        public SettingsViewModel(IDialogService dialogService, IFileSystemService fileSystemService, IDataserviceProvider dataserviceProvider, IFileDialogService fileDialogService, ICruiseNavigationService navigationService)
        {
            AppSettings = new XamarinApplicationSettingService();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public async void ResetDatabase()
        {
            if (await DialogService.AskYesNoAsync("This will delete all cruise data do you want to continue", "Warning", defaultNo: true))
            {
                if (await DialogService.AskYesNoAsync("Backup Cruise Data Before Resetting Database?", "", defaultNo: true))
                {
                    if (!await BackupDatabase())
                    { return; }
                }

                var databasePath = FileSystemService.DefaultCruiseDatabasePath;
                File.Delete(databasePath);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Database Reset");
                var newDatabase = new CruiseDatastore_V3(databasePath, true);
                DataserviceProvider.CruiseID = null;
            }
        }

        public async Task<bool> BackupDatabase()
        {
            var timestamp = DateTime.Today.ToString("ddMMyyyy");
            var defaultFileName = $"CruiseDatabaseBackup_{timestamp}.crz3db";

            var backupPath = await FileDialogService.SelectBackupFileDestinationAsync(defaultFileName: defaultFileName);
            if (string.IsNullOrEmpty(backupPath) == false)
            {
                FileSystemService.CopyTo(DataserviceProvider.DatabasePath, backupPath);
                return true;
            }
            else
            { return false; }
        }

        public async void LoadDatabase()
        {
            var loadPath = await FileDialogService.SelectCruiseDatabaseAsync();
            if (loadPath is null) { return; }

            if (await DialogService.AskYesNoAsync("Backup current cruise data before loading?", "", defaultNo: true))
            {
                if(!await BackupDatabase())
                { return; }
            }

            var databasePath = FileSystemService.DefaultCruiseDatabasePath;
            File.Copy(loadPath, databasePath, true);
            var newDatabase = new CruiseDatastore_V3(databasePath);
            DataserviceProvider.CruiseID = null;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            AppSettings.Save();
        }

        void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            // do nothing
        }
    }
}
