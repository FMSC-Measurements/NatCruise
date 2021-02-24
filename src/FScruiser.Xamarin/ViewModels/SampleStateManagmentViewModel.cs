﻿using NatCruise.Cruise.Data;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using NatCruise.Data;

namespace FScruiser.XF.ViewModels
{
    public class SampleStateManagmentViewModel : ViewModelBase
    {
        private NatCruise.Cruise.Models.Device _currentDevice;
        private Command<string> _copyDeviceStateCommand;
        private IEnumerable<NatCruise.Cruise.Models.Device> _devices;

        protected ISampleInfoDataservice SampleInfoDataservice { get; }

        public ICommand CopyDeviceStateCommand => _copyDeviceStateCommand ?? (_copyDeviceStateCommand = new Command<string>(CopyDeviceState));

        public SampleStateManagmentViewModel( IDataserviceProvider dataserviceProvider)
        {
            SampleInfoDataservice = dataserviceProvider.GetDataservice<ISampleInfoDataservice>();
        }

        public NatCruise.Cruise.Models.Device CurrentDevice
        {
            get => _currentDevice;
            protected set => SetProperty(ref _currentDevice, value);
        }

        public IEnumerable<NatCruise.Cruise.Models.Device> OtherDevices
        {
            get => _devices;
            protected set => SetProperty(ref _devices, value);
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