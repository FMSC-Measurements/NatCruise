using NatCruise.Design.Models;
using NatCruise.Design.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumTemplateFieldsView.xaml
    /// </summary>
    public partial class StratumTemplateFieldsView : UserControl
    {
        public StratumTemplateFieldsView()
        {
            InitializeComponent();

            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratumTemplate = (StratumTemplate)context.Value;
            (DataContext as StratumTemplateFieldsViewModel).StratumTemplate = selectedStratumTemplate;
        }
    }
}