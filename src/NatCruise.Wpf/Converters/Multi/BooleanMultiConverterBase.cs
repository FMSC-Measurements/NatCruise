using System;
using System.Globalization;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters.Multi
{
    public abstract class BooleanMultiConverterBase : IMultiValueConverter
    {
        public bool Invert { get; set; }
        public bool DefaultValue { get; set; }
        public object TrueObject { get; set; }
        public object FalseObject { get; set; }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        protected object GetValueForResult(bool result)
        {
            result = result ^ Invert;

            if (result)
            {
                return TrueObject ?? true;
            }
            else
            {
                return FalseObject ?? false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}