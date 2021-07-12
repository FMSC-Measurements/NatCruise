using CruiseDAL.Schema;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class CruiseMethodToVisableConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public CruiseMethodType MethodMap { get; set; }

        private CruiseMethodType StringToCruiseMethodType(string method)
        {
            return method switch
            {
                CruiseMethods.H_PCT => CruiseMethodType.H_PCT,
                CruiseMethods.STR => CruiseMethodType.STR,
                CruiseMethods.S3P => CruiseMethodType.S3P,
                CruiseMethods.THREEP => CruiseMethodType.ThreeP,
                CruiseMethods.FIX => CruiseMethodType.FIX,
                CruiseMethods.F3P => CruiseMethodType.F3P,
                CruiseMethods.FCM => CruiseMethodType.FCM,
                CruiseMethods.PCM => CruiseMethodType.PCM,
                CruiseMethods.PNT => CruiseMethodType.PNT,
                CruiseMethods.P3P => CruiseMethodType.P3P,
                CruiseMethods.THREEPPNT => CruiseMethodType.ThreePPNT,
                CruiseMethods.FIXCNT => CruiseMethodType.FIXCNT,
                _ => CruiseMethodType.None,
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var method = (string)value;
            var methodType = StringToCruiseMethodType(method);

            var boolValue = (methodType & MethodMap) != 0;
            return (boolValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}