using FScruiser.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotTreeViewCell : TallyEntryTreeViewCell_Base
    {
        protected override ScrollView TreeFieldViewContainer => _treeEditScrollView;

        protected override View DrawrView => _treeEditPanel;

        public PlotTreeViewCell()
        {
            InitializeComponent();

            _editButton.Clicked += _editButton_Clicked;
            _deleteButton.Clicked += _deleteButton_Clicked;
        }

        private void _editButton_Clicked(object sender, EventArgs e)
        {
            var tree = BindingContext as TreeStub_Plot;

            EditTreeCommand?.Execute(tree.TreeID);
        }

        protected void _deleteButton_Clicked(object sender, EventArgs e)
        {
            var tree = BindingContext as TreeStub_Plot;

            UntallyCommand?.Execute(tree.TreeID);
        }
    }
}