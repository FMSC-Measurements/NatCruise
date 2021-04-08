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
            OnInitialize(parameters);
        }

        protected virtual void OnInitialize(INavigationParameters parameters)
        { }
    }
}