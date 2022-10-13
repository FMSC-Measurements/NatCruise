using NatCruise.Design.Models;
using NatCruise.Design.ViewModels;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumListPage.xaml
    /// </summary>
    public partial class StratumListView : UserControl
    {
        public StratumListView()
        {
            InitializeComponent();
        }

        

        private void _stratumTemplateCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var template = e.AddedItems[0] as StratumTemplate;
                if (template != null)
                {
                    if(!string.IsNullOrWhiteSpace(template.StratumCode))
                    {
                        _stratumCodeTextBox.Text = template.StratumCode;
                    }
                    _stratumCodeTextBox.Focus();
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as StratumListViewModel;
            if(oldvm != null)
            {
                oldvm.StratumAdded -= Vm_StratumAdded;
            }
            var vm = e.NewValue as StratumListViewModel;
            if(vm != null)
            {
                vm.StratumAdded += Vm_StratumAdded;
            }
        }

        private void Vm_StratumAdded(object sender, System.EventArgs e)
        {
            _stratumCodeTextBox.Clear();
            _stratumTemplateCombobox.ClearValue(ComboBox.TextProperty);
        }
    }
}