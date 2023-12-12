using System.Globalization;

namespace FScruiser.Maui.Converters;

public class ReplaceNullConverter : IValueConverter
{
    public object? Value { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value ?? parameter ?? Value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // if original value is null or equals parameter/value, return null. Otherwise return value
        return value == null || value.Equals(parameter ?? Value) ? null : value;
    }
}