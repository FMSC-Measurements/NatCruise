using CruiseDAL.Schema;
using NatCruise;
using System;
using System.Globalization;


namespace FScruiser.Maui.Converters;

public class CruiseMethodToBoolConverter : IValueConverter
{
    public CruiseMethodType MethodMap { get; set; }

    public bool Invert { get; set; }

    private CruiseMethodType StringToCruiseMethodType(string? method)
    {
        return method switch
        {
            CruiseMethods.H_PCT => CruiseMethodType.H_PCT,
            CruiseMethods.STR => CruiseMethodType.STR,
            CruiseMethods.S3P => CruiseMethodType.S3P,
            CruiseMethods.THREEP => CruiseMethodType.ThreeP,
            CruiseMethods.FIX => CruiseMethodType.FIX,
            CruiseMethods.F3P => CruiseMethodType.F3P,
            CruiseMethods.FCM => CruiseMethodType.FCM,
            CruiseMethods.PCM => CruiseMethodType.PCM,
            CruiseMethods.PNT => CruiseMethodType.PNT,
            CruiseMethods.P3P => CruiseMethodType.P3P,
            CruiseMethods.THREEPPNT => CruiseMethodType.ThreePPNT,
            CruiseMethods.FIXCNT => CruiseMethodType.FIXCNT,
            _ => CruiseMethodType.None,
        };
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var method = (string)value!;
        var methodType = StringToCruiseMethodType(method);

        var andMap = (methodType & MethodMap) != 0;
        return andMap ^ Invert; // xor to invert value
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
