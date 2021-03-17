using CruiseDAL;
using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Services;
using NatCruise.Services;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _openFileCommand;
        private ICommand _selectFileCommand;
        private ICommand _createNewFileCommand;

        public MainWindowViewModel(
            IDataserviceProvider dataserviceProvider,
            IDesignNavigationService navigationService,
            IFileDialogService fileDialogService,
            IRecentFilesDataservice recentFilesDataservice,
            Prism.Services.Dialogs.IDialogService dialogService,
            IDeviceInfoService deviceInfo)
        {
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            RecentFilesDataservice = recentFilesDataservice ?? throw new ArgumentNullException(nameof(recentFilesDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DeviceInfoService = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        protected IDataserviceProvider DataserviceProvider { get; }
        protected IDesignNavigationService NavigationService { get; }
        protected IRecentFilesDataservice RecentFilesDataservice { get; }
        public Prism.Services.Dialogs.IDialogService DialogService { get; }
        protected IFileDialogService FileDialogService { get; }
        protected IDeviceInfoService DeviceInfoService { get; }

        //protected IRegionNavigationService RegionNavigationService => RegionManager?.Regions[Regions.ContentRegion].NavigationService;

        public string Title => "National Cruise System (0.2.1-Alpha)";

        public IEnumerable<FileInfo> RecentFiles => RecentFilesDataservice?.GetRecentFiles().Select(x => new FileInfo(x));

        public ICommand CreateNewFileCommand => _createNewFileCommand ?? (_createNewFileCommand = new DelegateCommand(CreateNewFile));

        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new DelegateCommand<string>(OpenFile));

        public ICommand OpenFileInfoCommand => _openFileCommand ?? (_openFileCommand = new DelegateCommand<FileInfo>(OpenFile));

        public ICommand SelectFileCommand => _selectFileCommand ?? (_selectFileCommand = new DelegateCommand(SelectFile));

        private void CreateNewFile()
        {
            DialogService.Show("NewCruise", (IDialogParameters)null, r =>
            {
                if(r.Result == ButtonResult.OK)
                {
                    var filePath = DataserviceProvider.DatabasePath;
                    var fileExtention = Path.GetExtension(filePath).ToLower();
                    RecentFilesDataservice.AddRecentFile(filePath);
                    if (fileExtention == ".crz3")
                    {
                        NavigationService.ShowCruiseLandingLayout();
                    }

                    RaisePropertyChanged(nameof(RecentFiles));
                }
            });
        }

        public async void SelectFile()
        {
            var path = await FileDialogService.SelectCruiseFileAsync();
            if (path != null)
            {
                OpenFile(path);
            }
        }

        public void OpenFile(string path)
        {
            OpenFile(new FileInfo(path));
        }

        public void OpenFile(FileInfo file)
        {
            var filePath = file.FullName;
            var database = new CruiseDatastore_V3(filePath);
            DataserviceProvider.Database = database;

            NavigationService.ShowCruiseLandingLayout();
            RecentFilesDataservice.AddRecentFile(filePath);
            RaisePropertyChanged(nameof(RecentFiles));
        }
    }
}