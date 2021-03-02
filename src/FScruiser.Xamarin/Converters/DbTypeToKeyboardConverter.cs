using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class DbTypeToKeyboardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dbType = (string)value;
            switch(dbType.ToUpper())
            {
                case "REAL":
                case "INT":
                case "INTEGER":
                    { return Keyboard.Numeric; }
                case "TEXT":
                    { return Keyboard.Text; }
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
