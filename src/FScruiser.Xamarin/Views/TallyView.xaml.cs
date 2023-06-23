using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using FScruiser.XF.ViewModels;
using NatCruise.Cruise.Services;
using NatCruise.MVVM.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyView : InitializableContentPage
    {
        public TallyView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var vm = _treeEditPanel.BindingContext as TreeEditViewModel;
            if (vm != null)
            {
                vm.Load();
            }
        }

        private void _tallyFeedListView_Focused(object sender, FocusEventArgs e)
        {
            _treeEditPanel.IsVisible = false;
        }

        private void _hideTreeEditPanelButton_Clicked(object sender, EventArgs e)
        {
            _treeEditPanel.IsVisible = false;
        }

        private void _treeCellTappedGesture_Tapped(object sender, EventArgs e)
        {
            _treeEditPanel.IsVisible = true;
        }

        private void openUntallyButton_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }

        private void _tallyFeedListView_Loaded(object sender, EventArgs e)
        {
            // scroll to end of tally feed when view loads

            var itemCount = _tallyFeedListView.ItemsSource?.OfType<object>()?.Count() ?? 0;
            if (itemCount > 2)
            {
                _tallyFeedListView.ScrollTo(itemCount - 1, position: ScrollToPosition.End, animate: false);
            }
        }

        private void _treeEditPanel_BindingContextChanged(object sender, EventArgs e)
        {
            var bindingContext = _treeEditPanel.BindingContext;
            if (bindingContext == null && _treeEditPanel.IsVisible)
            {
                _treeEditPanel.IsVisible = false;
            }
        }
    }
}