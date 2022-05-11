using Microsoft.AppCenter.Crashes;
using NatCruise.Data;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SampleStateManagmentViewModel : XamarinViewModelBase
    {
        //private NatCruise.Cruise.Models.Device _currentDevice;
        private Command<string> _copyDeviceStateCommand;

        private IEnumerable<NatCruise.Models.Device> _devices;

        protected ISamplerStateDataservice SampleStateDataservice { get; }
        public IDeviceDataservice DeviceDataservice { get; }

        //public IDeviceInfoService DeviceInfoService { get; }

        public ICommand CopyDeviceStateCommand => _copyDeviceStateCommand ?? (_copyDeviceStateCommand = new Command<string>(CopyDeviceState));

        public SampleStateManagmentViewModel(ISamplerStateDataservice samplerStateDataservice, IDeviceDataservice deviceDataservice)
        {
            SampleStateDataservice = samplerStateDataservice ?? throw new ArgumentNullException(nameof(samplerStateDataservice));
            DeviceDataservice = deviceDataservice ?? throw new ArgumentNullException(nameof(deviceDataservice));

            //DeviceInfoService = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));
            //CurrentDevice = new NatCruise.Cruise.Models.Device
            //{
            //    DeviceID = deviceInfoService.DeviceID,
            //    Name = deviceInfoService.DeviceName,
            //};
        }

        //public NatCruise.Cruise.Models.Device CurrentDevice
        //{
        //    get => _currentDevice;
        //    protected set => SetProperty(ref _currentDevice, value);
        //}

        public IEnumerable<NatCruise.Models.Device> OtherDevices
        {
            get => _devices;
            protected set => SetProperty(ref _devices, value);
        }

        protected override void Load(IParameters parameters)
        {
            try
            {
                OtherDevices = DeviceDataservice.GetOtherDevices();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void CopyDeviceState(string deviceID)
        {
            var ds = SampleStateDataservice;

            var currentDeviceID = ds.DeviceID;
            if (currentDeviceID == deviceID) { return; }

            ds.CopySamplerStates(deviceID, currentDeviceID);
        }
    }
}