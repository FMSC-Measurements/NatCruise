using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using FScruiser.XF.ViewModels;
using NatCruise.MVVM.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotTallyView : InitializableContentPage
    {
        protected PlotTallyViewModel ViewModel => BindingContext as PlotTallyViewModel;

        public PlotTallyView()
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

        private void _stratumFilterButton_Clicked(object sender, EventArgs e)
        {
            if (sender != null && sender is View view
                && view != null && view.BindingContext is string stratumFilter
                && BindingContext is PlotTallyViewModel viewModel)
            {
                viewModel.StratumFilter = stratumFilter;
            }
        }

        private void _treeCellTappedGesture_Tapped(object sender, EventArgs e)
        {
            _treeEditPanel.IsVisible = true;
        }

        private void _hideTreeEditPanelButton_Clicked(object sender, EventArgs e)
        {
            _treeEditPanel.IsVisible = false;
        }

        private void openDeleteButton_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
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