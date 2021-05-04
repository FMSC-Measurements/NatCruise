using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class IsNullOrWhiteSpaceConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sValue = value as String;

            var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(sValue);
            return isNullOrWhiteSpace ^ Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
