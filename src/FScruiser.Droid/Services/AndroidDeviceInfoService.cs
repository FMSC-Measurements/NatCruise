using Android.App;
using Android.OS;
using FScruiser.Services;
using FScruiser.Util;

using System;

namespace FScruiser.Droid.Services
{
    public class AndroidDeviceInfoService : IDeviceInfoService
    {
        private string _deviceID = string.Empty;
        private string _deviceName = string.Empty;

        public string DeviceID => !string.IsNullOrWhiteSpace(_deviceID) ? _deviceID : (_deviceID = GetUniqueDeviceID());

        public string DeviceName => !string.IsNullOrWhiteSpace(_deviceName) ? _deviceName : (_deviceName = GetDeviceName());

        public static string GetDeviceName()
        {
            var id = GetUniqueDeviceID();
            var idHash = id.GetHashCode();
            idHash = Math.Abs(idHash) % (36*36*36); //we want three alpha numeric chars so 36*36*36

            var deviceIDHashMod = idHash.ToAlphanumeric();

            var name = Android.Provider.Settings.System.GetString(Application.Context.ContentResolver, "device_name");
            if (string.IsNullOrWhiteSpace(name))
                name = Build.Model;

            return name +  "-" + deviceIDHashMod;
        }

        public static string GetUniqueDeviceID()
        {
            var id = Android.OS.Build.Serial ?? string.Empty;
            if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
            {
                try
                {
                    var context = Android.App.Application.Context;
                    id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                }
            }

            return id;
        }
    }
}