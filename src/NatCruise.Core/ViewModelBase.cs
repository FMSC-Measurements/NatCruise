using Prism;
using System;

namespace NatCruise
{
    public abstract class ViewModelBase : Prism.Mvvm.BindableBase, IActiveAware
    {
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
                Load();
            }
        }

        //public bool IsNavigationTarget(NavigationContext navigationContext)
        //{
        //    return true;
        //}

        //public void OnNavigatedFrom(NavigationContext navigationContext)
        //{
        //}

        //public void OnNavigatedTo(NavigationContext navigationContext)
        //{
        //    var navParams = navigationContext.Parameters as CruiseManagerNavigationParamiters;

        //    Refresh(navParams);
        //}

        //protected abstract void Refresh(CruiseManagerNavigationParamiters navParams);

        protected abstract void Load();
    }
}