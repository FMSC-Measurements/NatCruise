using AutoBogus;
using CruiseDAL.V3.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace NatCruise.Wpf.Controls.Test
{
    /// <summary>
    /// Interaction logic for MultiSelectDataGridTest.xaml
    /// </summary>
    public partial class MultiSelectDataGridTest : Window
    {
        public MultiSelectDataGridTest()
        {
            var selectedItems = new ObservableCollection<CuttingUnit>();
            selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            MySelectedItems = selectedItems;

            InitializeComponent();

            var faker = new AutoFaker<CuttingUnit>();

            _datagrid.ItemsSource = faker.Generate(10);
        }

        private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public IList MySelectedItems
        { get; set; }

    }
}
