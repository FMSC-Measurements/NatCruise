using NatCruise.Navigation;
using NatCruise.Services;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using NatCruise.Wpf.Views;

namespace NatCruise.Wpf.Services
{
    public class WpfDialogService : INatCruiseDialogService
    {
        public IAppService AppService { get; }

        public MainWindow MainWindow => (MainWindow)AppService.MainWindow;

        public WpfDialogService(IAppService appService)
        {
            AppService = appService;
        }

        public Task<bool> AskCancelAsync(string message, string caption, bool defaultCancel)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> AskCruiserAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            throw new System.NotImplementedException();
        }

        public Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> AskValueAsync(string prompt, params string[] values)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return Task.Run(() =>
                MessageBox.Show(message, caption,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    (defaultNo) ? MessageBoxResult.No : MessageBoxResult.Yes) == MessageBoxResult.Yes
                );
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return MainWindow.ShowMessageAsync(message, caption);
        }

        public void ShowNotification(string message, string title = null)
        {
            MessageBox.Show(message, title);
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            return Task.Run(() => MessageBox.Show(message, title));
        }
    }
}