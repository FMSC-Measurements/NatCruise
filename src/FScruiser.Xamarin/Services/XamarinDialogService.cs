using NatCruise.Services;
using NatCruise.Util;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public class XamarinDialogService : IDialogService
    {
        public IPageDialogService PageDialogService { get; }

        public XamarinDialogService(IPageDialogService pageDialogService)
        {
            PageDialogService = pageDialogService ?? throw new ArgumentNullException(nameof(pageDialogService));
        }

        public void ShowNotification(string message, string title = null)
        {
            PageDialogService.DisplayAlertAsync(title, message, "OK").FireAndForget();
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            return PageDialogService.DisplayAlertAsync(title, message, "OK");
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return PageDialogService.DisplayAlertAsync(caption, message, "Yes", "No");
        }
    }
}
