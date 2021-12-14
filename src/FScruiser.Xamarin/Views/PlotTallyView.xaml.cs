using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NatCruise.Util;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Markup;
using FScruiser.XF.Controls;
using FScruiser.XF.Util;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlotTallyView : ContentPage
    {
        private bool _treeCellIsSelected;

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
                vm.IsLoading = true; // prevent changing controls from triggering prop changed events
                try
                {
                    _treeEditControlGrid.Children.Clear();
                    var editControls = MakeEditControls(vm);
                    _treeEditControlGrid.Children.AddRange(editControls);
                }
                finally
                {
                    vm.IsLoading = false;
                }
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


        private void _treeEditPanel_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = _treeEditPanel.BindingContext as TreeEditViewModel;
            if (vm != null)
            {
                vm.IsLoading = true; // prevent changing controls from triggering prop changed events
                try
                {
                    _treeEditControlGrid.Children.Clear();
                    var editControls = MakeEditControls(vm);
                    _treeEditControlGrid.Children.AddRange(editControls);
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


        private IEnumerable<View> MakeEditControls(TreeEditViewModel vm)
        {
            var treeFieldValues = vm.TreeFieldValues;
            var controls = new List<View>();

            var spFieldLabel = new Label
                { Text = "Species" }
                .Column(0)
                .Row(0);

            var speciesPicker = new ValuePicker();
            SetupEditView(speciesPicker, spFieldLabel);

            var spEditControl = speciesPicker
                .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.SpeciesCode))
                .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.SpeciesOptions))
                .Column(0)
                .Row(1);

            controls.Add(spFieldLabel);
            controls.Add(spEditControl);

            int colCounter = 1;

            var cruiseMethod = vm.CruiseMethod;
            if(cruiseMethod == CruiseDAL.Schema.CruiseMethods.FIXCNT)
            {
                var treeCountLabel = new Label { Text = "Count" }
                .Column(colCounter)
                .Row(0);

                var treeCountEntry = new Entry()
                    .Bind(Entry.TextProperty, nameof(TreeEditViewModel.TreeCount))
                    .Column(colCounter)
                    .Row(1);
                SetupEditView(treeCountEntry, treeCountLabel);

                controls.Add(treeCountLabel);
                controls.Add(treeCountEntry);

                colCounter++;
            }


            
            foreach (var field in treeFieldValues)
            {
                if (field.IsHidden || field.IsLocked) { continue; }

                if (field.Field == nameof(TreeEditViewModel.LiveDead))
                {
                    var fieldLabel = new Label
                    { Text = "L/D" }
                    .Column(colCounter)
                    .Row(0);

                    var editControl = new ValuePicker()
                        .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.LiveDead))
                        .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.LiveDeadOptions))
                        .Column(colCounter)
                        .Row(1);
                    SetupEditView(editControl, fieldLabel);

                    controls.Add(fieldLabel);
                    controls.Add(editControl);
                }
                else if (field.Field == nameof(TreeEditViewModel.Initials))
                {
                    var fieldLabel = new Label()
                    {
                        Text = field.Heading
                    }
                    .Column(colCounter)
                    .Row(0);

                    var initPicker = new ValuePicker()
                        .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.Initials))
                        .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.Cruisers));
                    SetupEditView(initPicker, fieldLabel);

                    controls.Add(fieldLabel);
                    controls.Add(initPicker.Column(colCounter).Row(1));
                }
                else
                {
                    var fieldLabel = new Label()
                    {
                        Text = field.Heading
                    }
                    .Column(colCounter)
                    .Row(0);

                    var editControl = Util.TreeEditControlFactory.MakeEditView(field)
                        .Column(colCounter)
                        .Row(1);
                    SetupEditView(editControl, fieldLabel);

                    if (editControl is Entry entry)
                    {
                        entry.Completed += _entry_Completed;
                    }

                    controls.Add(fieldLabel);
                    controls.Add(editControl);
                }
                colCounter++;
            }

            return controls;
        }

        private static void SetupEditView(View editView, View label)
        {
            //view.Margin = new Thickness(5,0);
            editView.SetValue(AutomationProperties.LabeledByProperty, label);
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

        private void openDeleteButton_Clicked(object sender, EventArgs e)
        {
            var swipeview = ((Element)sender).GetAncestor<SwipeView>();
            swipeview.Open(OpenSwipeItem.LeftItems);
        }
    }
}