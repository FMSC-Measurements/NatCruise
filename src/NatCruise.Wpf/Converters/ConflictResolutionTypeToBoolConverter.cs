using CruiseDAL.V3.Sync;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class ConflictResolutionTypeToBoolConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public ConflictResolutionType Mask { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && Enum.TryParse<ConflictResolutionType>(str, out var convertVal))
            {
                return convertVal;
            }
            else if (value is ConflictResolutionType crt)
            {
                return (crt == Mask) ^ Invert;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = (bool)value;
            if (bValue ^ Invert)
            { return Mask; }
            else
            { return ConflictResolutionType.NotSet; }

            //throw new NotImplementedException();
        }
    }
}