﻿using NatCruise.Design.Models;
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
        public TreeDefaultValueListView()
        {
            InitializeComponent();
        }

        private void _tdvDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel =
            new[]
            {
                nameof(TreeDefaultValue.SpeciesCode),
                nameof(TreeDefaultValue.PrimaryProduct),
                nameof(TreeDefaultValue.CreatedBy),
            }.Contains(e.PropertyName);
        }
    }
}
