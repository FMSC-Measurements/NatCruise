using FScruiser.Util;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTreeTallyPage : ContentPage
    {
        private bool _treeCellIsSelected;

        protected UnitTreeTallyViewModel ViewModel => (UnitTreeTallyViewModel)BindingContext;

        public UnitTreeTallyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is UnitTreeTallyViewModel vm)
            {
                //vm.InitAsync().ConfigureAwait(true);
                vm.TallyEntryAdded += TallyFeed_CollectionChanged;
                TallyFeed_CollectionChanged(null, null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is UnitTreeTallyViewModel vm)
            {
                vm.TallyEntryAdded -= TallyFeed_CollectionChanged;
            }
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
    }
}