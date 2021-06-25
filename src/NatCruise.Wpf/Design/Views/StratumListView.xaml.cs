using NatCruise.Design.Models;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumListPage.xaml
    /// </summary>
    public partial class StratumListView : UserControl
    {
        public StratumListView()
        {
            InitializeComponent();
        }

        private void _stratumTemplateCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var template = e.AddedItems[0] as StratumTemplate;
                if (template != null)
                {
                    if(!string.IsNullOrWhiteSpace(template.StratumCode))
                    {
                        _stratumCodeTextBox.Text = template.StratumCode;
                    }
                    _stratumCodeTextBox.Focus();
                }
            }
        }
    }
}