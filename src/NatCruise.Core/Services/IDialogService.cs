namespace NatCruise.Services
{
    public interface IDialogService
    {
        void ShowNotification(string message, string title = null);
    }
}