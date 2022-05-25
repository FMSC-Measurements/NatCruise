using CruiseDAL.Schema;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace NatCruise.Wpf.Converters
{
    public class LogFieldsToDataGridColumnsConverter : IValueConverter
    {
        public const string DBTYPE_REAL = TreeFieldTableDefinition.DBTYPE_REAL;
        public const string DBTYPE_TEXT = TreeFieldTableDefinition.DBTYPE_TEXT;
        public const string DBTYPE_BOOLEAN = TreeFieldTableDefinition.DBTYPE_BOOLEAN;
        public const string DBTYPE_INTEGER = TreeFieldTableDefinition.DBTYPE_INTEGER;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logFields = value as IEnumerable<LogField>;
            if (logFields == null) { return null; }

            return logFields.Select(x =>
            {
                switch (x.DbType)
                {
                    case DBTYPE_BOOLEAN:
                        return (DataGridColumn)new DataGridCheckBoxColumn
                        {
                            Header = x.Heading ?? x.DefaultHeading ?? x.Field,
                            Binding = new Binding(x.Field),
                        };

                    case DBTYPE_REAL:
                    case DBTYPE_INTEGER:
                    case DBTYPE_TEXT:
                    default:
                        return (DataGridColumn)new DataGridTextColumn
                        {
                            Header = x.Heading ?? x.DefaultHeading ?? x.Field,
                            Binding = new Binding(x.Field),
                        };
                }
            }).ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}