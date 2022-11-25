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
using NatCruise;
using NatCruise.Data;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : ViewModelBase, INavigatedAware
    {
        public IApplicationSettingService AppSettings { get; }
        public INatCruiseDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }
        public IDataserviceProvider DataserviceProvider { get; }

        public ICommand ShowDatabaseUtilitiesCommand => new Command(() => NavigationService.ShowDatabaseUtilities().FireAndForget());
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
