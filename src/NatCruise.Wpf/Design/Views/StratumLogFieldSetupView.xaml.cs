using NatCruise.Design.ViewModels;
using NatCruise.Models;
using NatCruise.MVVM.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumLogFieldSetupView.xaml
    /// </summary>
    public partial class StratumLogFieldSetupView : UserControl
    {
        public StratumLogFieldSetupView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratum = (Stratum)context.Value;
            (DataContext as StratumLogFieldSetupViewModel).Stratum = selectedStratum;
        }
    }
}