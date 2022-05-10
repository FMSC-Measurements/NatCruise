﻿using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FScruiser.XF.ViewModels
{
    public class LogsListViewModel : XamarinViewModelBase
    {
        private ICommand _addLogCommand;
        private ICommand _editLogCommand;
        private ObservableCollection<Log> _logs;
        private IEnumerable<LogFieldSetup> _logFields;
        private int? _treeNumber;
        private ICommand _deleteLogCommand;

        public int? TreeNumber
        {
            get { return _treeNumber; }
            set { SetProperty(ref _treeNumber, value); }
        }

        public ObservableCollection<Log> Logs
        {
            get => _logs;
            protected set => SetProperty(ref _logs, value);
        }

        public IEnumerable<LogFieldSetup> LogFields
        {
            get => _logFields;
            protected set => SetProperty(ref _logFields, value);
        }

        protected ITreeDataservice TreeDataservice { get; }
        protected ILogDataservice LogDataservice { get; }
        protected ICuttingUnitDataservice Datastore { get; }
        protected ICruiseNavigationService NavigationService { get; }

        public ICommand AddLogCommand => _addLogCommand ?? (_addLogCommand = new DelegateCommand(ShowAddLogPage));

        public ICommand DeleteLogCommand => _deleteLogCommand ??= new DelegateCommand<Log>(DeleteLog);
        

        public ICommand EditLogCommand => _editLogCommand ?? (_editLogCommand = new DelegateCommand<Log>(ShowEditLogPage));

        public string Tree_GUID { get; private set; }

        public LogsListViewModel(
            ICruiseNavigationService navigationService,
            ITreeDataservice treeDataservice,
            ILogDataservice logDataservice)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var tree_guid = Tree_GUID = parameters.GetValue<string>(NavParams.TreeID)
                ?? parameters.GetValue<string>(KnownNavigationParameters.XamlParam);

            TreeNumber = TreeDataservice.GetTreeNumber(tree_guid);

            LogFields = LogDataservice.GetLogFields(tree_guid);

            Logs = LogDataservice.GetLogs(tree_guid).ToObservableCollection()
                ?? new ObservableCollection<Log>();
        }

        public void DeleteLog(Log log)
        {
            if(log is null) { return; }
            LogDataservice.DeleteLog(log.LogID);
            Logs.Remove(log);
        }

        private void ShowAddLogPage()
        {
            var newLog = new Log()
            {
                TreeID = Tree_GUID
            };

            LogDataservice.InsertLog(newLog);

            _logs.Add(newLog);
        }

        public void ShowEditLogPage(Log log)
        {
            NavigationService.ShowLogEdit(log.LogID);

            //NavigationService.NavigateAsync("Log", new NavigationParameters($"{NavParams.LogEdit_CreateNew}=false&{NavParams.LogID}={log.LogID}"));
        }
    }
}