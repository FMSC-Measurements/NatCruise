using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class IsVisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(value is bool)
                {
                    var isVisable = (bool)value;
                    return isVisable ? Visibility.Visible : Visibility.Collapsed;
                }
                else if(value is string)
                {
                    var isVisable = !string.IsNullOrEmpty((string)value);
                    return isVisable ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return value != null ? Visibility.Visible : Visibility.Collapsed;
                }
                
            }
            catch
            { return Visibility.Visible; }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
