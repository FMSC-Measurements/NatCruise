using MahApps.Metro.Controls;
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

            var cellNullValue = propName switch
            {
                nameof(TreeDefaultValue.MerchHeightType) => "F",
                _ => "0",
            };

            //var tbEditingStyle = new Style(typeof(TextBox), basedOn: DataGridTextColumn.DefaultEditingElementStyle)
            //{
            //    Setters =
            //    {
            //        new Setter(TextBoxHelper.WatermarkProperty, colDefatutValue),
            //    }
            //};

            //var tbStyle = new Style(typeof(TextBlock), basedOn: DataGridTextColumn.DefaultElementStyle)
            //{
            //    Setters =
            //    {
            //        new Setter(TextBoxHelper.WatermarkProperty, colDefatutValue),
            //    }
            //};

            var col = new DataGridTextColumn()
            {
                Header = propName,
                // although generally the default behavior of datagrid binding behaves the same as LostFocus
                // this is needed to cause the binding to update with the window is closed
                // this brings binding control back to the cell edit control rather than the datagrid
                Binding = new System.Windows.Data.Binding(propName)
                {
                    TargetNullValue = cellNullValue,
                    
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.LostFocus,
                },
                //EditingElementStyle = tbEditingStyle,
                //ElementStyle = tbStyle,
            };

            e.Column = col;

            
        }
    }
}