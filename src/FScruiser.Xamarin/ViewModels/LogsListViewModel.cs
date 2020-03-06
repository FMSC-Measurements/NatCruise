using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Services;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class LogsListViewModel : ViewModelBase
    {
        private ICommand _addLogCommand;
        private Command<Log> _editLogCommand;
        private ObservableCollection<Log> _logs;
        private IEnumerable<LogFieldSetup> _logFields;
        private int? _treeNumber;

        public int? TreeNumber
        {
            get { return _treeNumber; }
            set { SetValue(ref _treeNumber, value); }
        }

        public ObservableCollection<Log> Logs
        {
            get => _logs;
            protected set => SetValue(ref _logs, value);
        }

        public IEnumerable<LogFieldSetup> LogFields
        {
            get => _logFields;
            protected set => SetValue(ref _logFields, value);
        }

        protected ICuttingUnitDatastore Datastore { get; set; }

        public ICommand AddLogCommand => _addLogCommand ?? (_addLogCommand = new Command(ShowAddLogPage));

        public ICommand EditLogCommand => _editLogCommand ?? (_editLogCommand = new Command<Log>(ShowEditLogPage));

        public string Tree_GUID { get; private set; }

        public LogsListViewModel(INavigationService navigationService, IDataserviceProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var tree_guid = Tree_GUID = parameters.GetValue<string>("Tree_Guid")
                ?? parameters.GetValue<string>(KnownNavigationParameters.XamlParam);

            TreeNumber = Datastore.GetTreeStub(tree_guid)?.TreeNumber;

            LogFields = Datastore.GetLogFields(tree_guid);

            Logs = Datastore.GetLogs(tree_guid).ToObservableCollection()
                ?? new ObservableCollection<Log>();
        }

        private void ShowAddLogPage(object obj)
        {
            var newLog = new Log()
            {
                TreeID = Tree_GUID
            };

            Datastore.InsertLog(newLog);

            _logs.Add(newLog);
        }

        public void ShowEditLogPage(Log log)
        {
            NavigationService.NavigateAsync("Log", new NavigationParameters($"CreateNew=false&Log_Guid={log.LogID}"));
        }
    }
}