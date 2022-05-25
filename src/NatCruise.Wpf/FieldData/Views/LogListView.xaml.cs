using System.Windows.Controls;

namespace NatCruise.Wpf.FieldData.Views
{
    /// <summary>
    /// Interaction logic for LogListView.xaml
    /// </summary>
    public partial class LogListView : UserControl
    {
        public LogListView()
        {
            InitializeComponent();
        }

        private void OnLogSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deselectedItems = e.RemovedItems;
            if (deselectedItems.Count == 0)
            {
                _logEditExpander.IsExpanded = true;
            }
        }
    }
}