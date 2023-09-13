using NatCruise.Services;

namespace NatCruise.Test
{
    public class TestDeviceInfoService : IDeviceInfoService
    {
        public const string TEST_DEVICEID = "testDeviceID";
        public const string TEST_DEVICENAME = "testDeviceName";

        public string DeviceID => TEST_DEVICEID;

        public string DeviceName => TEST_DEVICENAME;
    }
}