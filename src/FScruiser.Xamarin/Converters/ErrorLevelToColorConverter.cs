using System;
using System.Globalization;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class ErrorLevelToColorConverter : IValueConverter
    {
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Default { get; set; } = Color.White;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Error { get; set; } = Color.Red;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Warning { get; set; } = Color.Gold;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (string)value;
            if (level == null) { return Color.White; }
            switch (level.ToUpper())
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