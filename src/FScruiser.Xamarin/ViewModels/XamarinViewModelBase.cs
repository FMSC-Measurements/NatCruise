using Microsoft.AppCenter.Crashes;
using NatCruise;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FScruiser.XF.ViewModels
{
    public abstract class XamarinViewModelBase : ViewModelBase, IInitialize
    {
        private bool _isFirstNavigatedTo = true;

        public void Initialize(INavigationParameters parameters)
        {
            if (_isFirstNavigatedTo)
            {
                Parameters = parameters;
                _isFirstNavigatedTo = false;
            }
            else
            {
                throw new Exception(); // can Initialize get called multiple times?
            }
        }

        //public virtual void OnNavigatedFrom(INavigationParameters parameters)
        //{
        //}

        //public virtual void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    if (_isFirstNavigatedTo)
        //    {
        //        Parameters = parameters;
        //        _isFirstNavigatedTo = false;
        //    }

        //    try
        //    {
        //        var stopwatch = Stopwatch.StartNew();

        //        Refresh();

        //        stopwatch.Stop();
        //        Microsoft.AppCenter.Analytics.Analytics.TrackEvent("view_model_refresh",
        //            new Dictionary<string, string> {
        //                { "time_ms", stopwatch.ElapsedMilliseconds.ToString() },
        //                { "view_model_type", this.GetType().Name },
        //            });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("ERROR::::" + ex);
        //        Crashes.TrackError(ex, new Dictionary<string, string>() { { "view_model_type", this.GetType().Name } });
        //    }

        //}

        //public void Refresh()
        //{
        //    Refresh(Parameters);
        //}

        //protected abstract void Refresh(IParameters parameters);
    }
}