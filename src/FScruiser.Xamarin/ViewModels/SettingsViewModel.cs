using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FScruiser.XF.Services;
using Prism.Navigation;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public IApplicationSettings AppSettings { get; set; }

        public SettingsViewModel()
        {
            AppSettings = new ApplicationSettings();
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            AppSettings.Save();
        }
    }
}
