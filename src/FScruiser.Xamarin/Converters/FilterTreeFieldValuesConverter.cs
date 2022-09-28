using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.Converters
{
    public class FilterTreeFieldValuesConverter : IValueConverter
    {
        private readonly string[] FILTERED_FIELDS = new[] { nameof(TreeEx.LiveDead), nameof(TreeEx.Initials), nameof(TreeEx.Remarks) };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var treeFields = (IEnumerable<TreeFieldValue>)value;

            if(value == null) return null;

            return treeFields
                .Where(x => !FILTERED_FIELDS.Contains(x.Field, StringComparer.OrdinalIgnoreCase))
                .ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}