using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NatCruise.Wpf.DialogViews
{
    /// <summary>
    /// Interaction logic for KpiDialog.xaml
    /// </summary>
    public partial class KpiDialog : CustomDialog
    {
        private readonly TaskCompletionSource<int?> tcs = new();

        public static readonly DependencyProperty KPIProperty
            = DependencyProperty.Register(nameof(KPI),
                typeof(int),
                typeof(KpiDialog),
                new PropertyMetadata(0,
                    (d, e) => ((KpiDialog)d).OnKpiChanged((int?)e.OldValue, (int?)e.NewValue),
                    CoerceKpi));

        private static object CoerceKpi(DependencyObject d, object baseValue)
        {
            if (baseValue is int kpi && kpi < 0)
            { return 0; }
            else
            { return baseValue; }
        }



        public int? KPI
        {
            get => (int)GetValue(KPIProperty);
            set => SetValue(KPIProperty, value);
        }



        public static readonly DependencyProperty IsStmProperty
            = DependencyProperty.Register(nameof(IsStm),
                typeof(bool),
                typeof(KpiDialog),
                new PropertyMetadata(false, (d, e)=> ((KpiDialog)d).OnIsStmChanged((bool)e.OldValue, (bool)e.NewValue)));

        

        public bool IsStm
        {
            get => (bool)GetValue(IsStmProperty);
            set => SetValue(IsStmProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageProperty
            = DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(KpiDialog), new PropertyMetadata(null));

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }


        private DelegateCommand _okCommand;
        private DelegateCommand _cancelCommand;

        public int? MinKPI { get; set; }
        public int? MaxKPI { get; set; }
        public int MaxValueLength => 6;


        public DelegateCommand OkCommand => _okCommand ??= new DelegateCommand(Ok, CanExecuteOk);

        public DelegateCommand CancelCommand => _cancelCommand ??= new DelegateCommand(Cancel);

        public KpiDialog() : this(null, null)
        {
        }

        public KpiDialog(MetroWindow parentWindow, MetroDialogSettings settings) : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        public async Task<int?> WaitForResult()
        {
            return await tcs.Task.ConfigureAwait(false);
        }


        public void Cancel()
        {
            tcs.TrySetResult(null);
        }

        public void Ok()
        {
            var isStm = IsStm;
            var kpi = KPI;
            if (ValidateInput(isStm, kpi))
            {
                if (isStm)
                {
                    tcs.TrySetResult(-1);
                }
                else
                {
                    tcs.TrySetResult(kpi);
                }
            }
        }

        private bool CanExecuteOk()
        {
            return ValidateInput(IsStm, KPI);
        }

        protected void OnKpiChanged(int? oldValue, int? newValue)
        {
            ValidateInput(IsStm, newValue);
            OkCommand.RaiseCanExecuteChanged();
        }

        protected void OnIsStmChanged(bool oldValue, bool newValue)
        {
            ValidateInput(newValue, KPI);
            OkCommand.RaiseCanExecuteChanged();
        }

        protected bool ValidateInput(bool isStm, int? kpi)
        {
            if (isStm)
            {
                ErrorMessage = null;
                return true;
            }
            else
            {
                if (kpi == null) return false;

                var minKPI = MinKPI;
                var maxKPI = MaxKPI;

                if (minKPI.HasValue && minKPI.Value > 0 && kpi < minKPI.Value)
                {
                    ErrorMessage = $"KPI Must be Greater or Equal to {minKPI}";
                    return false;
                }
                else if (maxKPI.HasValue
                    && (maxKPI.Value > 0 && maxKPI.Value > (minKPI ?? 0))
                    && kpi > maxKPI.Value)
                {
                    ErrorMessage = $"Value Must be Less Than or Equal to {maxKPI}";
                    return false;
                }
                else
                {
                    ErrorMessage = null;
                    return true;
                }
            }
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancel();
            }
            if (e.Key == Key.Enter)
            {
                Ok();
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {

            var button = (Button)sender;
        }
    }
}