using NatCruise.Design.Models;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumFieldsView.xaml
    /// </summary>
    public partial class StratumFieldsView : UserControl
    {
        public StratumFieldsView()
        {
            InitializeComponent();
            //RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratum = (Stratum)context.Value;
            RegionManager.SetRegionContext(_stratumFieldsRegion, selectedStratum);
        }
    }
}