using Android.Media.TV;
using FScruiser.Maui.Controls;
using FScruiser.Maui.Views;
using NatCruise.Data;
using NatCruise.Navigation;
using NatCruise.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Annotations;

namespace FScruiser.Maui.Services
{
    public class MauiDialogService : INatCruiseDialogService
    {
        private TaskCompletionSource<int?> _askKpiTcs;
        private TaskCompletionSource<AskTreeCountResult> _askTreeCountTcs;

        public MauiDialogService(INavigationProvider navProvider,
            IServiceProvider services,
            ICruisersDataservice cruisersDataservice)
        {
            NavigationProvider = navProvider;
            Services = services;
            CruisersDataservice = cruisersDataservice;
        }

        public INavigationProvider NavigationProvider { get; }
        public IServiceProvider Services { get; }
        private ICruisersDataservice CruisersDataservice { get; }

        protected INavigation Navigation => NavigationProvider.Navigation;

        protected Page Page => NavigationProvider.MainPage;

        private string[] GetCruisers()
        {
            return CruisersDataservice.GetCruisers().ToArray();
        }

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

        public async Task<string> AskCruiserAsync()
        {
            var cruisers = GetCruisers();

            if (cruisers == null || cruisers.Count() == 0) { return null; }
            if (cruisers.Count() == 1) { return cruisers[0]; }

            var result = await GetCurrentPage().DisplayActionSheet("Select Cruiser", "Cancel", null, cruisers);

            if (result == "Cancel") { return null; }

            return result;
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            var newTcs = new TaskCompletionSource<int?>();

            if (System.Threading.Interlocked.CompareExchange(ref _askKpiTcs, newTcs, null) != null)//if _askKpiTcs == null then _askKpiTcs = newTcs; return origianl value of _askKpiTcs
            {
                throw new InvalidOperationException("only one dialog can be active at a time");
            }

            // TODO can we improve performance by reusing view?
            var view = Services.GetRequiredService<AskKpiView>();
            view.MinKPI = min;
            view.MaxKPI = max;

            void handelClose(object? sender, object output)
            {
                var result = output as AskKPIResult;

                var tcs = System.Threading.Interlocked.Exchange(ref _askKpiTcs, null);//_askKpiTcs = null; return original value of _askKpiTcs

                view.OnClosed -= handelClose;

                if (result == null || result.DialogResult == DialogResult.Cancel)
                {
                    tcs?.SetResult(null);
                }
                else if (result.IsSTM)
                {
                    tcs?.SetResult(-1);
                }
                else
                {
                    tcs?.SetResult(result.KPI);
                }
            }

            view.OnClosed += handelClose;

            Navigation.PushModalAsync(view);

            return _askKpiTcs.Task;
        }

        public Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount)
        {
            var newTcs = new TaskCompletionSource<AskTreeCountResult>();

            if (System.Threading.Interlocked.CompareExchange(ref _askTreeCountTcs, newTcs, null) != null)//if _askKpiTcs == null then _askKpiTcs = newTcs; return origianl value of _askKpiTcs
            {
                throw new InvalidOperationException("only one dialog can be active at a time");
            }

            var view = new ClickerTreeCountEntryDialog(defaultTreeCount);

            void handelClose(object sender, object output)
            {
                var tcs = System.Threading.Interlocked.Exchange(ref _askTreeCountTcs, null);//_askKpiTcs = null; return original value of _askKpiTcs

                view.OnClosed -= handelClose;

                if (view.DialogResult == DialogResult.Cancel)
                {
                    tcs?.SetResult(null);
                }
                else
                {
                    tcs?.SetResult(output as AskTreeCountResult);
                }
            }

            view.OnClosed += handelClose;

            Navigation.PushModalAsync(view);

            return _askTreeCountTcs.Task;
        }

        public async Task<string?> AskValueAsync(string prompt, params string[] values)
        {
            var result = await GetCurrentPage().DisplayActionSheet(prompt, "Cancel", null, values);
            if (result == "Cancel") { result = null; }
            return result;
        }

        public async Task<TValue?> AskValueAsync<TValue>(string prompt, params TValue[] values)
        {
            var value = await GetCurrentPage().DisplayActionSheet(prompt, "Cancel", null, values.Select(x => x.ToString()).ToArray());
            if (value == "Cancel") { return default; }
            return values.First(x => x.ToString() == value);
        }

        public async Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return await GetCurrentPage().DisplayAlert(caption, message, accept: "Yes", cancel: "No");
            //if (result == "Cancel") { return !defaultNo;}
            //return result switch
            //{
            //    "Yes" => true,
            //    "No" => false,
            //    "Cancel" => !defaultNo,
            //    _ => throw new InvalidOperationException()
            //};
        }

        [Obsolete]
        public Task ShowMessageAsync(string message, string caption = null)
        {
            return GetCurrentPage().DisplayAlert(caption, message, "OK");
        }

        public void ShowNotification(string message, string title = null)
        {
            ShowNotificationAsync(message, title).FireAndForget();
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            return GetCurrentPage().DisplayAlert(title, message, "OK");
        }
    }
}
