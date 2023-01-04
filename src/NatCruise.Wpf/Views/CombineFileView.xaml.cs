using NatCruise.Wpf.ViewModels;
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

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CombineFileView.xaml
    /// </summary>
    public partial class CombineFileView : UserControl
    {
        public CombineFileView()
        {
            InitializeComponent();

            DataContextChanged += CombineFileView_DataContextChanged;
        }

        private void CombineFileView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is CombineFileViewModel oldVm)
            {
                oldVm.ConflictsDetected -= ViewModel_ConflictDetected;
            }
            if(e.NewValue is CombineFileViewModel newVm)
            {
                newVm.ConflictsDetected += ViewModel_ConflictDetected;
            }
        }

        private void ViewModel_ConflictDetected(object sender, EventArgs e)
        {
            _currentFileDetailsPanel.IsExpanded = true;
        }
    }
}
