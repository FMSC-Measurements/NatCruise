using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace NatCruise.Wpf.FieldData.Views
{
    /// <summary>
    /// Interaction logic for TreeEditView.xaml
    /// </summary>
    public partial class TreeEditView : UserControl
    {
        public TreeEditView()
        {
            InitializeComponent();
            

            Loaded += TreeEditView_Loaded;
            Unloaded += TreeEditView_Unloaded
                ;
        }

        private void TreeEditView_Unloaded(object sender, RoutedEventArgs e)
        {
            DialogManager.DialogClosed -= DialogManager_DialogClosed;
        }

        private void TreeEditView_Loaded(object sender, RoutedEventArgs e)
        {
            DialogManager.DialogClosed += DialogManager_DialogClosed;
        }

        private void DialogManager_DialogClosed(object sender, DialogStateChangedEventArgs e)
        {
            var vm = DataContext as TreeEditViewModel;
            if (vm != null)
            {
                vm.RefreshErrorsAndWarnings();
            }
        }
    }
}
