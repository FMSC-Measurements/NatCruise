using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NatCruise.Wpf.Controls
{
    public static class DataGridHelper
    {
        public static object GetColumnsBinding(DependencyObject obj)
        {
            return (object)obj.GetValue(ColumnsBindingProperty);
        }

        public static void SetColumnsBinding(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnsBindingProperty, value);
        }

        public static readonly DependencyProperty ColumnsBindingProperty = DependencyProperty.RegisterAttached(
                "ColumnsBinding",
                typeof(object),
                typeof(DataGridHelper),
                new UIPropertyMetadata(propertyChangedCallback: ColumnsChanged));

        private static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dg = d as DataGrid;
            var columns = e.NewValue as IEnumerable<DataGridColumn>;

            if (dg != null && columns != null)
            {
                var dgCols = dg.Columns;
                dgCols.Clear();
                foreach (var col in columns)
                {
                    dgCols.Add(col);
                }
            }
        }
    }
}