using NatCruise.Design.ViewModels;
using NatCruise.MVVM.ViewModels;
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
    /// Interaction logic for SpeciesListView.xaml
    /// </summary>
    public partial class SpeciesListView : UserControl
    {
        public SpeciesListView()
        {
            InitializeComponent();
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as SpeciesListViewModel;
            if (oldvm != null)
            {
                oldvm.SpeciesAdded -= HandleSpeciesAdded;
            }
            var newvm = e.NewValue as SpeciesListViewModel;
            if (newvm != null)
            {
                newvm.SpeciesAdded += HandleSpeciesAdded;
            }
        }

        private void HandleSpeciesAdded(object sender, EventArgs e)
        {
            _newSpeciesTextBox.Clear();
        }

        private void _speciesDetailsPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVM = e.OldValue as SpeciesDetailViewModel;
            if (oldVM != null)
            {
                oldVM.ContractSpeciesAdded -= SpDetailVM_ContractSpeciesAdded;
            }
            var newVM = e.NewValue as SpeciesDetailViewModel;
            if(newVM != null)
            {
                newVM.ContractSpeciesAdded += SpDetailVM_ContractSpeciesAdded;
            }


            void SpDetailVM_ContractSpeciesAdded(object sender, EventArgs e)
            {
                _ctrSpTextBox.Clear();
                _prodComboBox.SelectedIndex = 0;
            }

        }
    }
}
