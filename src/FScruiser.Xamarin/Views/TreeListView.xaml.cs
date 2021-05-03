using FScruiser.XF.ViewModels;
using NatCruise.Cruise.Models;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.Views
{
    public partial class TreeListView : ContentPage
    {
        public TreeListView()
        {
            InitializeComponent();

            _goToEndButton.Clicked += _goToEndButton_Clicked;
            _goToStartButton.Clicked += _goToStartButton_Clicked;
            //_treeListView.ItemSelected += _treeListView_ItemSelected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TreeListViewModel vm && vm != null)
            {
                vm.TreeAdded += ViewModel_TreeAdded;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is TreeListViewModel vm && vm != null)
            {
                vm.TreeAdded -= ViewModel_TreeAdded;
            }
        }

        private void ViewModel_TreeAdded(object sender, EventArgs e)
        {
            ScrollLast();
        }

        private void _treeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (BindingContext is TreeListViewModel vm
                && e.SelectedItem is TreeStub tree && tree != null)
            {
                _ = vm.ShowEditTreeAsync(tree);
            }

            _treeListView.SelectedItem = null; //deselect selected item
        }

        private void _goToEndButton_Clicked(object sender, EventArgs e)
        {
            ScrollLast();
        }

        private void ScrollLast()
        {
            var itemSource = _treeListView.ItemsSource;
            if (itemSource == null) { return; }
            var lastIndex = itemSource.OfType<object>().Count();

            _treeListView.ScrollTo(lastIndex - 1, position: ScrollToPosition.End, animate: false);
        }

        private void _goToStartButton_Clicked(object sender, EventArgs e)
        {
            ScrollFirst();
        }

        private void ScrollFirst()
        {
            var itemSource = _treeListView.ItemsSource;
            if (itemSource == null) { return; }

            _treeListView.ScrollTo(0, position: ScrollToPosition.Start, animate: false);
        }
    }
}