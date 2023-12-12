using Microsoft.Maui.Graphics.Converters;
using System.ComponentModel;
using System.Globalization;

namespace FScruiser.Maui.Converters;

public class ErrorLevelToColorConverter : IValueConverter
{
    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Default { get; set; } = Colors.White;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Error { get; set; } = Colors.Red;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Warning { get; set; } = Colors.Gold;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var level = (string)value!;
        if (level == null) { return Default; }
        switch (level.ToUpper())
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