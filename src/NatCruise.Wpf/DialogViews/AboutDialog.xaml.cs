using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NatCruise.Wpf.Services;
using Prism.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NatCruise.Wpf.DialogViews
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : CustomDialog
    {
        public const int UNLOCK_SUPERUSERMODE_CLICKCOUNT = 5;

        // we doesn't really need to return anything but
        // TaskCompletionSource with no type argument
        // doesn't exist in standard dotnet
        private readonly TaskCompletionSource<object> _tcs = new();

        private DelegateCommand _closeCommand;
        private DelegateCommand _logoClickedCommand;
        private DispatcherTimer _clickTimer;
        private int _clickCount = 0;

        public static readonly DependencyProperty VersionProperty
            = DependencyProperty.Register(nameof(Version),
                typeof(string),
                typeof(AboutDialog),
                new PropertyMetadata("<unknown>"));

        public string Version
        {
            get => (string)GetValue(VersionProperty);
            set => SetValue(VersionProperty, value);
        }

        public static readonly DependencyProperty IsSuperuserModeEnabledProperty
            = DependencyProperty.Register(nameof(IsSuperuserModeEnabled),
                typeof(bool),
                typeof(AboutDialog),
                new PropertyMetadata(false));

        public bool IsSuperuserModeEnabled
        {
            get => (bool)GetValue(IsSuperuserModeEnabledProperty);
            set
            {
                SettingsService.IsSuperuserMode = value;
                SetValue(IsSuperuserModeEnabledProperty, value);
            }
        }

        public ICommand CloseCommand => _closeCommand ??= new DelegateCommand(Close);

        public ICommand LogoClickedCommand => _logoClickedCommand ??= new DelegateCommand(OnLogoClicked);

        public IWpfApplicationSettingService SettingsService { get; }

        protected AboutDialog()
        {
            Initialize();
        }

        public AboutDialog(MetroWindow parentWindow, MetroDialogSettings settings, IWpfApplicationSettingService settingsService)
            : base(parentWindow, settings)
        {
            Initialize();

            SettingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            IsSuperuserModeEnabled = settingsService.IsSuperuserMode;
        }

        private void Initialize()
        {
            InitializeComponent();
            DataContext = this;
            _clickTimer = new() { Interval = TimeSpan.FromSeconds(3) };
            _clickTimer.Tick += _clickTimer_Tick;

            void _clickTimer_Tick(object sender, EventArgs e)
            {
                _clickCount = 0;
                _clickTimer.Stop();
            }

            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public async Task<object> WaitForResult()
        {
            return await _tcs.Task.ConfigureAwait(false);
        }

        public void Close()
        {
            _tcs.TrySetResult(null);
        }

        private void OnLogoClicked()
        {
        }

        public void EnableSupervisorMode()
        {
            SettingsService.IsSuperuserMode = true;
        }

        private void logoPanel_Click(object sender, RoutedEventArgs e)
        {
            if (IsSuperuserModeEnabled) { return; }

            // reset timer
            _clickTimer.Stop();

            _clickCount++;
            if (_clickCount == UNLOCK_SUPERUSERMODE_CLICKCOUNT)
            {
                IsSuperuserModeEnabled = true;
                _clickCount = 0;
            }
            else
            {
                _clickTimer.Start();
            }
        }
    }
}