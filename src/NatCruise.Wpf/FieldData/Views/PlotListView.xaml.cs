﻿using System;
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

namespace NatCruise.Wpf.FieldData.Views
{
    /// <summary>
    /// Interaction logic for PlotListView.xaml
    /// </summary>
    public partial class PlotListView : UserControl
    {
        public PlotListView()
        {
            InitializeComponent();
        }

        private void SelectedPlotChanged(object sender, SelectionChangedEventArgs e)
        {
            var deselectedItems = e.RemovedItems;
            if (deselectedItems.Count == 0)
            {
                _plotEditExpander.IsExpanded = true;
            }
        }
    }
}
