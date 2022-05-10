using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Services;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.Util;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _openFileCommand;
        private ICommand _selectFileCommand;
        private ICommand _createNewFileCommand;
        private string _currentFileName;
        private ICommand _createNewTemplateCommand;
        private ICommand _shutdownCommand;

        public MainWindowViewModel(
            IAppService appService,
            IDataserviceProvider dataserviceProvider,
            IDesignNavigationService navigationService,
            IFileDialogService fileDialogService,
            IRecentFilesDataservice recentFilesDataservice,
            Prism.Services.Dialogs.IDialogService prismDialogService,
            INatCruiseDialogService dialogService,
            IDeviceInfoService deviceInfo)
        {
            AppService = appService ?? throw new ArgumentNullException(nameof(appService));
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            PrismDialogService = prismDialogService ?? throw new ArgumentNullException(nameof(prismDialogService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            RecentFilesDataservice = recentFilesDataservice ?? throw new ArgumentNullException(nameof(recentFilesDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DeviceInfoService = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public Version AppVersion { get; }
        protected IAppService AppService { get; }
        protected IDataserviceProvider DataserviceProvider { get; }
        protected IDesignNavigationService NavigationService { get; }
        protected IRecentFilesDataservice RecentFilesDataservice { get; }
        public Prism.Services.Dialogs.IDialogService PrismDialogService { get; }
        protected INatCruiseDialogService DialogService { get; }
        protected IFileDialogService FileDialogService { get; }
        protected IDeviceInfoService DeviceInfoService { get; }

        //protected IRegionNavigationService RegionNavigationService => RegionManager?.Regions[Regions.ContentRegion].NavigationService;

        public string CurrentFileName
        {
            get => _currentFileName;
            set
            {
                SetProperty(ref _currentFileName, value);
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Title => $"National Cruise System ({AppVersion}) {CurrentFileName?.Prepend(" - ")}";

        public IEnumerable<FileInfo> RecentFiles => RecentFilesDataservice?.GetRecentFiles()
            .Reverse()
            .Select(x => new FileInfo(x)).ToArray();

        public ICommand CreateNewFileCommand => _createNewFileCommand ??= new DelegateCommand(CreateNewFile);
        public ICommand CreateNewTemplateCommand => _createNewTemplateCommand ??= new DelegateCommand(() => CreateNewTemplate().FireAndForget());
        public ICommand OpenFileCommand => _openFileCommand ??= new DelegateCommand<string>((path) => OpenFile(path).FireAndForget());
        public ICommand OpenFileInfoCommand => _openFileCommand ??= new DelegateCommand<FileInfo>((fi) => OpenFile(fi).FireAndForget());
        public ICommand SelectFileCommand => _selectFileCommand ??= new DelegateCommand(() => SelectFile().FireAndForget());
        public ICommand ShutdownCommand => _shutdownCommand ??= new DelegateCommand(() => AppService.Shutdown());

        private void CreateNewFile()
        {
            PrismDialogService.Show("NewCruise", (IDialogParameters)null, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var filePath = DataserviceProvider.DatabasePath;
                    var fileExtention = Path.GetExtension(filePath).ToLower();
                    RecentFilesDataservice.AddRecentFile(filePath);
                    if (fileExtention == ".crz3")
                    {
                        NavigationService.ShowCruiseLandingLayout().FireAndForget();
                    }

                    CurrentFileName = Path.GetFileName(filePath);
                    RaisePropertyChanged(nameof(RecentFiles));
                }
            });
        }

        private async Task CreateNewTemplate()
        {
            var filePath = await FileDialogService.SelectTemplateFileDestinationAsync();
            if (filePath != null)
            {
                var fileInfo = new FileInfo(filePath);

                var extension = fileInfo.Extension.ToLower();
                if (extension == ".crz3t")
                {
                    var saleID = Guid.NewGuid().ToString();
                    var sale = new CruiseDAL.V3.Models.Sale()
                    {
                        SaleID = saleID,
                        SaleNumber = "00000",
                    };

                    var cruiseID = Guid.NewGuid().ToString();
                    var cruise = new CruiseDAL.V3.Models.Cruise()
                    {
                        CruiseID = cruiseID,
                        SaleID = saleID,
                        CruiseNumber = "00000",
                        SaleNumber = "00000",
                    };

                    var database = new CruiseDatastore_V3(fileInfo.FullName, true);
                    database.Insert(sale);
                    database.Insert(cruise);

                    DataserviceProvider.Database = database;
                    DataserviceProvider.CruiseID = cruiseID;

                    RecentFilesDataservice.AddRecentFile(filePath);
                    RaisePropertyChanged(nameof(RecentFiles));

                    NavigationService.ShowTemplateLandingLayout().FireAndForget();
                }
            }
        }

        public async Task SelectFile()
        {
            var path = await FileDialogService.SelectCruiseFileAsync();
            if (path != null)
            {
                await OpenFile(path);
            }
        }

        public Task OpenFile(string path)
        {
            return OpenFile(new FileInfo(path));
        }

        public async Task OpenFile(FileInfo file)
        {
            var filePath = file.FullName;
            var extention = file.Extension;

            if (extention is ".crz3")
            {
                var database = new CruiseDatastore_V3(filePath);

                var cruiseIDs = database.QueryScalar<string>("SELECT CruiseID FROM Cruise;").ToArray();
                if (cruiseIDs.Length > 1)
                {
                    DialogService.ShowNotification("File contains multiple cruises. \r\nOpening files with multiple cruises is not supported yet", "Warning");
                    return;
                }
                var cruiseID = cruiseIDs.First();

                DataserviceProvider.Database = database;
                DataserviceProvider.CruiseID = cruiseID;

                await NavigationService.ShowCruiseLandingLayout();
                RecentFilesDataservice.AddRecentFile(filePath);
                CurrentFileName = Path.GetFileName(filePath);
                RaisePropertyChanged(nameof(RecentFiles));
            }
            else if (extention is ".crz3t")
            {
                var database = new CruiseDatastore_V3(filePath);

                var cruiseIDs = database.QueryScalar<string>("SELECT CruiseID FROM Cruise;").ToArray();
                if (cruiseIDs.Length > 1)
                {
                    DialogService.ShowNotification("Invalid Template File", "Warning");
                    return;
                }
                var cruiseID = cruiseIDs.First();

                DataserviceProvider.Database = database;
                DataserviceProvider.CruiseID = cruiseID;

                await NavigationService.ShowTemplateLandingLayout();
                RecentFilesDataservice.AddRecentFile(filePath);
                CurrentFileName = Path.GetFileName(filePath);
                RaisePropertyChanged(nameof(RecentFiles));
            }
        }
    }
}