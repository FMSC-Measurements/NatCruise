using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
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
        private readonly TaskCompletionSource<string> tcs = new();

        public static readonly DependencyProperty SelectedValueProperty
             = DependencyProperty.Register(nameof(SelectedValue),
        typeof(string),
        typeof(SelectValueDialog),
        new PropertyMetadata(default(string)));

        public string SelectedValue
        {
            get => (string)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty ValuesProperty
            = DependencyProperty.Register(nameof(Values),
                typeof(IEnumerable<string>),
                typeof(SelectValueDialog),
                new PropertyMetadata(default(IEnumerable<string>)));

        public IEnumerable<string> Values
        {
            get => (IEnumerable<string>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }


        public SelectValueDialog()
        {
            InitializeComponent();
        }

        public SelectValueDialog(MetroWindow parentWindow) : base(parentWindow)
        {
        }

        public SelectValueDialog(MetroDialogSettings settings) : base(settings)
        {
        }

        public SelectValueDialog(MetroWindow parentWindow, MetroDialogSettings settings) : base(parentWindow, settings)
        {
        }

        public async Task<string> WaitForResult()
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                Focus();

                // focus value picker
            }));

            return await tcs.Task.ConfigureAwait(false);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null) return;

            var value = button.Content as string;

            this.tcs.TrySetResult(value);

            e.Handled = true;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
            {
                this.tcs.TrySetResult(null!);

                e.Handled = true;
            }
        }
    }
}