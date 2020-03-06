using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotListPage : ContentPage
    {
        protected PlotListViewModel ViewModel => BindingContext as PlotListViewModel;

        public PlotListPage()
        {
            InitializeComponent();

            _goToEndButton.Clicked += _goToEndButton_Clicked;
            _goToStartButton.Clicked += _goToStartButton_Clicked;

            _plotListView.ItemSelected += _plotListView_ItemSelected;
        }

        private void _plotListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Plot plot && plot != null)
            {
                ViewModel?.ShowTallyPlot(plot);
            }
        }

        private void _goToEndButton_Clicked(object sender, EventArgs e)
        {
            ScrollLast();
        }

        private void ScrollLast()
        {
            var itemSource = _plotListView.ItemsSource;
            if (itemSource == null) { return; }
            var lastItem = itemSource.Cast<object>().LastOrDefault();
            if (lastItem == null) { return; }

            _plotListView.ScrollTo(lastItem, ScrollToPosition.End, false);
        }

        private void _goToStartButton_Clicked(object sender, EventArgs e)
        {
            ScrollFirst();
        }

        private void ScrollFirst()
        {
            var itemSource = _plotListView.ItemsSource;
            if (itemSource == null) { return; }
            var firstItem = itemSource.Cast<object>().FirstOrDefault();
            if (firstItem == null) { return; }

            _plotListView.ScrollTo(firstItem, ScrollToPosition.Start, false);
        }
    }
}