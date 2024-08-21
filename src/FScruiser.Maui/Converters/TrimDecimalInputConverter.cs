using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Converters
{
    public class TrimDecimalInputConverter : IValueConverter
    {
        public int DecimalPlaces { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;

            if (value is string s && Double.TryParse(s, out var d))
            {
                if (DecimalPlaces == 0) return Math.Floor(d).ToString();
                var num = Math.Floor(d * Math.Pow(10.0d, DecimalPlaces))
                    / Math.Pow(10.0d, DecimalPlaces);
                return num.ToString();
            }

            return value;
        }
    }
}
