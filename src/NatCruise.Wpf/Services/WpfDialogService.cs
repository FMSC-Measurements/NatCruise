using NatCruise.Navigation;
using NatCruise.Services;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Services
{
    public class WpfDialogService : INatCruiseDialogService
    {
        public WpfDialogService()
        {
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
            throw new System.NotImplementedException();
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