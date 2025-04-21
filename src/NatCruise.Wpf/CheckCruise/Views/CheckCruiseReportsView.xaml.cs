using NatCruise.Wpf.CheckCruise.ViewModels;
using System.Windows.Controls;

namespace NatCruise.Wpf.CheckCruise.Views
{
    /// <summary>
    /// Interaction logic for CheckCruiseReportsView.xaml
    /// </summary>
    public partial class CheckCruiseReportsView : UserControl
    {
        protected CheckCruiseReportsView()
        {
            InitializeComponent();
        }

        public CheckCruiseReportsView(CheckCruiseReportsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}