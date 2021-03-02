using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class ValueOrDefaultConverter<T> : IValueConverter
    {
        public static ValueOrDefaultConverter<T> Instance { get; } = new ValueOrDefaultConverter<T>();

        public T DefaultValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value ?? DefaultValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (((T)value).Equals(DefaultValue)) ? (object)null : value;
    }
}
