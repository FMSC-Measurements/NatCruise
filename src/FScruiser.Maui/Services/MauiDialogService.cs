using Android.Media.TV;
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
        public MauiDialogService(INavigationProvider navProvider)
        {
            NavigationProvider = navProvider;
        }

        public INavigationProvider NavigationProvider { get; }

        protected INavigation Navigation => NavigationProvider.Navigation;

        protected Page Page => NavigationProvider.MainPage;

        private Page GetCurrentPage()
        {
            // TODO do we need to get the Navigation using this method
            // or can we get the Navigation in the constructor
            Page page = null;
            if (Navigation.ModalStack.Count > 0)
                page = Navigation.ModalStack.LastOrDefault();
            else
                page = Navigation.NavigationStack.LastOrDefault();

            if (page == null)
                page = Page;

            return page;
        }

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

        public async Task<string> AskValueAsync(string prompt, params string[] values)
        {
            var result = await GetCurrentPage().DisplayActionSheet(prompt, "Cancel", null, values);
            if (result == "Cancel") { result = null; }
            return result;
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
