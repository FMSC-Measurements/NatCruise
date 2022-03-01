using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class IsEmptyConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as IEnumerable;

            var isEmpty = v == null || !v.GetEnumerator().MoveNext();
            return isEmpty ^ Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}