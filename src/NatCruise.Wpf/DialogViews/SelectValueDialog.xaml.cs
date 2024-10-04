using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NatCruise.Wpf.DialogViews
{
    /// <summary>
    /// Interaction logic for SelectValueDialog.xaml
    /// </summary>
    public partial class SelectValueDialog : CustomDialog
    {
        private readonly TaskCompletionSource<object> tcs = new();

        public static readonly DependencyProperty SelectedValueProperty
             = DependencyProperty.Register(nameof(SelectedValue),
        typeof(object),
        typeof(SelectValueDialog),
        new PropertyMetadata(default(string)));

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty ValuesProperty
            = DependencyProperty.Register(nameof(Values),
                typeof(IList),
                typeof(SelectValueDialog),
                new PropertyMetadata(default(IList)));

        public IList Values
        {
            get => (IList)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        public SelectValueDialog() : this(null)
        { }

        public SelectValueDialog(MetroWindow parentWindow) : this(parentWindow, null)
        { }

        public SelectValueDialog(MetroWindow parentWindow, MetroDialogSettings settings) : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        public async Task<object> WaitForResult()
        {
            //await Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    Focus();

            //    // focus value picker
            //}));

            return await tcs.Task.ConfigureAwait(false);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null) return;

            if (object.ReferenceEquals(button, _cancelButton))
            {
                tcs.TrySetResult(null);
            }
            else
            {
                var value = button.Content;

                tcs.TrySetResult(value);
            }

            e.Handled = true;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
            {
                tcs.TrySetResult(null!);

                e.Handled = true;
            }
        }
    }
}