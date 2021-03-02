using NatCruise.Cruise.Models;
using System;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ICruiseDialogService
    {
        Task<bool> AskCancelAsync(String message, String caption, bool defaultCancel);

        Task<string> AskCruiserAsync();

        Task<string> AskValueAsync(string prompt, params string[] values);

        Task<int?> AskKPIAsync(int max, int min = 1);

        Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount);

        Task ShowMessageAsync(string message, string caption = null);
    }

    public class AskTreeCountResult
    {
        //public string Cruiser { get; set; }

        public int? TreeCount { get; set; }
    }
}