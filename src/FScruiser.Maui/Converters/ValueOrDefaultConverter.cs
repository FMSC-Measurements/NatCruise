using System.Globalization;

namespace FScruiser.Maui.Converters;

public class ValueOrDefaultConverter<T> : IValueConverter
{
    public static ValueOrDefaultConverter<T> Instance { get; } = new ValueOrDefaultConverter<T>();

    public T? DefaultValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value ?? DefaultValue;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value is null || ((T)value).Equals(DefaultValue)) ? null : value;
}