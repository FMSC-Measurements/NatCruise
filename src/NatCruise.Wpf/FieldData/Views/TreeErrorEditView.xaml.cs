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

namespace NatCruise.Wpf.FieldData.Views
{
    /// <summary>
    /// Interaction logic for TreeErrorEditView.xaml
    /// </summary>
    public partial class TreeErrorEditView : UserControl
    {
        public TreeErrorEditView()
        {
            InitializeComponent();
        }

        public EventHandler OnCancelButtonClicked;

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            OnCancelButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
