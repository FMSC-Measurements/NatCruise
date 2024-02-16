using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Services
{
    public class MauiDialogService : INatCruiseDialogService
    {
        public Task<string> AskCruiserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            throw new NotImplementedException();
        }

        public Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount)
        {
            throw new NotImplementedException();
        }

        public Task<string> AskValueAsync(string prompt, params string[] values)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            throw new NotImplementedException();
        }

        public void ShowNotification(string message, string title = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            throw new NotImplementedException();
        }
    }
}
