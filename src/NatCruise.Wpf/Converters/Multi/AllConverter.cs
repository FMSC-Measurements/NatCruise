using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters.Multi
{
    public class AllConverter : BooleanMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = values.Select(x => (x == null || x == DependencyProperty.UnsetValue) ? DefaultValue : (bool)x).All(x => x);
            return GetValueForResult(result);
        }
    }
}