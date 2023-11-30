using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
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

namespace NatCruise.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for DataGridMultiSelectColumn.xaml
    /// </summary>
    public partial class DataGridMultiSelectColumn : DataGridTemplateColumn
    {
        private bool _isChangingSelection;

        public DataGridMultiSelectColumn()
        {
            InitializeComponent();


        }

        

        private void Cell_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Cell_UnChecked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            // clicked event fires after checked state has been toggled
            // but we can use clicked event to detect if state has been
            // changed via click
            _isChangingSelection = true;
            try
            {
                var checkbox = (CheckBox)sender;
                var isChecked = checkbox.IsChecked ?? false;
                if (isChecked)
                {
                    base.DataGridOwner.SelectAll();

                }
                else
                {
                    base.DataGridOwner.UnselectAll();
                }
            }
            finally
            {
                _isChangingSelection= false;
            }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return base.GenerateElement(cell, dataItem);
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            //base.DataGridOwner.SelectAll();
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            //base.DataGridOwner.UnselectAll();
        }

        private void DataGrid_SelectionChnaged(object sender, SelectionChangedEventArgs e)
        {
            PART_headerSelectAllCheckbox.IsChecked = false;
        }

        private void PART_headerSelectAllCheckbox_Loaded(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var dataGrid = checkbox.GetVisualAncestor<DataGrid>();
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void PART_headerSelectAllCheckbox_Unloaded(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var dataGrid = checkbox.GetVisualAncestor<DataGrid>();
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if selection state of data grid changes we need to invalidate the
            // select all state of the header checkbox

            if (_isChangingSelection) { return; }
            PART_headerSelectAllCheckbox.IsChecked = false;
        }

    }
}
