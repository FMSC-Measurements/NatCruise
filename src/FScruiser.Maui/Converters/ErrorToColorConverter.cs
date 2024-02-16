using Microsoft.Maui.Graphics.Converters;
using NatCruise.Models;
using System.ComponentModel;
using System.Globalization;

namespace FScruiser.Maui.Converters;

public class ErrorToColorConverter : IValueConverter
{
    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Default { get; set; } = Colors.White;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Error { get; set; } = Colors.Red;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Warning { get; set; } = Colors.Gold;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color SuppressedColor { get; set; } = Colors.LightGray;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var error = (ErrorBase)value;
        if (error == null) { return Default; }

        if (error.IsResolved) { return SuppressedColor; }

        switch (error.Level.ToUpper())
        {
            case "E": return Error;
            case "W": return Warning;
            default: return Default;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}