using NatCruise.Design.ViewModels;
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
    /// Interaction logic for CuttingUnitListPage.xaml
    /// </summary>
    public partial class CuttingUnitListView : UserControl
    {
        public CuttingUnitListView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as CuttingUnitListViewModel;
            if (oldvm != null)
            {
                oldvm.CuttingUnitAdded -= CuttingUnitAdded;
            }
            var newvm = e.NewValue as CuttingUnitListViewModel;
            if(newvm != null)
            {
                newvm.CuttingUnitAdded += CuttingUnitAdded;
            }

        }

        private void CuttingUnitAdded(object sender, EventArgs e)
        {
            _unitCodeTextBox.Clear();
        }
    }
}
