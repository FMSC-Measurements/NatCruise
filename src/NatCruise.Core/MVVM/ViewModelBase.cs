using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NatCruise.Navigation;
using Prism;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NatCruise.MVVM
{ 

    //TODO consolidate behavior between IActiveAware and Initialize
    // ActiveAware was used because IInitialize didn't support WPF and was needed to create a common
    // point to trigger loading a view model
    // now all non-parameter dependent loading should be handled in the ViewModel constructor.
    // if it is long running use Fire and Forget Async with a IsLoaded flag and marshal property change events
    // back to the main thread.
    // Also remove use of the load method in favor of Initialize.
    // TreeEditView model sometimes use Initialize and Load to make use of the TreeEditViewModel in the tally page

    // Calling Load when view made active is a conviniant way to initialize and refresh views
    // it might be benificial to keep Initialize and Load seperate.
    // the purpose of Initialize is to set properties that would otherwise be set directly in WPF
    //
    // the question is what should be initialized during Initialize and what should be initialized during Load
    // see NatCruise.WPF.PlotEditViewModel for example on implementing cross platform loading

    public abstract class ViewModelBase : Prism.Mvvm.BindableBase, IActiveAware, ITheRealInitialize
    {
        public bool IsLoaded { get; private set; }
        private bool _isActive;
        private bool _isFirstNavigatedTo = true;

        public event EventHandler IsActiveChanged; // TODO remove if unused
        public IParameters Parameters { get; protected set; }


        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnIsActiveChanged(value);
            }
        }

        protected virtual void OnIsActiveChanged(bool isActivated)
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            if (isActivated)
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();

                    Load();
                    IsLoaded = true;

                    stopwatch.Stop();
                    var tenthsecond = stopwatch.ElapsedMilliseconds / 100; //round to tenth of second, this helps reduces the number of data points in reporting
                    var viewModelType = this.GetType().Name;
                    Analytics.TrackEvent("view_model_load",
                        new Dictionary<string, string> {
                            { "time_TenthSec", tenthsecond.ToString() },
                            { "view_model_type:time_TenthSec", viewModelType + ":" + tenthsecond.ToString() },
                            { "view_model_type", viewModelType },
                        });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR::::" + ex);
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "view_model_type", this.GetType().Name } });
                }


            }
        }

        public virtual void Initialize(IParameters parameters)
        {
            if (_isFirstNavigatedTo)
            {
                Parameters = parameters;
                _isFirstNavigatedTo = false;
            }
            OnInitialize(parameters);
        }

        protected virtual void OnInitialize(IParameters parameters)
        { }


        public virtual void Load()
        {
            Load(Parameters);
        }

        protected virtual void Load(IParameters parameters)
        {
        }
    }
}