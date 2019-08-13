using NatCruise.Wpf.Data;
using NatCruise.Wpf.Navigation;
using NatCruise.Wpf.Services;
using NatCruise.Wpf.Views;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _openFileCommand;
        private ICommand _selectFileCommand;
        private ICommand _createNewFileCommand;

        public MainWindowViewModel(IRegionManager regionManager, 
            IFileDialogService fileDialogService, 
            IRecentFilesDataservice recentFilesDataservice, 
            IDataserviceProvider dataserviceProvider,
            IDialogService dialogService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            RegionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            //RegionNavigationService = regionNavigationService ?? throw new ArgumentNullException(nameof(regionNavigationService));
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            RecentFilesDataservice = recentFilesDataservice ?? throw new ArgumentNullException(nameof(recentFilesDataservice));
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
        }

        protected IDataserviceProvider DataserviceProvider { get; }

        protected IRecentFilesDataservice RecentFilesDataservice { get; }
        public IDialogService DialogService { get; }
        public IRegionManager RegionManager { get; }
        protected IFileDialogService FileDialogService { get; }

        //protected IRegionNavigationService RegionNavigationService => RegionManager?.Regions[Regions.ContentRegion].NavigationService;




        public string Title => "National Cruise System";

        public IEnumerable<FileInfo> RecentFiles => RecentFilesDataservice?.GetRecentFiles();

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
                    var filePath = DataserviceProvider.CruiseFilePath;
                    var fileExtention = Path.GetExtension(filePath).ToLower();
                    if (fileExtention == ".crz3")
                    {
                        RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CruiseMasterPage));
                    }
                }
            });

        }

        public void OpenFile(string path)
        {
            OpenFile(new FileInfo(path));
        }

        public void OpenFile(FileInfo file)
        {
            DataserviceProvider.OpenFile(file.FullName);

            RegionManager.RequestNavigate(Regions.ContentRegion, nameof(CruiseMasterPage));
        }

        public void SelectFile()
        {
            var path = FileDialogService.SelectCruiseFile();
            if(path != null)
            {
                OpenFile(path);
            }

        }



        public void Exit()
        {

        }

        protected override void Load()
        {
            
        }
    }
}
