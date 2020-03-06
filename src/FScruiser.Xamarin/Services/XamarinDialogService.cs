using FScruiser.Services;
using FScruiser.XF.Pages;
using Prism.Common;
using Prism.Ioc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Services
{
    public class XamarinDialogService : IDialogService
    {
        private TaskCompletionSource<int?> _askKpiTcs;
        private TaskCompletionSource<AskTreeCountResult> _askTreeCountTcs;
        private IDataserviceProvider _datastoreProvider;
        private IApplicationProvider _applicationProvider;
        private IContainerExtension _container;

        public XamarinDialogService(IApplicationProvider applicationProvider,
            IContainerExtension container,
            IDataserviceProvider datastoreProvider )
        {
            _datastoreProvider = datastoreProvider;
            _applicationProvider = applicationProvider;
            _container = container;
        }

        private Page GetCurrentPage()
        {
            Page page = null;
            if (_applicationProvider.MainPage.Navigation.ModalStack.Count > 0)
                page = _applicationProvider.MainPage.Navigation.ModalStack.LastOrDefault();
            else
                page = _applicationProvider.MainPage.Navigation.NavigationStack.LastOrDefault();

            if (page == null)
                page = _applicationProvider.MainPage;

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

            var view = new AskKpiPage() { MinKPI = min, MaxKPI = max };

            void handelClose(object sender, object output)
            {
                var reslut = output as AskKPIResult;

                var tcs = System.Threading.Interlocked.Exchange(ref _askKpiTcs, null);//_askKpiTcs = null; return original value of _askKpiTcs

                view.OnClosed -= handelClose;

                if (reslut.DialogResult == DialogResult.Cancel)
                {
                    tcs?.SetResult(null);
                }
                else if (reslut.IsSTM)
                {
                    tcs?.SetResult(-1);
                }
                else
                {
                    tcs?.SetResult(reslut.KPI);
                }
            }

            view.OnClosed += handelClose;

            GetCurrentPage().Navigation.PushModalAsync(view);


            return _askKpiTcs.Task;
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return App.Current.MainPage.DisplayAlert(caption, message, "Yes", "No");
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
            var cruisers = _datastoreProvider.Get<ICruisersDataservice>();
            return cruisers.GetCruisers().ToArray();
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return GetCurrentPage().DisplayAlert(caption, message, "OK");
        }
    }
}