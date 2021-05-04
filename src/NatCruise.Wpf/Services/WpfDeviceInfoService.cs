using NatCruise.Core.Services;
using NatCruise.Util;
using System;

namespace NatCruise.Wpf.Services
{
    public class WpfDeviceInfoService : IDeviceInfoService
    {
        public WpfDeviceInfoService()
        {
            var deviceID = GetUniqueDeviceID();
            var deviceHash = deviceID.GetHashCode();
            var machineShortCode = (Math.Abs(deviceHash) % (36 * 36 * 36)).ToAlphanumeric();
            var deviceName = "Win-" + machineShortCode;

            DeviceName = deviceName;
            DeviceID = deviceID;
        }

        public string DeviceID { get; }

        public string DeviceName { get; }

        public static string GetUniqueDeviceID()
        {
            return System.Environment.MachineName;
        }
    }
}