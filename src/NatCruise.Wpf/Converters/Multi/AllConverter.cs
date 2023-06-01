using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters.Multi
{
    public class AllConverter : IMultiValueConverter
    {
        public bool Invert { get; set; }
        public bool DefaultValue { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = values.Select(x => (x == null) ? DefaultValue : (bool)x).All(x => x);
            result = result ^ Invert;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
