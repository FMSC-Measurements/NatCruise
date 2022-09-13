using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for TreeDefaultValueListView.xaml
    /// </summary>
    public partial class TreeDefaultValueListView : UserControl
    {
        static readonly string[] DEFINED_COLUMNS = new[]
            {
                nameof(TreeDefaultValue.SpeciesCode),
                nameof(TreeDefaultValue.PrimaryProduct),
                nameof(TreeDefaultValue.CreatedBy),
            };

        Style NumericCellStyle { get; }

        public TreeDefaultValueListView()
        {
            InitializeComponent();

            var baseTBstyle = TryFindResource("MahApps.Styles.TextBox") as Style;
            var style = new Style(typeof(TextBox), baseTBstyle);
            style.Setters.Add(new Setter(MahApps.Metro.Controls.TextBoxHelper.WatermarkProperty, "0"));

            NumericCellStyle = style;
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
