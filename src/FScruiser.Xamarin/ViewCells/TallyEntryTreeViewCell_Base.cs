using Backpack.XF.WidgiWhats;
using CSharpForMarkup;
using FScruiser.Models;
using FScruiser.XF.Constants;
using FScruiser.XF.ViewModels;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.ViewCells
{
    public abstract class TallyEntryTreeViewCell_Base : TallyEntryViewCell_Base
    {
        private TreeEditViewModel _treeViewModel;

        #region EditTreeCommand

        /// <summary>
        /// Identifies the <see cref="EditTreeCommand"/> bindable property.
        /// </summary>
        public static readonly BindableProperty EditTreeCommandProperty =
            BindableProperty.Create(nameof(EditTreeCommand),
              typeof(Command<string>),
              typeof(TallyEntryTreeViewCell_Base),
              defaultValue: default(Command<string>));

        /// <summary>
        /// Gets or sets the <see cref="EditTreeCommand" /> property. This is a bindable property.
        /// </summary>
        public Command<string> EditTreeCommand
        {
            get { return (Command<string>)GetValue(EditTreeCommandProperty); }
            set { SetValue(EditTreeCommandProperty, value); }
        }

        #endregion EditTreeCommand

        protected abstract ScrollView TreeFieldViewContainer { get; }

        public TreeEditViewModel TreeViewModel
        {
            get { return _treeViewModel; }
            set
            {
                var curTreeViewModel = _treeViewModel;
                if (curTreeViewModel != null)
                {
                    curTreeViewModel.PropertyChanged -= _treeViewModel_PropertyChanged;
                    _treeViewModel.OnNavigatedFrom(new Prism.Navigation.NavigationParameters());
                }
                _treeViewModel = value;
                if (value != null)
                {
                    value.PropertyChanged += _treeViewModel_PropertyChanged;
                }
            }
        }

        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                var item = BindingContext as IHasTreeID;
                if (item?.TreeID != null)
                {
                    var container = ((App)App.Current).Container;

                    var treeEditViewModel = container.Resolve<TreeEditViewModel>();
                    treeEditViewModel.UseSimplifiedTreeFields = true;

                    TreeViewModel = treeEditViewModel;
                    RefreshTree();
                }
            }
            else
            {
                TreeViewModel = null;
                TreeFieldViewContainer.Content = null;
            }

            base.OnIsSelectedChanged(isSelected);
        }

        private void _treeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = (TreeEditViewModel)sender;

            if (e.PropertyName == nameof(TreeViewModel.TreeFieldValues))
            {
                var view = MakeEditControlContainer(viewModel.TreeFieldValues);
                view.BindingContext = TreeViewModel;

                TreeFieldViewContainer.Content = view;
            }
            if (e.PropertyName == nameof(TreeViewModel.ErrorsAndWarnings))
            {
                var grid = TreeFieldViewContainer.Content as View;
                if (grid == null) { return; }
                grid.BackgroundColor = (viewModel.ErrorsAndWarnings != null && viewModel.ErrorsAndWarnings.Count() > 0) ? Color.OrangeRed : Color.Transparent;
            }
        }

        private View MakeEditControlContainer(IEnumerable<TreeFieldValue> treeFieldValues)
        {
            var grid = new Grid();

            grid.Children.Add(new Label
            { Text = "Spcies" }
            .Col(0)
            .Row(0));

            grid.Children.Add(new ValuePicker()
                .Bind(ValuePicker.SelectedValueProperty, nameof(TreeEditViewModel.Species))
                .Bind(ValuePicker.ValueSourceProperty, nameof(TreeEditViewModel.SpeciesOptions))
                .Col(0)
                .Row(1));

            grid.Children.Add(new Label
            { Text = "L/D" }
            .Col(1)
            .Row(0));

            grid.Children.Add(new ValuePicker()
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

                if (editControl is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                grid.Children.Add(fieldLabel);
                grid.Children.Add(editControl);
                counter++;
            }

            return grid;
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

        private void RefreshTree()
        {
            var item = (IHasTreeID)BindingContext;
            var treeID = item?.TreeID;
            if (treeID != null)
            {
                TreeViewModel?.OnNavigatedTo(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, treeID } });
            }
        }
    }
}