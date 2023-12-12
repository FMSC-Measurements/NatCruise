using System.Globalization;

namespace FScruiser.Maui.Converters;

//TODO remove this converterr and switch to using cruisemethodtoboolconverter
public class IsSTRConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var cruiseMethod = (string)value!;
        return cruiseMethod == CruiseDAL.Schema.CruiseMethods.STR;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}