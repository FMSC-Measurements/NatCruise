using NatCruise.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NatCruise.Wpf.Converters
{
    public class ErrorToBrushConverter : IValueConverter
    {
        [TypeConverter(typeof(BrushConverter))]
        public Brush Default { get; set; } = Brushes.White;

        [TypeConverter(typeof(BrushConverter))]
        public Brush Error { get; set; } = Brushes.Red;

        [TypeConverter(typeof(BrushConverter))]
        public Brush Warning { get; set; } = Brushes.Gold;

        [TypeConverter(typeof(BrushConverter))]
        public Brush SuppressedColor { get; set; } = Brushes.LightGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var error = (ErrorBase)value;
            if (error == null) { return Default; }

            if (error.IsResolved) { return SuppressedColor; }

            switch (error.Level.ToUpper())
            {
                case "E": return Error;
                case "W": return Warning;
                default: return Default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}