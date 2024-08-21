using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Maui.DataGrid;

namespace FScruiser.Maui.Controls
{
    public static class DXDatagridHelper
    {
        public static readonly BindableProperty ColumnsProperty = BindableProperty.CreateAttached(
            "Columns",
            typeof(IList<GridColumn>),
            typeof(DXDatagridHelper),
            default,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnColumnsChanged,
            defaultValueCreator: (x) => new List<GridColumn>());

        public static IList<GridColumn> GetColumns(BindableObject target)
        {
            return (IList<GridColumn>)target.GetValue(ColumnsProperty);
        }

        public static void SetColumns(BindableObject source, IList<GridColumn> columns)
        {
            source.SetValue(ColumnsProperty, columns);
        }

        private static void OnColumnsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var dataGrid = (DevExpress.Maui.DataGrid.DataGridView)bindable;
            var newList = newValue as IList<GridColumn>;

            dataGrid.Columns.Clear();
            dataGrid.Columns.AddRange(newList);
        }
    }
}
