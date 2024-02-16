using CruiseDAL;
using FScruiser.XF.Services;
using Microsoft.Extensions.DependencyInjection;
using NatCruise;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class DatabaseUtilitiesViewModel : ViewModelBase
    {
        public ICommand ResetDatabaseCommand => new Command(() => ResetDatabase().FireAndForget());
        public ICommand BackupDatabaseCommand => new Command(() => BackupDatabase().FireAndForget());
        public ICommand LoadDatabaseCommand => new Command(() => LoadDatabase().FireAndForget());
        public INatCruiseDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDataContextService DataContext { get; }
        public IFileDialogService FileDialogService { get; }
        public ICruiseNavigationService NavigationService { get; }

        public DatabaseUtilitiesViewModel(INatCruiseDialogService dialogService,
            IFileSystemService fileSystemService,
            IFileDialogService fileDialogService,
            ICruiseNavigationService cruiseNavigationService,
            IServiceProvider serviceProvider)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            NavigationService = cruiseNavigationService ?? throw new ArgumentNullException(nameof(cruiseNavigationService));

            DataContext = serviceProvider.GetService<IDataContextService>();
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
                var context = DataContext;
                if(context != null)
                {
                    context.Database?.ReleaseConnection(true);

                    var databasePath = FileSystemService.DefaultCruiseDatabasePath;
                    File.Delete(databasePath);
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Database Reset");
                    var newDatabase = new CruiseDatastore_V3(databasePath, true);
                    if (context != null)
                    {
                        context.CruiseID = null;
                    }
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
                if (!await BackupDatabase())
                { return; }
            }

            var dataContext = DataContext;
            if (dataContext != null)
            {
                dataContext.Database?.ReleaseConnection(true);
            }

            // data context might be null but we will try to copy in the new database regardless
            var databasePath = FileSystemService.DefaultCruiseDatabasePath;
            File.Copy(loadPath, databasePath, true);
            var newDatabase = new CruiseDatastore_V3(databasePath);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Load Database");
            if (dataContext != null)
            {
                dataContext.CruiseID = null;
            }
        }
    }
}
