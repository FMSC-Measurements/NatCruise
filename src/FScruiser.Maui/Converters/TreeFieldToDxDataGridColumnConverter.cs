using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Maui.DataGrid;
using NatCruise.Models;

namespace FScruiser.Maui.Converters
{
    public class TreeFieldToDxDataGridColumnConverter : IValueConverter
    {
        public bool IncludePlotNumber { get; set; } = false;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var treeFields = (IEnumerable<TreeField>)value!;

            var columns = new List<GridColumn>();
            if (treeFields == null) { return columns; }

            if (IncludePlotNumber)
            {
                columns.Add(new TextColumn { Caption = "Plot #", FieldName = "PlotNumber", IsReadOnly = true });
            }
            columns.Add(new TextColumn { Caption = "Tree #", FieldName = "TreeNumber", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "Stratum", FieldName = "StratumCode", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "Sample Group", FieldName = "SampleGroupCode", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "Species", FieldName = "SpeciesCode", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "L/D", FieldName = "LiveDead", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "C/M", FieldName = "CountOrMeasure", IsReadOnly = true });

            columns.AddRange(treeFields.Select(x =>
                                new TextColumn
                                {
                                    Caption = x.Heading,
                                    FieldName = x.Field,
                                    IsReadOnly = true,
                                }));

            columns.Add(new TextColumn { Caption = "KPI", FieldName = "KPI", IsReadOnly = true });

            columns.Add(new TextColumn { Caption = "Errors", FieldName = "ErrorCount", IsReadOnly = true });
            columns.Add(new TextColumn { Caption = "Warnings", FieldName = "WarningCount", IsReadOnly = true });

            foreach(var col in  columns)
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
