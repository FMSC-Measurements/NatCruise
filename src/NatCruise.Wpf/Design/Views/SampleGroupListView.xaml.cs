﻿using NatCruise.Design.ViewModels;
using NatCruise.Models;
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

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for SampleGroupListPage.xaml
    /// </summary>
    public partial class SampleGroupListView : UserControl
    {
        public SampleGroupListView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratum = (Stratum)context.Value;
            (DataContext as SampleGroupListViewModel).Stratum = selectedStratum;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as SampleGroupListViewModel;
            if(oldvm != null)
            {
                oldvm.SampleGroupAdded -= OnSampleGroupAdded;
            }
            var newvm = e.NewValue as SampleGroupListViewModel;
            if (newvm != null)
            {
                newvm.SampleGroupAdded += OnSampleGroupAdded;
            }
            
        }

        private void OnSampleGroupAdded(object sender, EventArgs e)
        {
            _sampleGroupCodeTextBox.Clear();

            _sampleGroupDetailsRegion.SelectedIndex = 0;
        }
    }
}
