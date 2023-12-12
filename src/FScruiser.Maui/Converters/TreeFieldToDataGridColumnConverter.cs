using Maui.DataGrid;
using NatCruise.Models;
using NatCruise.Util;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FScruiser.Maui.Converters;

public class TreeFieldToDataGridColumnConverter : IValueConverter
{
    public bool IncludePlotNumber { get; set; } = false;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var treeFields = (IEnumerable<TreeField>)value!;

        var columns = new ObservableCollection<DataGridColumn>();
        if (treeFields == null) { return columns; }

        if (IncludePlotNumber)
        {
            columns.Add(new DataGridColumn { Title = "Plot #", PropertyName = "PlotNumber" });
        }
        columns.Add(new DataGridColumn { Title = "Tree #", PropertyName = "TreeNumber" });
        columns.Add(new DataGridColumn { Title = "Stratum", PropertyName = "StratumCode" });
        columns.Add(new DataGridColumn { Title = "Sample Group", PropertyName = "SampleGroupCode" });
        columns.Add(new DataGridColumn { Title = "Species", PropertyName = "SpeciesCode" });
        columns.Add(new DataGridColumn { Title = "L/D", PropertyName = "LiveDead" });
        columns.Add(new DataGridColumn { Title = "C/M", PropertyName = "CountOrMeasure" });

        columns.AddRange(treeFields.Select(x =>
                            new DataGridColumn
                            {
                                Title = x.Heading,
                                PropertyName = x.Field,
                            }));

        columns.Add(new DataGridColumn { Title = "KPI", PropertyName = "KPI" });

        columns.Add(new DataGridColumn { Title = "Errors", PropertyName = "ErrorCount" });
        columns.Add(new DataGridColumn { Title = "Warnings", PropertyName = "WarningCount" });

        return columns;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}