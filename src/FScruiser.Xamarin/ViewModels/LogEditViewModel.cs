using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Navigation;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace FScruiser.XF.ViewModels
{
    public class LogEditViewModel : XamarinViewModelBase, INavigatedAware
    {
        private Log _log;
        private IEnumerable<LogFieldSetup> _logFields;
        private IEnumerable<LogError> _errors;

        public Log Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }

        public IEnumerable<LogFieldSetup> LogFields { get => _logFields; set => SetProperty(ref _logFields, value); }

        public IEnumerable<LogError> Errors { get => _errors; set => SetProperty(ref _errors, value); }
        public ILogDataservice LogDataservice { get; }

        public LogEditViewModel(ILogDataservice logDataservice)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var log_guid = parameters.GetValue<string>(NavParams.LogID);

            var log = LogDataservice.GetLog(log_guid);

            LogFields = LogDataservice.GetLogFields(log.TreeID);
            Errors = LogDataservice.GetLogErrorsByLog(log.LogID);
            Log = log;
        }

        void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            // do nothing
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            SaveLog();
        }

        public void SaveLog()
        {
            var log = Log;
            if (log != null)
            {
                LogDataservice.UpdateLog(log);
            }
        }
    }
}