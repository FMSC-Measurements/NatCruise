using FScruiser.Models;
using FScruiser.XF.Constants;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyEntryTreeViewCell : TallyEntryTreeViewCell_Base
    {
        protected override ScrollView TreeFieldViewContainer => _treeEditScrollView;

        protected override View DrawrView => _treeEditPanel;

        public TallyEntryTreeViewCell()
        {
            InitializeComponent();

            _editButton.Clicked += _editButton_Clicked;
            _untallyButton.Clicked += _untallyButton_Clicked;
        }

        private void _editButton_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is TallyEntry tallyEntry && tallyEntry != null)
            {
                EditTreeCommand?.Execute(tallyEntry.TreeID);
            }
        }

        protected void _untallyButton_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is TallyEntry tallyEntry && tallyEntry != null)
            {
                UntallyCommand?.Execute(tallyEntry.TallyLedgerID);
            }
        }

        private void _entry_Completed(object sender, EventArgs e)
        {
            if (sender != null && sender is View view)
            {
                var layout = (Grid)view.Parent;

                var indexOfChild = layout.Children.IndexOf(view);
                var nextChild = layout.Children.Skip(indexOfChild + 1).Where(x => x is Entry || x is Picker).FirstOrDefault();
                nextChild?.Focus();
            }
        }

        private void RefreshTree()
        {
            var tallyEntry = BindingContext as TallyEntry;
            if (tallyEntry?.TreeID != null)
            {
                TreeViewModel?.OnNavigatedTo(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, tallyEntry.TreeID } });
            }
        }
    }
}