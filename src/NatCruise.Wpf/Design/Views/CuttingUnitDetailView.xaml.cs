using NatCruise.Design.Models;
using NatCruise.Design.ViewModels;
using Prism.Common;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
