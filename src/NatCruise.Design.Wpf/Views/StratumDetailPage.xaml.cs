using NatCruise.Wpf.Models;
using NatCruise.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for StratumDetailPage.xaml
    /// </summary>
    public partial class StratumDetailPage : UserControl
    {
        public StratumDetailPage()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratum = (Stratum)context.Value;
            (DataContext as StratumDetailPageViewModel).Stratum = selectedStratum;
        }
    }
}