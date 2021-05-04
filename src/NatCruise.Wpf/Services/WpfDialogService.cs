using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class WpfDialogService : IDialogService
    {
        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
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
