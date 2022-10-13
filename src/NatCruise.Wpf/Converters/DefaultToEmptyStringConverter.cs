using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    // converter for displaying default values of value types as an empty string
    // this is primarily for displaying '' instead of 0 for numeric properties

    public class DefaultToEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;

            var valType = value.GetType();
            var defaultVal = Activator.CreateInstance(valType);
            if (value.Equals(defaultVal)) return DependencyProperty.UnsetValue;

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            

            // were expecting targetType to be a value type, nullable value type or string.
            // check if it is a nullable value type, by using GetUnderlyingType
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;


            var defaultVal = Activator.CreateInstance(targetType);

            // value should be a string since it is likely coming from a text box control
            var sVal = (string)value;
            if (String.IsNullOrEmpty(sVal)) return defaultVal.ToString();

            return value;
        }
    }
}