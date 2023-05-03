using NatCruise.Design.ViewModels;
using NatCruise.Models;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for TreeAuditSelectorsView.xaml
    /// </summary>
    public partial class TreeAuditSelectorsView : UserControl
    {
        public TreeAuditSelectorsView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var treeAuditRule = (TreeAuditRule)context.Value;
            (DataContext as TreeAuditSelectorsViewModel).TreeAuditRule = treeAuditRule;
        }
    }
}