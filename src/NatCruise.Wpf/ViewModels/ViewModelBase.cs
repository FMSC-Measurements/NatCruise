using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatCruise.Wpf.Navigation;
using Prism;
using Prism.Regions;

namespace NatCruise.Wpf.ViewModels
{
    public abstract class ViewModelBase : Prism.Mvvm.BindableBase, IActiveAware//, Prism.Regions.INavigationAware
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
            if(isActivated)
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
