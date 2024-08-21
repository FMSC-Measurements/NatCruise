using CommunityToolkit.Mvvm.Input;
using CruiseDAL.Schema;
using FScruiser.Maui.Services;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Common;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class TreeListViewModel : ViewModelBase
{
    private ICommand? _deleteTreeCommand;
    private ICommand? _editTreeCommand;
    private ICollection<TreeEx>? _allTrees;
    private ICommand? _addTreeCommand;
    private ICommand? _showLogsCommand;
    private bool _onlyShowTreesWithErrorsOrWarnings;
    private IEnumerable<TreeField>? _treeFields;
    private CuttingUnit? _cuttingUnit;

    public string Title => $"Unit {UnitCode} - {CuttingUnit?.Description} Trees";

    public IEnumerable<TreeField>? TreeFields
    {
        get => _treeFields;
        protected set => SetProperty(ref _treeFields, value);
    }

    public ICollection<TreeEx>? AllTrees
    {
        get { return _allTrees; }
        protected set
        {
            SetProperty(ref _allTrees, value);
            OnPropertyChanged(nameof(Trees));
        }
    }

    public bool OnlyShowTreesWithErrorsOrWarnings
    {
        get => _onlyShowTreesWithErrorsOrWarnings;
        set
        {
            SetProperty(ref _onlyShowTreesWithErrorsOrWarnings, value);
            OnPropertyChanged(nameof(Trees));
        }
    }

    public IEnumerable<TreeEx>? Trees
    {
        get
        {
            if (OnlyShowTreesWithErrorsOrWarnings)
            { return AllTrees.Where(x => x.ErrorCount > 0 || x.WarningCount > 0); }
            else
            { return AllTrees; }
        }
    }

    public string[]? StratumCodes { get; protected set; }

    public ICommand AddTreeCommand => _addTreeCommand ??= new RelayCommand(() => AddTree().FireAndForget());
    public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new RelayCommand<TreeEx>((t) => DeleteTree(t).FireAndForget());
    public ICommand EditTreeCommand => _editTreeCommand ??= new RelayCommand<TreeEx>((t) => ShowEditTree(t).FireAndForget());
    public ICommand ShowLogsCommand => _showLogsCommand ??= new RelayCommand<TreeEx>((t) => ShowLogsAsync(t).FireAndForget());

    public string? UnitCode => CuttingUnit?.CuttingUnitCode;

    public CuttingUnit? CuttingUnit
    {
        get => _cuttingUnit;
        protected set
        {
            SetProperty(ref _cuttingUnit, value);
            OnPropertyChanged(nameof(UnitCode));
            OnPropertyChanged(nameof(Title));
        }
    }

    protected ICuttingUnitDataservice CuttingUnitDatastore { get; }
    protected ITreeDataservice TreeDataservice { get; }
    public IStratumDataservice StratumDataservice { get; }
    public ISampleGroupDataservice SampleGroupDataservice { get; }
    public ISubpopulationDataservice SubpopulationDataservice { get; }
    protected INatCruiseDialogService DialogService { get; }
    protected ICruiseNavigationService NavigationService { get; }
    public ITreeFieldDataservice TreeFieldDataservice { get; }

    public event EventHandler? TreeAdded;

    public TreeListViewModel(
        INatCruiseDialogService dialogService,
        ICruiseNavigationService navigationService,
        ICuttingUnitDataservice cuttingUnitDatastore,
        IStratumDataservice stratumDataservice,
        ISampleGroupDataservice sampleGroupDataservice,
        ISubpopulationDataservice subpopulationDataservice,
        ITreeDataservice treeDataservice,
        ITreeFieldDataservice treeFieldDataservice)
    {
        CuttingUnitDatastore = cuttingUnitDatastore ?? throw new ArgumentNullException(nameof(cuttingUnitDatastore));
        TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
        StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
        SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
    }

    protected override void OnInitialize(IDictionary<string, object> parameters)
    {
        if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        var unitCode = parameters.GetValue<string>(NavParams.UNIT);
        var cuttingUnit = CuttingUnit = CuttingUnitDatastore.GetCuttingUnit(unitCode);

        if (IsLoaded is false)
        {
            TreeFields = TreeFieldDataservice.GetNonPlotTreeFields(unitCode);
            StratumCodes = StratumDataservice.GetStratumCodesByUnit(unitCode).ToArray();
        }
        AllTrees = TreeDataservice.GetTreesByUnitCode(unitCode).ToObservableCollection();
    }

    public async Task AddTree()
    {
        var stratumCode = await DialogService.AskValueAsync("Select Stratum", StratumCodes);

        if (stratumCode != null)
        {
            var sampleGroups = SampleGroupDataservice.GetSampleGroupCodes(stratumCode)
                .OrEmpty().ToArray();

            var sampleGroupCode = await DialogService.AskValueAsync("Select Sample Group", sampleGroups);

            if (sampleGroupCode != null)
            {
                await AddTree(stratumCode, sampleGroupCode);
            }
        }
    }

    protected async Task AddTree(string stratumCode, string sampleGroupCode)
    {
        var sg = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);

        var defaultLD = sg.DefaultLiveDead;
        var subPops = SubpopulationDataservice.GetSubpopulations(stratumCode, sampleGroupCode)
            .ToDictionary(x => x.SpeciesCode + ((x.LiveDead != defaultLD) ? $" ({x.LiveDead})" : ""));

        Subpopulation selectedSubPop = (subPops.Count == 1) ? subPops.Values.First() : null;
        if (selectedSubPop == null)
        {
            var subPopAlias = subPops.Keys.ToArray();
            var selectedSubPopAlias = await DialogService.AskValueAsync("Select Sub-Population", subPopAlias);
            if (selectedSubPopAlias is null) return;
            subPops.TryGetValue(selectedSubPopAlias, out selectedSubPop);
        }

        var cruiseMethod = StratumDataservice.GetCruiseMethod(stratumCode);
        string? treeID = null;
        if (cruiseMethod == CruiseMethods.THREEP || cruiseMethod == CruiseMethods.S3P)
        {
            var kpi = await DialogService.AskKPIAsync((int)sg.MaxKPI, (int)sg.MinKPI);
            if (kpi is null) { return; }

            var isSTM = kpi is -1;
            treeID = TreeDataservice.InsertManualTree(UnitCode,
                    stratumCode,
                    sampleGroupCode: sampleGroupCode,
                    species: selectedSubPop.SpeciesCode,
                    liveDead: selectedSubPop.LiveDead,
                    treeCount: 0,
                    kpi: isSTM ? 0 : kpi.Value,
                    stm: isSTM);
        }
        else
        {
            var isHpct = cruiseMethod == CruiseMethods.H_PCT;
            var treeCount = (isHpct ? 1 : 0);

            treeID = TreeDataservice.InsertManualTree(UnitCode,
                stratumCode,
                sampleGroupCode,
                species: selectedSubPop.SpeciesCode,
                liveDead: selectedSubPop.LiveDead,
                treeCount: treeCount);
        }

        OnTreeAdded(treeID);
    }

    public void OnTreeAdded(string treeID)
    {
        var newTree = TreeDataservice.GetTree(treeID);
        AllTrees.Add(newTree);

        //reset OnlyShowTreesWithErrorsOrWarnings value to ensure new tree is shown
        OnlyShowTreesWithErrorsOrWarnings = false;
        OnPropertyChanged(nameof(Trees));

        TreeAdded?.Invoke(this, EventArgs.Empty);
    }

    public Task ShowEditTree(TreeEx? tree)
    {
        if (tree == null) { return Task.CompletedTask; }
        return NavigationService.ShowTreeEdit(tree.TreeID);
    }

    private async Task DeleteTree(TreeEx? tree)
    {
        if (tree == null) { return; }

        var result = await DialogService.AskValueAsync("Delete Tree?", "yes", "no");
        if (result is "yes")
        {
            TreeDataservice.DeleteTree(tree.TreeID);
            AllTrees.Remove(tree);
            OnPropertyChanged(nameof(Trees));
        }
    }

    public Task ShowLogsAsync(TreeEx? tree)
    {
        return NavigationService.ShowLogsList(tree.TreeID);
    }
}