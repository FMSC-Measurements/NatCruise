using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Services;
using Microsoft.Extensions.Logging;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class LogsListViewModel : ViewModelBase
{
    private ICommand? _addLogCommand;
    private ICommand? _editLogCommand;
    private ObservableCollection<Log>? _logs;
    private IEnumerable<LogFieldSetup>? _logFields;
    private ICommand? _deleteLogCommand;
    private Tree? _tree;

    //public int? TreeNumber
    //{
    //    get { return _treeNumber; }
    //    set { SetProperty(ref _treeNumber, value); }
    //}

    public Tree? Tree
    {
        get => _tree;
        protected set => SetProperty(ref _tree, value);
    }

    public ObservableCollection<Log>? Logs
    {
        get => _logs;
        protected set
        {
            if(_logs != null)
            {
                foreach (var log in _logs)
                {
                    log.PropertyChanged -= Log_PropertyChanged;
                }
            }

            SetProperty(ref _logs, value);

            if(value != null)
            {
                foreach (var log in value)
                {
                    log.PropertyChanged += Log_PropertyChanged;
                }
            }
        }
    }

    private void Log_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Log log)
        {
            Logger.LogTrace($"Log property changed: {e.PropertyName}");
            SaveLog(log);
        }
    }

    public IEnumerable<LogFieldSetup>? LogFields
    {
        get => _logFields;
        protected set => SetProperty(ref _logFields, value);
    }

    protected ITreeDataservice TreeDataservice { get; }
    protected ILogDataservice LogDataservice { get; }
    protected ICuttingUnitDataservice Datastore { get; }
    protected ICruiseNavigationService NavigationService { get; }
    public IFieldSetupDataservice FieldSetupDataservice { get; }
    public ILogger Logger { get; }
    public INatCruiseDialogService DialogService { get; }

    public ICommand AddLogCommand => _addLogCommand ??= new RelayCommand(ShowAddLogPage);

    public ICommand DeleteLogCommand => _deleteLogCommand ??= new RelayCommand<Log>(DeleteLog);

    public ICommand EditLogCommand => _editLogCommand ??= new RelayCommand<Log>(ShowEditLogPage);

    public string? Tree_GUID { get; private set; }

    public LogsListViewModel(
        ICruiseNavigationService navigationService,
        ITreeDataservice treeDataservice,
        ILogDataservice logDataservice,
        IFieldSetupDataservice fieldSetupDataservice,
        ILogger<LogsListViewModel> log,
        INatCruiseDialogService dialogService)
    {
        TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
        LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
        Logger = log ?? throw new ArgumentNullException(nameof(log));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    protected override void OnInitialize(IDictionary<string, object> parameters)
    {
        if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        var tree_guid = Tree_GUID = parameters.GetValue<string>(NavParams.TreeID);

        Tree = TreeDataservice.GetTree(tree_guid);

        //TreeNumber = TreeDataservice.GetTreeNumber(tree_guid);

        LogFields = FieldSetupDataservice.GetLogFieldSetupsByTreeID(tree_guid);

        Logs = LogDataservice.GetLogs(tree_guid).ToObservableCollection()
            ?? new ObservableCollection<Log>();
    }

    public void DeleteLog(Log? log)
    {
        if (log is null) { return; }
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

    public void ShowEditLogPage(Log? log)
    {
        if (log is null) { return; }
        NavigationService.ShowLogEdit(log.LogID);

        //NavigationService.NavigateAsync("Log", new NavigationParameters($"{NavParams.LogEdit_CreateNew}=false&{NavParams.LogID}={log.LogID}"));
    }

    public void SaveLog(Log log)
    {
        if(log == null) { return; }

        try
        {
            LogDataservice.UpdateLog(log);
        }
        catch (FMSC.ORM.ConstraintException ex)
        {
            Logger.LogError(ex, "Error saving log: Constraint Exception");
            DialogService.ShowMessageAsync("Save Log Error - Invalid Field Value");
        }
    }
}