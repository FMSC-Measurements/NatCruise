using Microsoft.Maui.Graphics.Converters;
using System.ComponentModel;
using System.Globalization;

namespace FScruiser.Maui.Converters;

public class CountMeasureToColorConverter : IValueConverter
{
    [TypeConverter(typeof(ColorTypeConverter))]
    public Color? Measure { get; set; }

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color? Count { get; set; } = Colors.White;

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color? Insurance { get; set; }

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color Default { get; set; } = Colors.White;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var countMeasure = (string)value;
        if (countMeasure == null) { return Default; }
        switch (countMeasure.ToUpper())
        {
            case "M": return Measure ?? Default;
            case "C": return Count ?? Default;
            case "I": return Insurance ?? Default;
            default: return Default;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}