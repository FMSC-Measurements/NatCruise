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
        public ICommand BackupDatabaseCommand => new Command(BackupDatabase);
        public ICommand ShowUserAgreement => new Command(() => NavigationService.ShowUserAgreement());

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
                var databasePath = FileSystemService.DefaultCruiseDatabasePath;
                File.Delete(databasePath);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Database Reset");
                var newDatabase = new CruiseDatastore_V3(databasePath, true);
                DataserviceProvider.CruiseID = null;
            }
        }

        public async void BackupDatabase()
        {
            var timestamp = DateTime.Today.ToString("ddMMyyyy");
            var defaultFileName = $"CruiseDatabaseBackup_{timestamp}.crz3db";

            var backupPath = await FileDialogService.SelectBackupFileDestinationAsync(defaultFileName: defaultFileName);
            if(string.IsNullOrEmpty(backupPath) == false)
            {

                FileSystemService.CopyTo(DataserviceProvider.DatabasePath, backupPath);
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
