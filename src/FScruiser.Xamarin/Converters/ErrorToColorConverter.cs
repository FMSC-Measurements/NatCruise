using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class ErrorToColorConverter : IValueConverter
    {
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Default { get; set; } = Color.White;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Error { get; set; } = Color.Red;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Warning { get; set; } = Color.Gold;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color SuppressedColor { get; set; } = Color.LightGray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var error = (Error_Base)value;
            if (error == null) { return Color.White; }

            if(error.IsResolved) { return SuppressedColor; }

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
