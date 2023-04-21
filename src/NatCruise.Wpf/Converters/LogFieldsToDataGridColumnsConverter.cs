using CruiseDAL.Schema;
using FastExpressionCompiler.LightExpression;
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

        public bool TreeNumberField { get; set; } = true;
        public bool CuttingUnitField { get; set; } = true;
        public bool PlotNumberField { get; set; } = true;
        public bool StratumField { get; set; } = true;

        public bool LogNumberField { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logFields = value as IEnumerable<LogFieldSetup>;
            if (logFields == null) { return null; }

            var columns = new List<DataGridColumn>();
            //if(CuttingUnitField)
            //{
            //    columns.Add(new DataGridTextColumn
            //    {
            //        Header = "Unit",
            //        Binding = new Binding(nameof(Log.CuttingUnitCode))
            //    });
            //}
            //if(PlotNumberField)
            //{
            //    columns.Add(new DataGridTextColumn
            //    {
            //        Header = "Plot",
            //        Binding = new Binding(nameof(Log.PlotNumber))
            //    });
            //}
            //if(StratumField)
            //{
            //    columns.Add(new DataGridTextColumn
            //    {
            //        Header = "Stratum",
            //        Binding = new Binding(nameof(Log.StratumCode))
            //    });
            //}
            //if(TreeNumberField)
            //{
            //    columns.Add(new DataGridTextColumn
            //    {
            //        Header = "Tree",
            //        Binding = new Binding(nameof(Log.TreeNumber))
            //    });
            //}
            if (LogNumberField)
            {
                columns.Add(new DataGridTextColumn
                {
                    Header = "Log",
                    Binding = new Binding(nameof(Log.LogNumber))
                });
            }

            columns.AddRange(logFields.Select(x =>
            {
                switch (x.DbType)
                {
                    case DBTYPE_BOOLEAN:
                        return (DataGridColumn)new DataGridCheckBoxColumn
                        {
                            Header = x.Heading,
                            Binding = new Binding(x.Field),
                        };

                    case DBTYPE_REAL:
                    case DBTYPE_INTEGER:
                    case DBTYPE_TEXT:
                    default:
                        return (DataGridColumn)new DataGridTextColumn
                        {
                            Header = x.Heading,
                            Binding = new Binding(x.Field),
                        };
                }
            }));

            return columns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}