﻿using NatCruise.Cruise.Services;
using Prism.Common;
using Prism.Ioc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using NatCruise.Data;
using FScruiser.XF.Views;

namespace FScruiser.XF.Services
{
    public class XamarinCruiseDialogService : ICruiseDialogService
    {
        private TaskCompletionSource<int?> _askKpiTcs;
        private TaskCompletionSource<AskTreeCountResult> _askTreeCountTcs;
        private IDataserviceProvider DatastoreProvider { get; }
        private IApplicationProvider ApplicationProvider { get; }
        private ICruisersDataservice CruisersDataservice { get; }
        //private IContainerExtension Container { get; }

        public XamarinCruiseDialogService(IApplicationProvider applicationProvider,
            //IContainerExtension container,
            IDataserviceProvider datastoreProvider,
            ICruisersDataservice cruisersDataservice)
        {
            DatastoreProvider = datastoreProvider;
            ApplicationProvider = applicationProvider;
            CruisersDataservice = cruisersDataservice;
            //Container = container;
        }

        private Page GetCurrentPage()
        {
            Page page = null;
            if (ApplicationProvider.MainPage.Navigation.ModalStack.Count > 0)
                page = ApplicationProvider.MainPage.Navigation.ModalStack.LastOrDefault();
            else
                page = ApplicationProvider.MainPage.Navigation.NavigationStack.LastOrDefault();

            if (page == null)
                page = ApplicationProvider.MainPage;

            return page;
        }

        public Task<bool> AskCancelAsync(string message, string caption, bool defaultCancel)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public async Task<string> AskCruiserAsync()
        {
            var cruisers = GetCruisers();

            if (cruisers.Count() == 0) { return null; }

            var result = await GetCurrentPage().DisplayActionSheet("Select Cruiser", "Cancel", null, cruisers);

            if (result == "Cancel") { return null; }

            return result;
        }

        public async Task<string> AskValueAsync(string prompt, params string[] values)
        {
            var result = await GetCurrentPage().DisplayActionSheet(prompt, "Cancel", null, values);
            if (result == "Cancel") { result = null; }
            return result;
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            var newTcs = new TaskCompletionSource<int?>();

            if (System.Threading.Interlocked.CompareExchange(ref _askKpiTcs, newTcs, null) != null)//if _askKpiTcs == null then _askKpiTcs = newTcs; return origianl value of _askKpiTcs
            {
                throw new InvalidOperationException("only one dialog can be active at a time");
            }

            var view = new AskKpiView() { MinKPI = min, MaxKPI = max };

            void handelClose(object sender, object output)
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

            GetCurrentPage().Navigation.PushModalAsync(view);


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

            GetCurrentPage().Navigation.PushModalAsync(view);

            return _askTreeCountTcs.Task;
        }

        private string[] GetCruisers()
        {
            return CruisersDataservice.GetCruisers().ToArray();
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return GetCurrentPage().DisplayAlert(caption, message, "OK");
        }
    }
}