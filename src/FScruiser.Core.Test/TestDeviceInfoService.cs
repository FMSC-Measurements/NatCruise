using FScruiser.Services;
using System;

namespace FScruiser.Core.Test
{
    public class TestDeviceInfoService : IDeviceInfoService
    {
        public TestDeviceInfoService() : this("testDeviceID", "testDeviceName")
        {
        }

        public TestDeviceInfoService(string deviceID, string deviceName)
        {
            DeviceID = deviceID ?? throw new ArgumentNullException(nameof(deviceID));
            DeviceName = deviceName ?? throw new ArgumentNullException(nameof(deviceName));
        }

        public string DeviceID
        {
            get;
            set;
        }

        public string DeviceName
        {
            get;
            set;
        }
    }
}