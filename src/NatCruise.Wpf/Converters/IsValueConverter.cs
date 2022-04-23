using CruiseDAL.V3.Sync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace NatCruise.Wpf.Converters
{
    [ContentProperty(nameof(Values))]
    public class IsValueConverter : IValueConverter
    {
        public object Value { get; set; }

        public List<object> Values { get; set; } = new List<object>();

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = Values;
            if (values != null && values.Count > 0)
            {
                var containValue = values.Contains(value);
                return containValue ^ Invert;
            }

            var areEqual = value.Equals(Value);
            return areEqual ^ Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ContentProperty(nameof(Values))]
    public class IsResolutionTypeValueConverter : IValueConverter
    {
        public object Value { get; set; }

        public List<ConflictResolutionType> Values { get; set; } = new List<ConflictResolutionType>();

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = Values;
            if (values != null && values.Count > 0)
            {
                var containValue = values.Contains((ConflictResolutionType)value);
                return containValue ^ Invert;
            }

            var areEqual = value.Equals(Value);
            return areEqual ^ Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //[ContentProperty(nameof(Values))]
    //public class IsValueConverterGeneric<TValue> : IValueConverter where TValue : class
    //{
    //    public TValue Value { get; set; }

    //    public List<TValue> Values { get; set; } = new List<TValue>();

    //    public bool Invert { get; set; }

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var values = Values;
    //        if (values != null && values.Count > 0)
    //        {
    //            var containValue = values.Contains((TValue)value);
    //            return containValue ^ Invert;
    //        }

    //        var areEqual = value.Equals(Value);
    //        return areEqual ^ Invert;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}