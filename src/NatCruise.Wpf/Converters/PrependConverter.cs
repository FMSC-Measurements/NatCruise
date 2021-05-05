using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class PrependConverter : IValueConverter
    {
        public object Value { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) { return null; }

            var items = (IEnumerable<object>)value;
            return Enumerable.Prepend(items, parameter ?? Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}