using NatCruise.Core.Services;
using NatCruise.Data;
using NatCruise.Design.Services;
using NatCruise.Services;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
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
            IContainerRegistry container,
            IDesignNavigationService navigationService,
            IFileDialogService fileDialogService,
            IRecentFilesDataservice recentFilesDataservice,
            Prism.Services.Dialogs.IDialogService dialogService,
            IDeviceInfoService deviceInfo)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            RecentFilesDataservice = recentFilesDataservice ?? throw new ArgumentNullException(nameof(recentFilesDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            DeviceInfoService = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        }

        protected IContainerRegistry Container { get; }
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
                if (r.Result == ButtonResult.OK)
                {
                    var filePath = DataserviceProvider.DatabasePath;
                    var fileExtention = Path.GetExtension(filePath).ToLower();
                    if (fileExtention == ".crz3")
                    {
                        NavigationService.ShowCruiseLandingLayout();
                        //RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CruiseMasterPage));
                    }

                    RecentFilesDataservice.AddRecentFile(filePath);
                    RaisePropertyChanged(nameof(RecentFiles));
                }
            });
        }

        public void OpenFile(string path)
        {
            OpenFile(new FileInfo(path));
        }

        public void OpenFile(FileInfo file)
        {

            var filePath = file.FullName;
            //DataserviceProvider.OpenDatabase(file.FullName);
            var newDataserviceProvider = new DataserviceProvider(filePath, DeviceInfoService);
            Container.RegisterInstance<IDataserviceProvider>(newDataserviceProvider);


            NavigationService.ShowCruiseLandingLayout();
            //RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CruiseMasterPage));
            RecentFilesDataservice.AddRecentFile(file.FullName);
            RaisePropertyChanged(nameof(RecentFiles));
        }

        public async void SelectFile()
        {
            var path = await FileDialogService.SelectCruiseFileAsync();
            if (path != null)
            {
                OpenFile(path);
            }
        }

        public void Exit()
        {
        }
    }
}