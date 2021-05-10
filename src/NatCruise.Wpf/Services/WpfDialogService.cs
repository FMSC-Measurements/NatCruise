using NatCruise.Services;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Services
{
    public class WpfDialogService : IDialogService
    {
        public WpfDialogService()
        {
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