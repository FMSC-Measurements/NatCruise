using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF
{
    public class TestDeviceInfoService : IDeviceInfoService
    {
        public string DeviceID => "testDeviceID";

        public string DeviceName => "testDeviceName";
    }
}
