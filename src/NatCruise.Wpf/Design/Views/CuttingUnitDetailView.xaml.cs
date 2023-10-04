using NatCruise.Design.ViewModels;
using NatCruise.Models;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for CuttingUnitEditPage.xaml
    /// </summary>
    public partial class CuttingUnitDetailView : UserControl
    {
        public CuttingUnitDetailView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedUnit = (CuttingUnit)context.Value;
            (DataContext as CuttingUnitDetailViewModel).CuttingUnit = selectedUnit;
        }
    }
}