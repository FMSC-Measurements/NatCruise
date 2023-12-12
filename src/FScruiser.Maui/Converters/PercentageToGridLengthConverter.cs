using System.Globalization;

namespace FScruiser.Maui.Converters;

public class PercentageToGridLengthConverter : IValueConverter
{
    public bool Invert { get; set; }

    public int RangeUpper { get; set; } = 100;

    public GridUnitType GridUnitType { get; set; } = GridUnitType.Star;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valPercntage = (double)value!;
        if (valPercntage < 0.0 || valPercntage > 1.0) throw new ArgumentOutOfRangeException(nameof(value), valPercntage, "");

        if (Invert || (parameter is bool pInvert && pInvert is true)) { valPercntage = 1.0 - valPercntage; }

        var glValue = Math.Floor(RangeUpper * valPercntage);

        return new GridLength(glValue, GridUnitType);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}