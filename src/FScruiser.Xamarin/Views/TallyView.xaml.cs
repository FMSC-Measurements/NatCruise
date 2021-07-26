using FScruiser.XF.ViewModels;
using NatCruise.Util;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NatCruise.Cruise.Models;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Markup;
using FScruiser.XF.Controls;
using FScruiser.XF.Util;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyView : ContentPage
    {
        public TallyView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var count = _tallyFeedListView.ItemsSource.OrEmpty().OfType<object>().Count();
            if (count > 1)
            {
                _tallyFeedListView.ScrollTo(count - 1, position: ScrollToPosition.End, animate: false);
            }

            var vm = _treeEditPanel.BindingContext as TreeEditViewModel;
            if (vm != null)
            {
                vm.Load();
                vm.IsLoading = true; // prevent changing controls from triggering prop changed events
                try
                {
                    _treeEditControlGrid.Children.Clear();
                    var editControls = MakeEditControls(vm.TreeFieldValues);
                    _treeEditControlGrid.Children.AddRange(editControls);
                }
                finally
                { vm.IsLoading = false; }
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

        private void _treeEditPanel_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = _treeEditPanel.BindingContext as TreeEditViewModel;
            if(vm != null)
            {
                vm.IsLoading = true; // prevent changing controls from triggering prop changed events
                try
                {
                    _treeEditControlGrid.Children.Clear();
                    var editControls = MakeEditControls(vm.TreeFieldValues);
                    _treeEditControlGrid.Children.AddRange(editControls);
                    _treeEditPanel.ForceLayout();
                }
                finally
                {
                    vm.IsLoading = false;
                }
            }
            else
            {
                _treeEditControlGrid.Children.Clear();
            }
        }


        private IEnumerable<View> MakeEditControls(IEnumerable<TreeFieldValue> treeFieldValues)
        {
            var controls = new List<View>();

            controls.Add(new Label
            { Text = "Species" }
            .Column(0)
            .Row(0));

            var speciesPicker = new ValuePicker();
            AjustEditView(speciesPicker);

            controls.Add(speciesPicker
                .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.SpeciesCode))
                .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.SpeciesOptions))
                .Column(0)
                .Row(1));


            int counter = 1;
            foreach (var field in treeFieldValues)
            {
                if (field.IsHidden || field.IsLocked) { continue; }

                if (field.Field == nameof(TreeEditViewModel.LiveDead))
                {
                    controls.Add(new Label
                    { Text = "L/D" }
            .Column(counter)
            .Row(0));

                    var ldPicker = new ValuePicker();
                    AjustEditView(ldPicker);

                    controls.Add(ldPicker
                        .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.LiveDead))
                        .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.LiveDeadOptions))
                        .Column(counter)
                        .Row(1));
                }
                else if (field.Field == nameof(TreeEditViewModel.Initials))
                {
                    var fieldLabel = new Label()
                    {
                        Text = field.Heading
                    }
                    .Column(counter)
                    .Row(0);

                    var initPicker = new ValuePicker()
                        .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.Initials))
                        .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.Cruisers));
                    AjustEditView(initPicker);

                    controls.Add(initPicker.Column(counter).Row(1));
                }
                else
                {
                    var fieldLabel = new Label()
                    {
                        Text = field.Heading
                    }
                    .Column(counter)
                    .Row(0);

                    var editControl = Util.TreeEditControlFactory.MakeEditView(field)
                        .Column(counter)
                        .Row(1);
                    AjustEditView(editControl);


                    if (editControl is Entry entry)
                    {
                        entry.Completed += _entry_Completed;
                    }

                    controls.Add(fieldLabel);
                    controls.Add(editControl);
                }
                counter++;
            }

            return controls;
        }

        private static void AjustEditView(View view)
        {
            //view.Margin = new Thickness(5,0);
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

        private void openUntallyButton_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }
    }
}