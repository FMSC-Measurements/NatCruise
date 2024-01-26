using MahApps.Metro.Controls;
using Microsoft.Win32;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace NatCruise.Wpf.FieldData.Views
{
    /// <summary>
    /// Interaction logic for FieldDataView.xaml
    /// </summary>
    public partial class FieldDataView : UserControl
    {
        public FieldDataView()
        {
            InitializeComponent();
        }

        private void HandelExportButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedTab = _tabControl.SelectedContent as Control;
            var selectedDg = selectedTab.FindChild<DataGrid>();

            var fileDialog = new SaveFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "Comma Separated Values (*.csv)|*.csv",
                AddExtension = true,
                OverwritePrompt = true,
            };
            var result = fileDialog.ShowDialog();
            if(result == true)
            {
                var filePath = fileDialog.FileName;

                var data = SerializeDataGridAsCSV(selectedDg);
                File.WriteAllText(filePath, data);
            }
        }

        public string SerializeDataGridAsCSV(DataGrid dataGrid)
        {
            var sb = new StringBuilder();

            var cols = dataGrid.Columns.Where(x => x.Visibility == Visibility.Visible).ToArray();
            var colCount = cols.Length;
            var lastColIndex = colCount - 1;
            // write headers
            
            for( int i = 0; i < colCount; i++)
            {
                var col = cols[i];
                AppendFormatedCellValue(col.Header, sb);
                if(i == lastColIndex)
                {
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    sb.Append(',');
                }
                
            }

            // write rows
            var itemCount = dataGrid.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                var item = dataGrid.Items[i];

                for(int j = 0; j < colCount; j++)
                {
                    var col = cols[j];

                    var cellValue = col.OnCopyingCellClipboardContent(item);
                    AppendFormatedCellValue(cellValue, sb);
                    if (j == lastColIndex)
                    {
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(',');
                    }
                }

            }

            return sb.ToString();
        }

        private void AppendFormatedCellValue(object cellValue, StringBuilder sb )
        {
            if(cellValue == null )
            {
                sb.Append("");
                return;
            }

            var s = cellValue.ToString();
            var sw = new StringWriter(sb, CultureInfo.CurrentCulture);

            bool needsEscape = false;
            foreach(var ch in s)
            {
                if(ch == '"' || ch == ',')
                {
                    needsEscape = true;
                    break;
                }
            }

            if(needsEscape)
            {
                sw.Write('"');
            }

            foreach (var ch in s)
            {
                if (ch == '"')
                { sw.Write("\"\""); }
                if (ch == '\r' || ch == '\n')
                { continue; }
                else
                { sw.Write(ch); }
            }

            if (needsEscape)
            {
                sw.Write('"');
            }
        }

        private void _tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection Changed is a Routed Event. meaning that all child controls that
            // make use of the SelectionChanged event i.e. dataGrids, ListBoxes, ComboBoxes
            // will also raise selection changed events on our tabControl.
            // so we need to ignore all those extra events not coming directly from our tabControl

            if(e.Source != sender) { return; }
            if (sender is TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;
                if (selectedTab != null && selectedTab.Content is FrameworkElement element)
                {
                    var viewModel = element.DataContext as ViewModelBase;
                    viewModel.Load();
                }
            }
        }
    }
}
