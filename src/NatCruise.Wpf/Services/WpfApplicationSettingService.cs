using NatCruise.Services;
using NatCruise.Wpf.Properties;
using Prism.Mvvm;
using System;
using System.IO;

namespace NatCruise.Wpf.Services
{
    public class WpfApplicationSettingService : BindableBase, IWpfApplicationSettingService
    {
        private bool _isSuperuserModeEnabled;

        public WpfApplicationSettingService(ILoggingService loggingService)
        {
            var userDocumentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DefaultOpenCruiseDir = Path.Combine(userDocumentsDir, "CruiseFiles");
            DefaultOpenTemplateDir = Path.Combine(DefaultOpenCruiseDir, "Templates");
            AppSettings = Settings.Default;
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        protected Settings AppSettings { get; }
        public ILoggingService LoggingService { get; }
        public string DefaultOpenCruiseDir { get; }
        public string DefaultOpenTemplateDir { get; }

        public bool EnableCrashReports
        {
            get => Microsoft.AppCenter.Crashes.Crashes.IsEnabledAsync().Result;
            set => Microsoft.AppCenter.Crashes.Crashes.SetEnabledAsync(value);
        }

        public bool EnableAnalitics
        {
            get => Microsoft.AppCenter.Analytics.Analytics.IsEnabledAsync().Result;
            set => Microsoft.AppCenter.Analytics.Analytics.SetEnabledAsync(value);
        }


        public string LastOpenCruiseDir
        {
            get
            {
                var locDir = AppSettings.LastOpenCruiseDir;
                return String.IsNullOrEmpty(locDir) ? DefaultOpenCruiseDir : locDir;
            }
            set
            {
                AppSettings.LastOpenCruiseDir = value;
                AppSettings.Save();
            }
        }



        public string LastOpenTemplateDir
        {
            get
            {
                var dir = AppSettings.LastOpenTemplateDir;
                return String.IsNullOrEmpty(dir) ? DefaultOpenTemplateDir : dir;
            }
            set
            {
                AppSettings.LastOpenTemplateDir = value;
                AppSettings.Save();
            }
        }

        public bool UseNewLimitingDistanceCalculator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SelectPrevNextTreeSkipsCountTrees { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsSuperuserMode
        {
            get => _isSuperuserModeEnabled;
            set
            {
                SetProperty(ref _isSuperuserModeEnabled, value);
                if(value)
                {
                    LoggingService.LogEvent("Superuser Mode Enabled");
                }
            }
        }

        public bool IsDarkModeEnabled => throw new NotImplementedException();

        public void ToggleDarkMode()
        {
            throw new NotImplementedException();
        }
    }
}