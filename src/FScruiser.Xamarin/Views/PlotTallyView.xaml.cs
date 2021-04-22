using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NatCruise.Util;
using System.Collections.Generic;
using CSharpForMarkup;
using Backpack.XF.WidgiWhats;

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
                _treeEditControlGrid.Children.Clear();
                vm.Load();
                var editControls = MakeEditControls(vm.TreeFieldValues);
                _treeEditControlGrid.Children.AddRange(editControls);
            }
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
                _treeEditControlGrid.Children.Clear();
                var editControls = MakeEditControls(vm.TreeFieldValues);
                _treeEditControlGrid.Children.AddRange(editControls);
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
            { Text = "Spcies" }
            .Col(0)
            .Row(0));

            var speciesPicker = new ValuePicker();
            AjustEditView(speciesPicker);

            controls.Add(speciesPicker
                .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.SpeciesCode))
                .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.SpeciesOptions))
                .Col(0)
                .Row(1));

            controls.Add(new Label
            { Text = "L/D" }
            .Col(1)
            .Row(0));

            var ldPicker = new ValuePicker();
            AjustEditView(ldPicker);

            controls.Add(ldPicker
                .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.LiveDead))
                .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.LiveDeadOptions))
                .Col(1)
                .Row(1));

            int counter = 2;
            foreach (var field in treeFieldValues)
            {
                var fieldLabel = new Label()
                {
                    Text = field.Heading
                }
                .Col(counter)
                .Row(0);

                var editControl = Util.TreeEditControlFactory.MakeEditView(field)
                    .Col(counter)
                    .Row(1);
                AjustEditView(editControl);

                if (editControl is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                controls.Add(fieldLabel);
                controls.Add(editControl);
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
    }
}