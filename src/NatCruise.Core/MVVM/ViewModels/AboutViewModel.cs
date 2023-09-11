using NatCruise.Async;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Reflection;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public ICommand ShowUserAgreementCommand => new DelegateCommand(() => NavigationService.ShowUserAgreement().FireAndForget());
        public ICommand ShowPrivacyPolicyCommand => new DelegateCommand(() => NavigationService.ShowPrivacyPolicy().FireAndForget());

        public INatCruiseNavigationService NavigationService { get; }

        public AboutViewModel(IDeviceInfoService deviceInfoDataservice, INatCruiseNavigationService navigationService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            var assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version.ToString();
            DeviceName = deviceInfoDataservice.DeviceName;
            DeviceID = deviceInfoDataservice.DeviceID;
        }

        public string Version { get; }

        public string DeviceName { get; }

        public string DeviceID { get; }
    }
}