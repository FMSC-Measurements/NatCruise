using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class Is3PConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cruiseMethod = (string)value;
            return CruiseDAL.Schema.CruiseMethods.THREE_P_METHODS.Contains(cruiseMethod);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}