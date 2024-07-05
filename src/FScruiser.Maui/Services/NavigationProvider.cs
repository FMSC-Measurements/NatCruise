using FScruiser.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Services
{
    public interface INavigationProvider
    {
        Page MainPage { get; set; }

        INavigation Navigation { get; }
    }

    public class NavigationProvider : INavigationProvider
    {
        public Page MainPage { get; set; }

        public INavigation? Navigation => GetNavigation();

        protected INavigation GetNavigation()
        {
            var mainPage = MainPage;
            if(mainPage is FlyoutPage fp)
            {
                return fp.Detail.Navigation;
            }
            else
            {
                return mainPage.Navigation;
            }
        }
    }
}
