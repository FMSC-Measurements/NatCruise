using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class ContainsConverter : IValueConverter
    {
        public object[] Items { get; set; }

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                var containsValue = Items.OfType<string>().Contains(s, StringComparer.InvariantCultureIgnoreCase);
                return containsValue ^ Invert;
            }
            else
            {
                var containsValue = Items.OfType<object>().Contains(value);
                return containsValue ^ Invert;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
