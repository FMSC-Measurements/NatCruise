using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Util
{
    public static class INavigationServiceExtentions
    {
        public static async Task NavigateAsyncEx(this INavigationService @this, string name, NavigationParameters parameters = null)
        {
            await @this.NavigateAsync(name, parameters);
            
        }
    }
}
