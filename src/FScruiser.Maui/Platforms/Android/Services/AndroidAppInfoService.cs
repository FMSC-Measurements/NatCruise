﻿using NatCruise.Services;

namespace FScruiser.Maui.Platforms.Android.Services;

public class AndroidAppInfoService : IAppInfoService
{
    string _version = string.Empty;

    public string Version => !string.IsNullOrWhiteSpace(_version) ? _version : (_version = GetVersion());

    public static string GetVersion()
    {
        var context = global::Android.App.Application.Context;

        var manager = context.PackageManager;
        var info = manager?.GetPackageInfo(context.PackageName, 0);

        return info?.VersionName ?? "";
    }
}