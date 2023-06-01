using NatCruise.Navigation;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public class InitializableTabbedPag : TabbedPage, IInitialize, IInitializeAsync
    {
        public void Initialize(INavigationParameters parameters)
        {
            if (BindingContext is ITheRealInitialize viewModel)
            {
                viewModel.Initialize(parameters);
            }
        }

        public Task InitializeAsync(INavigationParameters parameters)
        {
            if (BindingContext is ITheRealInitializeAsync viewModel)
            {
                return viewModel.InitializeAsync(parameters);
            }
            return Task.CompletedTask;
        }
    }
}

