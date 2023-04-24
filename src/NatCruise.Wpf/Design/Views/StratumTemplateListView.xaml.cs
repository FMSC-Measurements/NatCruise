using NatCruise.Design.ViewModels;
using System;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumProfileListView.xaml
    /// </summary>
    public partial class StratumTemplateListView : UserControl
    {
        public StratumTemplateListView()
        {
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is StratumTemplateListViewModel oldVm)
            {
                oldVm.StratumTemplateAdded -= HandleTemplateAdded;
            }
            if(e.NewValue is StratumTemplateListViewModel newVm)
            {
                newVm.StratumTemplateAdded += HandleTemplateAdded;
            }

            void HandleTemplateAdded(object sender, EventArgs e)
            {
                _stratumTemplateRegion.SelectedIndex = 0;
            }
        }
    }
}