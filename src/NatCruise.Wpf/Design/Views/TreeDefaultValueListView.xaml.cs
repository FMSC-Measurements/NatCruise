using NatCruise.Design.Models;
using NatCruise.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for TreeDefaultValueListView.xaml
    /// </summary>
    public partial class TreeDefaultValueListView : UserControl
    {
        private static readonly string[] DEFINED_COLUMNS = new[]
            {
                nameof(TreeDefaultValue.SpeciesCode),
                nameof(TreeDefaultValue.PrimaryProduct),
                nameof(TreeDefaultValue.CreatedBy),
            };

        public TreeDefaultValueListView()
        {
            InitializeComponent();
        }


        private void _tdvDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var propName = e.PropertyName;
            var isDefinedColumn = DEFINED_COLUMNS.Contains(propName);

            e.Cancel = isDefinedColumn;
            if (isDefinedColumn) { return; }

            var col = new DataGridTextColumn()
            {
                Header = propName,
                Binding = new System.Windows.Data.Binding(propName)
                { UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.LostFocus },
                // although generally the default behavior of datagrid binding is close to this
                // this is needed to cause the binding to update with the window is closed
                // this brings binding control back to the cell edit control rather than the datagrid
            };

            e.Column = col;

            
        }
    }
}