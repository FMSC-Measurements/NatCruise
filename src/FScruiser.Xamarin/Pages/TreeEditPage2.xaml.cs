using FScruiser.Models;
using FScruiser.XF.Util;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreeEditPage2 : ContentPage
    {

        #region TreeNumber

        /// <summary>
        /// Identifies the <see cref="TreeNumber"/> bindable property.
        /// </summary>
        public static readonly BindableProperty TreeNumberProperty =
            BindableProperty.Create(nameof(TreeNumber),
              typeof(int?),
              typeof(TreeEditPage2),
              defaultValue: default(int?),
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeEditPage2)bindable).OnTreeNumberChanged((int?)oldValue, (int?)newValue));

        /// <summary>
        /// Invoked after changes have been applied to the <see cref="TreeNumber"/> property.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="TreeNumber"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="TreeNumber"/> property.</param>
        protected virtual void OnTreeNumberChanged(int? oldValue, int? newValue)
        {

            _treeNumberEntry.Text = newValue?.ToString();

        }

        /// <summary>
        /// Gets or sets the <see cref="TreeNumber" /> property. This is a bindable property.
        /// </summary>
        public int? TreeNumber
        {
            get { return (int?)GetValue(TreeNumberProperty); }
            set { SetValue(TreeNumberProperty, value); }
        }

        #endregion TreeNumber

        #region TreeFieldValues

        /// <summary>
        /// Identifies the <see cref="TreeFieldValues"/> bindable property.
        /// </summary>
        public static readonly BindableProperty TreeFieldValuesProperty =
            BindableProperty.Create(nameof(TreeFieldValues),
              typeof(IEnumerable<TreeFieldValue>),
              typeof(TreeEditPage2),
              defaultValue: default(IEnumerable<TreeFieldValue>),
              propertyChanged: (bindable, oldValue, newValue) => ((TreeEditPage2)bindable).OnTreeFieldValuesChanged((IEnumerable<TreeFieldValue>)oldValue, (IEnumerable<TreeFieldValue>)newValue));

        /// <summary>
        /// Invoked after changes have been applied to the <see cref="TreeFieldValues"/> property.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="TreeFieldValues"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="TreeFieldValues"/> property.</param>
        protected virtual void OnTreeFieldValuesChanged(IEnumerable<TreeFieldValue> oldValue, IEnumerable<TreeFieldValue> newValue)
        {
            if (newValue != null)
            {
                var view = MakeTreeFields(newValue);
                _editViewsHost.Content = view;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TreeFieldValues" /> property. This is a bindable property.
        /// </summary>
        public IEnumerable<TreeFieldValue> TreeFieldValues
        {
            get { return (IEnumerable<TreeFieldValue>)GetValue(TreeFieldValuesProperty); }
            set { SetValue(TreeFieldValuesProperty, value); }
        }

        #endregion TreeFieldValues

        public TreeEditPage2()
        {
            InitializeComponent();

            //_altRowColor = (Color)App.Current.Resources["black_12"];
            _treeNumberEntry.Completed += _treeNumberEntry_Completed;
            _treeNumberEntry.Keyboard = Keyboard.Numeric;
        }

        private void _treeNumberEntry_Completed(object sender, EventArgs e)
        {
            var value = _treeNumberEntry.Text;
            if (int.TryParse(value, out var intValue))
            {
                TreeNumber = intValue;
            }
        }

        private View MakeTreeFields(IEnumerable<TreeFieldValue> treeFields)
        {
            if (treeFields == null) { throw new ArgumentNullException(nameof(treeFields)); }

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
            //containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
            var index = 0;
            foreach (var field in treeFields)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //if (index % 2 == 0) //alternate row color
                //{
                //    grid.Children.Add(new BoxView { Color = _altRowColor }, 0, 2, index, index + 1);
                //}

                var header = new Label() { Text = field.Heading };
                if (field.Field == "Species")
                { header.Text = "Sp/LD"; }

                grid.Children.Add(header, 0, index);

                var editView = TreeEditControlFactory.MakeEditView(field);
                if (editView is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                grid.Children.Add(editView, 1, index);
                index++;
            }
            return grid;
        }

        private static void _entry_Completed(object sender, EventArgs e)
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