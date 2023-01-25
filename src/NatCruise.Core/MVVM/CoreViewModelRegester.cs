using NatCruise.MVVM.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM
{
    public class CoreViewModelRegester : IViewModelRegester
    {
        public void RegisterViewModels()
        {
            ViewModelLocationProvider.Register("TreeCountEditView", typeof(TreeCountEditViewModel));
        }
    }
}
