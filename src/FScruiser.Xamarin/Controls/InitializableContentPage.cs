using NatCruise.Navigation;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using NatCruise.Util;

namespace FScruiser.XF.Controls
{
    // since IInitialize only exists on Prism.Forms I use ITheRealInitialize
    // so that I can use common code for view models in NatCruise.Core
    // the purpose of this custom content page class is to forward IInitialize's calls
    // to my ITheRealInitialize interface
    public class InitializableContentPage : ContentPage, IInitialize, IInitializeAsync
    {
        public void Initialize(INavigationParameters parameters)
        {
            if (BindingContext is ITheRealInitialize viewModel)
            {
                viewModel.Initialize(parameters.ToDictionary());
            }
        }

        public Task InitializeAsync(INavigationParameters parameters)
        {
            if (BindingContext is ITheRealInitializeAsync viewModel)
            {
                return viewModel.InitializeAsync(parameters.ToDictionary());
            }
            return Task.CompletedTask;
        }
    }
}