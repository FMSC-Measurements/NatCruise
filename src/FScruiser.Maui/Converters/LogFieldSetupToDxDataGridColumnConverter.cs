using DevExpress.Maui.DataGrid;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Converters
{
    public class LogFieldSetupToDxDataGridColumnConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var logFields = (IEnumerable<LogFieldSetup>)value!;

            var columns = new List<GridColumn>();
            if (logFields == null) { return columns; }

            columns.Add(new TextColumn { Caption = "Log #", FieldName = "LogNumber", IsReadOnly = true });

            columns.AddRange(logFields.Select(x =>
                                new TextColumn
                                {
                                    Caption = x.Heading,
                                    FieldName = x.Field,
                                    IsReadOnly = true,
                                }));

            foreach (var col in columns)
            {
                col.MinWidth = Math.Max(col.MinWidth, 100);
                col.IsReadOnly = true;
            }

            return columns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
