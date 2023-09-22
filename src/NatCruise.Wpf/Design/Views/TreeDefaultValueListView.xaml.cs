using NatCruise.Design.Models;
using System;
using System.Linq;
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
        }
    }
}