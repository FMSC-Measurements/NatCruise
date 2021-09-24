using System;
using System.Globalization;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i) { return i > 0; }
            else if (value is double d) { return d > 0.0; }
            else if (value is float f) { return f > 0.0f; }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}