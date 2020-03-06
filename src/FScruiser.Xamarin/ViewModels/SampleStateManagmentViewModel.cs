using FScruiser.Data;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SampleStateManagmentViewModel : ViewModelBase
    {
        private Models.Device _currentDevice;
        private Command<string> _copyDeviceStateCommand;
        private IEnumerable<Models.Device> _devices;

        protected ISampleInfoDataservice SampleInfoDataservice { get; }

        public ICommand CopyDeviceStateCommand => _copyDeviceStateCommand ?? (_copyDeviceStateCommand = new Command<string>(CopyDeviceState));

        public SampleStateManagmentViewModel(INavigationService navigationService, IDataserviceProvider dataserviceProvider) : base(navigationService)
        {
            SampleInfoDataservice = dataserviceProvider.Get<ISampleInfoDataservice>();
        }

        public Models.Device CurrentDevice
        {
            get => _currentDevice;
            protected set => SetValue(ref _currentDevice, value);
        }

        public IEnumerable<Models.Device> OtherDevices
        {
            get => _devices;
            protected set => SetValue(ref _devices, value);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            try
            {
                var sids = SampleInfoDataservice;
                CurrentDevice = sids.CurrentDevice;
                OtherDevices = sids.GetOtherDevices();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void CopyDeviceState(string deviceID)
        {
            var ds = SampleInfoDataservice;

            var currentDeviceID = ds.CurrentDevice.DeviceID;
            if (currentDeviceID == deviceID) { return; }

            ds.CopySamplerStates(deviceID, currentDeviceID);
        }
    }
}