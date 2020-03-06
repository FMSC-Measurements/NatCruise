using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Navigation;

namespace FScruiser.XF.ViewModels
{
    public class LogEditViewModel : ViewModelBase
    {
        private Log _log;
        private IEnumerable<LogFieldSetup> _logFields;
        private IEnumerable<LogError> _errors;


        public Log Log
        {
            get => _log;
            set => SetValue(ref _log, value);
        }

        public ICuttingUnitDatastore Datastore { get; set; }

        public IEnumerable<LogFieldSetup> LogFields { get => _logFields; set => SetValue(ref _logFields, value); }

        public IEnumerable<LogError> Errors { get => _errors; set => SetValue(ref _errors, value); }

        public LogEditViewModel(INavigationService navigationService, IDataserviceProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var log_guid = parameters.GetValue<string>("Log_Guid");

            var log = Datastore.GetLog(log_guid);


            LogFields = Datastore.GetLogFields(log.TreeID);
            Errors = Datastore.GetLogErrorsByLog(log.LogID);
            Log = log;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            SaveLog();
        }

        public void SaveLog()
        {
            var log = Log;
            if (log != null)
            {
                Datastore.UpdateLog(log);
            }
        }

    }
}
