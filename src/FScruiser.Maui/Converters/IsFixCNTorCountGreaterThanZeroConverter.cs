using CruiseDAL.Schema;
using NatCruise.Models;
using System.Globalization;

namespace FScruiser.Maui.Converters;

public class IsFixCNTorCountGreaterThanOneConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return false;
        var tree = (PlotTreeEntry)value;
        return tree.Method == CruiseMethods.FIXCNT || tree.TreeCount > 1;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}