using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NatCruise.Wpf.Converters
{
    public class ValueToBrushConverter : IValueConverter
    {
        public object Value1 { get; set; }
        public object Value2 { get; set; }

        public Brush Value1Brush { get; set; }

        public Brush Value2Brush { get; set; }

        public Brush DefaultBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.Equals(Value1))
                { return Value1Brush; }
                if (value.Equals(Value2))
                { return Value2Brush; }
            }
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}