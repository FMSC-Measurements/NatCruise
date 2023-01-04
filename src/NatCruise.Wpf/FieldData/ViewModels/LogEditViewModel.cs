﻿using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        public ITreeDataservice TreeDataservice { get; }
        public ILogErrorDataservice LogErrorDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }
        public ILoggingService LoggingService { get; }
        public INatCruiseDialogService DialogService { get; }

        public IEnumerable<string> GradeOptions
        {
            get => _gradeOptions;
            set => SetProperty(ref _gradeOptions, value);
        }

        public LogEditViewModel(ILogDataservice logDataservice,
            ITreeDataservice treeDataservice,
            ILogErrorDataservice logErrorDataservice,
            IFieldSetupDataservice fieldSetupDataservice,
            ILoggingService loggingService,
            INatCruiseDialogService dialogService)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            LogErrorDataservice = logErrorDataservice ?? throw new ArgumentNullException(nameof(logErrorDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
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
            SaveLog(log);
        }

        public void SaveLog(Log log)
        {
            if (log != null)
            {
                try
                {
                    LogDataservice.UpdateLog(log);
                }
                catch (FMSC.ORM.ConstraintException ex)
                {
                    LoggingService.LogException(nameof(LogEditViewModel), "SaveLog", ex);
                    DialogService.ShowMessageAsync("Save Log Error - Invalid Field Value");
                }
            }
        }
    }
}