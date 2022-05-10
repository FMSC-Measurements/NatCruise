﻿using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Models;
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
        public ILogErrorDataservice LogErrorDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }

        public LogEditViewModel(ILogDataservice logDataservice, ILogErrorDataservice logErrorDataservice, IFieldSetupDataservice fieldSetupDataservice)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            LogErrorDataservice = logErrorDataservice ?? throw new ArgumentNullException(nameof(logErrorDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var log_guid = parameters.GetValue<string>(NavParams.LogID);

            var log = LogDataservice.GetLog(log_guid);

            LogFields = FieldSetupDataservice.GetLogFieldSetupsByTreeID(log.TreeID);
            Errors = LogErrorDataservice.GetLogErrorsByLog(log.LogID);
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