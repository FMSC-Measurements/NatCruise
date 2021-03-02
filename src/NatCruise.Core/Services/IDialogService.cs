using System.Threading.Tasks;

namespace NatCruise.Services
{
    public interface IDialogService
    {
        Task ShowNotificationAsync(string message, string title = null);

        void ShowNotification(string message, string title = null);

        Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false);
    }
}