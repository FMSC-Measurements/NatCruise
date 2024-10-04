using NatCruise.Models;
using System;
using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public interface INatCruiseDialogService
    {
        Task<string> AskCruiserAsync();

        Task<int?> AskKPIAsync(int max, int min = 1);

        Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount);

        Task<string> AskValueAsync(string prompt, params string[] values);

        Task<TValue> AskValueAsync<TValue>(string prompt, params TValue[] values);

        Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false);

        Task ShowMessageAsync(string message, string caption = null);

        void ShowNotification(string message, string title = null);

        Task ShowNotificationAsync(string message, string title = null);
    }

    public class AskTreeCountResult
    {
        //public string Cruiser { get; set; }

        public int? TreeCount { get; set; }
    }
}