﻿using FScruiser.XF.Controls;
using FScruiser.XF.Util;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LogEditViewModel = NatCruise.MVVM.ViewModels.LogEditViewModel;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogEditView : InitializableContentPage
    {
        #region LogFields

        /// <summary>
        /// Identifies the <see cref="LogFields"/> bindable property.
        /// </summary>
        public static readonly BindableProperty LogFieldsProperty =
            BindableProperty.Create(nameof(LogFields),
              typeof(IEnumerable<LogFieldSetup>),
              typeof(LogEditView),
              defaultValue: default(IEnumerable<LogFieldSetup>),
              defaultBindingMode: BindingMode.Default,
              propertyChanged: (bindable, oldValue, newValue) => ((LogEditView)bindable).OnLogFieldsChanged((IEnumerable<LogFieldSetup>)oldValue, (IEnumerable<LogFieldSetup>)newValue));

        /// <summary>
        /// Invoked after changes have been applied to the <see cref="LogFields"/> property.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="LogFields"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="LogFields"/> property.</param>
        protected virtual void OnLogFieldsChanged(IEnumerable<LogFieldSetup> oldValue, IEnumerable<LogFieldSetup> newValue)
        {
            if (newValue != null)
            {
                var view = MakeLogFields(newValue);
                _editViewsHost.Content = view;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LogFields" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public IEnumerable<LogFieldSetup> LogFields
        {
            get { return (IEnumerable<LogFieldSetup>)GetValue(LogFieldsProperty); }
            set { SetValue(LogFieldsProperty, value); }
        }

        #endregion LogFields

        public LogEditView()
        {
            InitializeComponent();

            this.SetBinding(LogFieldsProperty, nameof(LogEditViewModel.LogFields));
        }

        protected View MakeLogFields(IEnumerable<LogFieldSetup> logFields)
        {
            if (logFields == null) { throw new ArgumentNullException(nameof(logFields)); }

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
            //containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
            var index = 0;
            foreach (var field in logFields)
            {
                var editView = LogEditControlFactory.MakeEditView(field);
                if (editView == null) { continue; }
                if (editView is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                if (index % 2 == 0) //alternate row color
                {
                    grid.Children.Add(new BoxView
                    {
                        Color = Color.FromHex("1E000000") // black_12
                    }, 0, 2, index, index + 1);
                }

                var header = new Label() { Text = field.Heading };
                //if (field.Field == "Species")
                //{ header.Text = "Sp/LD"; }

                grid.Children.Add(header, 0, index);

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