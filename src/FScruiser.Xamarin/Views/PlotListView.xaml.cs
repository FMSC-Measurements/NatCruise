using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FScruiser.XF.Util;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotListView : ContentPage
    {
        protected PlotListViewModel ViewModel => BindingContext as PlotListViewModel;

        public PlotListView()
        {
            InitializeComponent();

            _goToEndButton.Clicked += _goToEndButton_Clicked;
            _goToStartButton.Clicked += _goToStartButton_Clicked;
        }

        private void _goToEndButton_Clicked(object sender, EventArgs e)
        {
            ScrollLast();
        }

        private void ScrollLast()
        {
            var itemSource = _plotListView.ItemsSource;
            if (itemSource == null) { return; }
            var numItems = itemSource.Cast<object>().Count();
            if(numItems < 1) { return; }

            _plotListView.ScrollTo(numItems - 1, position: ScrollToPosition.End, animate: true);
        }

        private void _goToStartButton_Clicked(object sender, EventArgs e)
        {
            ScrollFirst();
        }

        private void ScrollFirst()
        {
            var itemSource = _plotListView.ItemsSource;
            if (itemSource == null) { return; }
            var numItems = itemSource.Cast<object>().Count();
            if (numItems < 1) { return; }

            _plotListView.ScrollTo(0, position: ScrollToPosition.Start, animate: false);
        }

        private void openDeletePlotButton_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }
    }
}