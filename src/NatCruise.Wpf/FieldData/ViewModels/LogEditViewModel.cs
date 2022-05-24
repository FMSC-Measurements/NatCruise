﻿using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class LogEditViewModel : ViewModelBase
    {
        private Log _log;
        private IEnumerable<LogFieldSetup> _logFields;
        private IEnumerable<LogError> _errors;
        private IEnumerable<string> _gradeOptions;

        public Log Log
        {
            get => _log;
            set
            {
                if (_log != null)
                { _log.PropertyChanged -= Log_PropertyChanged; }
                OnLogChanged(value);
                SetProperty(ref _log, value);
                if (value != null)
                { value.PropertyChanged += Log_PropertyChanged; }
            }
        }

        private void Log_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveLog();
        }

        private void OnLogChanged(Log log)
        {
            if (log != null)
            {
                LogFields = FieldSetupDataservice.GetLogFieldSetupsByTreeID(log.TreeID);
                Errors = LogErrorDataservice.GetLogErrorsByLog(log.LogID);
            }
            else
            {
                LogFields = new LogFieldSetup[0];
                Errors = new LogError[0];
            }
        }

        public IEnumerable<LogFieldSetup> LogFields { get => _logFields; set => SetProperty(ref _logFields, value); }

        public IEnumerable<LogError> Errors { get => _errors; set => SetProperty(ref _errors, value); }
        public ILogDataservice LogDataservice { get; }
        public ILogErrorDataservice LogErrorDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }

        public IEnumerable<string> GradeOptions
        {
            get => _gradeOptions;
            set => SetProperty(ref _gradeOptions, value);
        }

        public LogEditViewModel(ILogDataservice logDataservice, ILogErrorDataservice logErrorDataservice, IFieldSetupDataservice fieldSetupDataservice)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            LogErrorDataservice = logErrorDataservice ?? throw new ArgumentNullException(nameof(logErrorDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
        }

        //protected override void Load(IParameters parameters)
        //{
        //    if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        //    var log_guid = parameters.GetValue<string>(NavParams.LogID);

        //    Log = LogDataservice.GetLog(log_guid);
        //}

        //void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        //{
        //    // do nothing
        //}

        //public void OnNavigatedFrom(INavigationParameters parameters)
        //{
        //    SaveLog();
        //}

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