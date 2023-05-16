using NatCruise.Design.ViewModels;
using NatCruise.Models;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumTemplateLogFieldSetupView.xaml
    /// </summary>
    public partial class StratumTemplateLogFieldSetupView : UserControl
    {
        public StratumTemplateLogFieldSetupView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratumTemplate = (StratumTemplate)context.Value;
            (DataContext as StratumTemplateLogFieldSetupViewModel).StratumTemplate = selectedStratumTemplate;
        }
    }
}