using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NatCruise
{
    public abstract class ViewModelBase : Prism.Mvvm.BindableBase, IActiveAware
    {
        public IParameters Parameters { get; protected set; }
        public bool IsLoaded { get; private set; }

        private bool _isActive;

        public event EventHandler IsActiveChanged;

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
                    Analytics.TrackEvent("view_model_load",
                        new Dictionary<string, string> {
                        { "time_ms", stopwatch.ElapsedMilliseconds.ToString() },
                        { "view_model_type", this.GetType().Name },
                        });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR::::" + ex);
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { "view_model_type", this.GetType().Name } });
                }

                
            }
        }


        public virtual void Load()
        {
            Load(Parameters);
        }

        protected virtual void Load(IParameters parameters)
        {
        }
    }
}