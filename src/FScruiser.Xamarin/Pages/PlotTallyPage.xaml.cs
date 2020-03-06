using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FScruiser.Util;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotTallyPage : ContentPage
    {
        private bool _treeCellIsSelected;

        protected PlotTallyViewModel ViewModel => BindingContext as PlotTallyViewModel;

        public PlotTallyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is PlotTallyViewModel vm)
            {
                vm.TreeAdded += TallyFeed_CollectionChanged;
                TallyFeed_CollectionChanged(null, null);//Scroll to the bottom of the tally feed when page appears
            }

            
            MessagingCenter.Subscribe<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED, _plotTreeViewCell_IsSelectedChanged);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is PlotTallyViewModel vm)
            {
                vm.TreeAdded -= TallyFeed_CollectionChanged;
            }

            MessagingCenter.Unsubscribe<object, string>(this, Messages.EDIT_TREE_CLICKED);
            MessagingCenter.Unsubscribe<object, string>(this, Messages.DELETE_TREE_CLICKED);
            MessagingCenter.Unsubscribe<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED);
        }

        private void TallyFeed_CollectionChanged(object sender, EventArgs e)
        {
            if (_treeCellIsSelected) { return; } //dont scroll down it tree entry is in edit mode

            var lastItem = _tallyFeedListView.ItemsSource.OrEmpty().OfType<object>().LastOrDefault();
            if (lastItem != null)
            {
                _tallyFeedListView.ScrollTo(lastItem, ScrollToPosition.End, false);
            }
        }

        public void TallyFeedListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            var selectedItem = (TreeStub_Plot)eventArgs.SelectedItem;

            if (selectedItem != null)
            {
                var viewModel = ViewModel;
                //viewModel.ShowTree(selectedItem.Tree);
            }

            var view = (ListView)sender;
            //view.SelectedItem = null;//disable selection so that selection acts as a click
        }

        private void _plotTreeViewCell_IsSelectedChanged(object sender, bool isSelected)
        {
            _treeCellIsSelected = isSelected;
        }

        private void _stratumFilterButton_Clicked(object sender, EventArgs e)
        {
            if (sender != null && sender is View view
                && view != null   && view.BindingContext is string stratumFilter
                && BindingContext is PlotTallyViewModel viewModel)
            {
                viewModel.StratumFilter = stratumFilter;
            }
        }
    }
}