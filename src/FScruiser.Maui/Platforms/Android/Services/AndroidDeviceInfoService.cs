using Android.Content;
using Android.OS;
using Android.Provider;
using NatCruise.Services;
using NatCruise.Util;

namespace FScruiser.Maui.Platforms.Android.Services;

public class AndroidDeviceInfoService : IDeviceInfoService
{
    private const string DEVICE_INFO_PREF = "device_info";
    private const string DEVICE_ID = "device_id";
    private const string DEVICE_NAME = "device_name";

    private string _deviceID = string.Empty;
    private string _deviceName = string.Empty;

    //public AndroidDeviceInfoService(IMauiContext context)
    //    : this(context.Context)
    //{ }

    public AndroidDeviceInfoService() : this(Microsoft.Maui.ApplicationModel.Platform.AppContext)
    {
        
    }

    protected AndroidDeviceInfoService(Context context)
    {
        var deviceInfoPref = context.GetSharedPreferences(DEVICE_INFO_PREF, FileCreationMode.MultiProcess);

        var deviceID = deviceInfoPref.GetString(DEVICE_ID, null);

        if (string.IsNullOrWhiteSpace(deviceID))
        {
            deviceID = GenerateUniqueDeviceID();
            var editor = deviceInfoPref.Edit();
            editor.PutString(DEVICE_ID, deviceID);
            editor.Commit();
        }

        var deviceName = deviceInfoPref.GetString(DEVICE_NAME, null);
        if (string.IsNullOrWhiteSpace(DeviceName))
        {
            deviceName = GenerateDeviceName(deviceID, context);
            var editor = deviceInfoPref.Edit();
            editor.PutString(DEVICE_NAME, deviceName);
            editor.Commit();
        }

        DeviceID = deviceID;
        DeviceName = deviceName;
    }

    public string DeviceID { get; }

    public string DeviceName { get; }

    public static string GenerateDeviceName(string id, Context context)
    {
        var idHash = id.GetHashCode();
        idHash = Math.Abs(idHash) % (36 * 36 * 36 * 36); //we want four alpha numeric chars so 36*36*36*36

        var deviceIDHashMod = idHash.ToAlphanumeric();

        var name = Settings.Global.GetString(context.ContentResolver, Settings.Global.DeviceName);
        if (string.IsNullOrWhiteSpace(name))
            name = Build.Model;
        name = name.Replace(" ", "");

        return name + "-" + deviceIDHashMod;
    }

    public static string GenerateUniqueDeviceID()
    {
        var id = Guid.NewGuid().ToString();
        return id;
    }
}