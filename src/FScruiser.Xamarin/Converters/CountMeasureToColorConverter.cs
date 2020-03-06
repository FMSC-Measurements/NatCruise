using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    

    public class CountMeasureToColorConverter : IValueConverter
    {
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Measure { get; set; }

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Count { get; set; } = Color.White;

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Insurance { get; set; }

        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Default { get; set; } = Color.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var countMeasure = (string)value;
            if (countMeasure == null) { return Default; }
            switch (countMeasure.ToUpper())
            {
                case "M": return Measure;
                case "C": return Count;
                case "I": return Insurance;
                default: return Default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
