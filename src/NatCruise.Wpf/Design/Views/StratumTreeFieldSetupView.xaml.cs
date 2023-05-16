using NatCruise.Models;
using NatCruise.MVVM.ViewModels;
using Prism.Common;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumFieldSetupView.xaml
    /// </summary>
    public partial class StratumTreeFieldSetupView : UserControl
    {
        public StratumTreeFieldSetupView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratum = (Stratum)context.Value;
            (DataContext as StratumTreeFieldSetupViewModel).Stratum = selectedStratum;
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as StratumTreeFieldSetupViewModel;
            if (oldvm != null)
            {
                oldvm.TreeFieldAdded -= HandleTreeFieldAdded;
            }
            var vm = e.NewValue as StratumTreeFieldSetupViewModel;
            if (vm != null)
            {
                vm.TreeFieldAdded += HandleTreeFieldAdded;
            }
        }

        private void HandleTreeFieldAdded(object sender, EventArgs e)
        {
            _treeFieldCombobox.ClearValue(ComboBox.SelectedItemProperty);
        }
    }
}