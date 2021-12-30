using NatCruise.Design.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class HasKeyToColorConverter : IValueConverter
    {
        public Color HasKeyColor { get; set; }
        public Color DefaultColor { get; set; } = Color.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dict = (Dictionary<CuttingUnit, IEnumerable<string>>)value;
            var key = parameter as CuttingUnit;

            if (dict.ContainsKey(key))
            {
                var dValue = dict[key];
                if (dValue.Any())
                {
                    return HasKeyColor;
                }
                else
                { return DefaultColor; }
            }
            else
            { return DefaultColor; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}