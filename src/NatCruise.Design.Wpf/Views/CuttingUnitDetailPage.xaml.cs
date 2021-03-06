﻿using NatCruise.Wpf.Models;
using NatCruise.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CuttingUnitEditPage.xaml
    /// </summary>
    public partial class CuttingUnitDetailPage : UserControl
    {
        public CuttingUnitDetailPage()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedUnit = (CuttingUnit)context.Value;
            (DataContext as CuttingUnitDetailPageViewModel).CuttingUnit = selectedUnit;
        }
    }
}
