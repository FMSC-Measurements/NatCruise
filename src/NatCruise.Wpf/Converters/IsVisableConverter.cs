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
        public bool Invert { get; set; }

        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool isVisable;
                if(value is bool)
                {
                    isVisable = (bool)value;
                }
                else if(value is string sValue)
                {
                    isVisable = !string.IsNullOrEmpty(sValue);
                }
                else if(value is double dValue)
                {
                    isVisable = dValue > 0;
                }
                else if (value is float fValue)
                {
                    isVisable = fValue > 0;
                }
                else if ((value is int iValue))
                {
                    isVisable = iValue > 0;
                }
                else
                {
                    isVisable = value != null;
                }

                return isVisable ^ Invert ? Visibility.Visible : FalseValue;

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
