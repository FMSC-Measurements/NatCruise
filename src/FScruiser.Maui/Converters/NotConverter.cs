using System.Globalization;

namespace FScruiser.Maui.Converters;

public class NotConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return Binding.DoNothing;

        return !((bool)value);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return Binding.DoNothing;

        return !((bool)value);
    }
}