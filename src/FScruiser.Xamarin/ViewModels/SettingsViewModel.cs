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
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : XamarinViewModelBase, INavigatedAware
    {
        public IApplicationSettingService AppSettings { get; }
        public INatCruiseDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDataserviceProvider DataserviceProvider { get; }

        public ICommand ResetDatabaseCommand => new Command(() => ResetDatabase().FireAndForget());
        public ICommand BackupDatabaseCommand => new Command(() => BackupDatabase().FireAndForget());
        public ICommand LoadDatabaseCommand => new Command(() => LoadDatabase().FireAndForget());
        public ICommand ShowUserAgreementCommand => new Command(() => NavigationService.ShowUserAgreement().FireAndForget());
        public ICommand ShowPrivacyPolicyCommand => new Command(() => NavigationService.ShowPrivacyPolicy().FireAndForget());

        public IFileDialogService FileDialogService { get; }
        public ICruiseNavigationService NavigationService { get; }

        public SettingsViewModel(INatCruiseDialogService dialogService, IFileSystemService fileSystemService, IFileDialogService fileDialogService, ICruiseNavigationService navigationService, IContainerProvider containerProvicer)
        {
            AppSettings = new XamarinApplicationSettingService();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            //DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            try
            {
                DataserviceProvider = containerProvicer.Resolve<IDataserviceProvider>();
            }
            catch { }
        }

        public async Task ResetDatabase()
        {
            if (await DialogService.AskYesNoAsync("Backup Cruise Data Before Resetting Database?", "", defaultNo: true))
            {
                if (!await BackupDatabase())
                { return; }
            }

            if (await DialogService.AskYesNoAsync("Resetting Database will delete all cruise data.\r\n Do you want to continue", "Warning", defaultNo: true))
            {
                var database = DataserviceProvider.Database;
                database.ReleaseConnection(true);
                var databasePath = FileSystemService.DefaultCruiseDatabasePath;
                File.Delete(databasePath);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Database Reset");
                var newDatabase = new CruiseDatastore_V3(databasePath, true);
                if (DataserviceProvider != null)
                {
                    DataserviceProvider.CruiseID = null;
                }
            }
        }

        public async Task<bool> BackupDatabase()
        {
            var timestamp = DateTime.Today.ToString("ddMMyyyy");
            var defaultFileName = $"CruiseDatabaseBackup_{timestamp}.crz3db";

            var backupPath = await FileDialogService.SelectBackupFileDestinationAsync(defaultFileName: defaultFileName);
            if (string.IsNullOrEmpty(backupPath) == false)
            {
                FileSystemService.CopyTo(FileSystemService.DefaultCruiseDatabasePath, backupPath);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Backup Database");
                return true;
            }
            else
            { return false; }
        }

        public async Task LoadDatabase()
        {
            var loadPath = await FileDialogService.SelectCruiseDatabaseAsync();
            if (loadPath is null) { return; }

            if (await DialogService.AskYesNoAsync("Backup current cruise data before loading?", "", defaultNo: false))
            {
                if(!await BackupDatabase())
                { return; }
            }

            var database = DataserviceProvider.Database;
            database.ReleaseConnection(true);
            var databasePath = database.Path;
            File.Copy(loadPath, databasePath, true);
            var newDatabase = new CruiseDatastore_V3(databasePath);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Load Database");
            if (DataserviceProvider != null)
            {
                DataserviceProvider.CruiseID = null;
            }
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
