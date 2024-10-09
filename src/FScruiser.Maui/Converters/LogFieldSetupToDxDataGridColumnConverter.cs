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
        public static LogFieldSetupToDxDataGridColumnConverter Instance { get; } = new LogFieldSetupToDxDataGridColumnConverter();

        public bool CreateReadOnly { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var logFields = (IEnumerable<LogFieldSetup>)value!;

            var columns = new List<GridColumn>();
            if (logFields == null) { return columns; }

            columns.Add(new NumberColumn { Caption = "Log #", FieldName = "LogNumber", IsReadOnly = CreateReadOnly });

            columns.AddRange(logFields.Select(x =>
                  (GridColumn)(x.Field switch
                {
                    nameof(Log.ExportGrade) => new ComboBoxColumn
                    {
                        Caption = x.Heading,
                        FieldName = x.Field,
                        IsReadOnly = CreateReadOnly,
                        ItemsSource = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" },
                    },
                    nameof(Log.Grade) => new ComboBoxColumn
                    {
                        Caption = x.Heading,
                        FieldName = x.Field,
                        IsReadOnly = CreateReadOnly,
                        ItemsSource = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" },
                    },
                    _ => new NumberColumn
                    {
                        Caption = x.Heading,
                        FieldName = x.Field,
                        IsReadOnly = CreateReadOnly,
                    }
                })));

            foreach (var col in columns)
            {
                col.MinWidth = Math.Max(col.MinWidth, 100);
            }

            return columns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
