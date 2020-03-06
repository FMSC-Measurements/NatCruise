using FScruiser.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INavigatedAware
    {
        private bool _isFirstNavigatedTo = true;

        protected INavigationService NavigationService { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigationParameters Parameters { get; private set; }

        //public abstract Task InitAsync();

        protected ViewModelBase()
        { }

        protected ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        protected virtual void RaisePropertyChanged(string propName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void SetValue<tTarget>(ref tTarget target, tTarget value, [CallerMemberName] string propName = null)
        {
            target = value;
            if (propName != null) { RaisePropertyChanged(propName); }
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (_isFirstNavigatedTo)
            {
                Parameters = parameters;
                _isFirstNavigatedTo = false;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();

                Refresh();

                stopwatch.Stop();
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("view_model_refresh", new Dictionary<string, string> { { "time_ms", stopwatch.ElapsedMilliseconds.ToString() } });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR::::" + ex);
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "view_model_type", this.GetType().Name } });
            }

            //MessagingCenter.Send<object, string>(this, Messages.PAGE_NAVIGATED_TO, parameters.ToString());
        }

        protected void Refresh()
        {
            Refresh(Parameters);
        }

        protected abstract void Refresh(INavigationParameters parameters);
    }
}