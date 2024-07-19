using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM.ViewModels
{
    public class LogEditViewModel : ViewModelBase
    {
        private Log _log;
        private IReadOnlyCollection<LogFieldSetup> _logFields;
        private IReadOnlyCollection<LogError> _errors;
        private IReadOnlyCollection<string> _gradeOptions;
        private IReadOnlyCollection<LogFieldValue> _logFieldValues;

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
                var logFieldValues = LogFieldValueDataservice.GetLogFieldValues(log.LogID).ToArray();
                if (logFieldValues != null || logFieldValues.Length == 0)
                {
                    logFieldValues = new LogFieldValue[]
                    {
                        new LogFieldValue()
                        {
                            LogID = log.LogID,
                            Field = "Grade",
                            Heading = "Grade",
                            ValueText = log.Grade,
                        },
                        new LogFieldValue()
                        {
                            LogID = log.LogID,
                            Field = "SeenDefect",
                            Heading = "PctSeenDef",
                            ValueReal = log.SeenDefect,
                        },
                    };
                }

                LogFieldValues = logFieldValues;

                LogFields = FieldSetupDataservice.GetLogFieldSetupsByTreeID(log.TreeID).ToArray();
                Errors = LogErrorDataservice.GetLogErrorsByLog(log.LogID).ToArray();
            }
            else
            {
                LogFieldValues = new LogFieldValue[0];
                LogFields = new LogFieldSetup[0];
                Errors = new LogError[0];
            }
        }

        public IReadOnlyCollection<LogFieldSetup> LogFields { get => _logFields; set => SetProperty(ref _logFields, value); }

        public IReadOnlyCollection<LogFieldValue> LogFieldValues
        {
            get => _logFieldValues;
            set
            {
                if (_logFieldValues != null)
                {
                    foreach (var item in _logFieldValues)
                    {
                        item.PropertyChanged -= LogFieldValue_PropertyChanged;
                    }
                }
                SetProperty(ref _logFieldValues, value);
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        item.PropertyChanged += LogFieldValue_PropertyChanged;
                    }
                }
            }
        }

        private void LogFieldValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var logFieldValue = (LogFieldValue)sender;
            LogFieldValueDataservice.UpdateLogFieldValue(logFieldValue);
        }

        public IReadOnlyCollection<LogError> Errors { get => _errors; set => SetProperty(ref _errors, value); }
        public ILogDataservice LogDataservice { get; }
        public ILogErrorDataservice LogErrorDataservice { get; }
        public ILogFieldValueDataservice LogFieldValueDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }
        public ILoggingService LoggingService { get; }
        public INatCruiseDialogService DialogService { get; }

        public IReadOnlyCollection<string> GradeOptions
        {
            get => _gradeOptions;
            set => SetProperty(ref _gradeOptions, value);
        }

        public LogEditViewModel(ILogDataservice logDataservice,
            ILogErrorDataservice logErrorDataservice,
            IFieldSetupDataservice fieldSetupDataservice,
            ILogFieldValueDataservice logFieldValueDataservice,
            ILoggingService loggingService,
            INatCruiseDialogService dialogService)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            LogErrorDataservice = logErrorDataservice ?? throw new ArgumentNullException(nameof(logErrorDataservice));
            LogFieldValueDataservice = logFieldValueDataservice ?? throw new ArgumentNullException(nameof(logFieldValueDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            GradeOptions = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        }

        protected override void Load(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var logID = parameters.GetValue<string>(NavParams.LogID);

            Load(logID);
        }

        public void Load(string logID)
        {
            Log = LogDataservice.GetLog(logID);
        }

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
