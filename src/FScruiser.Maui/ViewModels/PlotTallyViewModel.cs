using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Data;
using FScruiser.Maui.Services;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Logic;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public partial class PlotTallyViewModel : ViewModelBase
{
    private const string STRATUM_FILTER_ALL = "All";

    private ICollection<TallyPopulation_Plot>? _tallyPopulations;
    private ICollection<Stratum>? _strata;
    private string? _stratumFilter = STRATUM_FILTER_ALL;
    private NotifyTaskCompletion<ObservableCollection<PlotTreeEntry>>? _trees;

    private Plot? _plot;

    private TreeEditViewModel? _selectedTreeViewModel;
    private CuttingUnit? _cuttingUnit;
    private PlotTreeEntry? _selectedTree;

    public string Title => $"Unit {CuttingUnit?.CuttingUnitCode} - {CuttingUnit?.Description} Plot {Plot?.PlotNumber}";

    #region services

    public ITreeDataservice TreeDataservice { get; }
    public IPlotDataservice PlotDataservice { get; }
    public ICuttingUnitDataservice CuttingUnitDataservice { get; }
    public IPlotStratumDataservice PlotStratumDataservice { get; }
    public ITallyPopulationDataservice TallyPopulationDataservice { get; }
    public INatCruiseDialogService DialogService { get; }
    public ICruiseNavigationService NavigationService { get; }

    public ISampleSelectorDataService SampleSelectorDataService { get; private set; }
    public ITallySettingsDataService TallySettings { get; private set; }
    public ICruisersDataservice CruisersDataservice { get; }
    public ISoundService SoundService { get; private set; }
    public IPlotTallyService TallyService { get; }
    public IPlotTreeDataservice PlotTreeDataservice { get; }

    #endregion services

    public PlotTallyViewModel(ICruiseNavigationService navigationService,
        INatCruiseDialogService dialogService,
        IApplicationSettingService applicationSettingService,
        ITreeDataservice treeDataservice,
        IPlotDataservice plotDataservice,
        ICuttingUnitDataservice cuttingUnitDataservice,
        IPlotStratumDataservice plotStratumDataservice,
        ITallyPopulationDataservice tallyPopulationDataservice,
        ISampleSelectorDataService sampleSelectorDataservice,
        ISoundService soundService,
        ICruisersDataservice cruisersDataservice,
        ITallySettingsDataService tallySettings,
        TreeEditViewModel treeEditViewModel,
        IPlotTallyService tallyService,
        IPlotTreeDataservice plotTreeDataservice)
    {
        SelectPrevNextTreeSkipsCountTrees = applicationSettingService.SelectPrevNextTreeSkipsCountTrees;

        TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
        PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
        CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
        PlotStratumDataservice = plotStratumDataservice ?? throw new ArgumentNullException(nameof(plotStratumDataservice));
        TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
        SampleSelectorDataService = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(sampleSelectorDataservice));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        TallySettings = tallySettings ?? throw new ArgumentNullException(nameof(tallySettings));
        CruisersDataservice = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));
        SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
        TallyService = tallyService ?? throw new ArgumentNullException(nameof(tallyService));
        PlotTreeDataservice = plotTreeDataservice ?? throw new ArgumentNullException(nameof(plotTreeDataservice));

        treeEditViewModel.UseSimplifiedTreeFields = true;
        SelectedTreeViewModel = treeEditViewModel;
    }

    #region commands

    private ICommand? _editPlotCommand;
    private ICommand? _tallyCommand;
    private ICommand? _editTreeCommand;
    private ICommand? _showTallyPopulationDetailsCommand;
    private Plot_Stratum[]? _plotStrata;
    private ICollection<TallyPopulation_Plot>? _talliesFiltered;
    private bool _selectedTreeChanging;

    public ICommand EditTreeCommand => _editTreeCommand ??= new RelayCommand<string>(x => ShowEditTree(x).FireAndForget());

    public ICommand ShowTallyPopulationDetailsCommand => _showTallyPopulationDetailsCommand ??= new RelayCommand<TallyPopulation>(x => ShowTallyPopulationDetails(x).FireAndForget());

    public ICommand TallyCommand => _tallyCommand ??= new RelayCommand<TallyPopulation_Plot>(x => TallyAsync(x).FireAndForget());

    public ICommand EditPlotCommand => _editPlotCommand ??= new RelayCommand(() => ShowEditPlot().FireAndForget());

    #endregion commands

    public bool IsRecon { get; private set; } // set in constructor since it is a sale level value

    public bool SelectPrevNextTreeSkipsCountTrees { get; set; }

    public PlotTreeEntry? SelectedTree
    {
        get => _selectedTree;
        set
        {
            if (object.ReferenceEquals(_selectedTree, value)) return;

            var treeID = value?.TreeID;
            if (treeID != null)
            {
                try
                {
                    _selectedTreeChanging = true;
                    SelectedTreeViewModel.Load(treeID);

                    SetProperty(ref _selectedTree, value);
                }
                finally { _selectedTreeChanging = false; }
            }
            else
            {
                SetProperty(ref _selectedTree, null);
            }
        }
    }

    public TreeEditViewModel? SelectedTreeViewModel
    {
        get => _selectedTreeViewModel;
        protected init
        {
            if (_selectedTreeViewModel != null)
            { _selectedTreeViewModel.PropertyChanged -= SelectedTreeViewModel_PropertyChanged; }
            SetProperty(ref _selectedTreeViewModel, value);
            if (value != null)
            { value.PropertyChanged += SelectedTreeViewModel_PropertyChanged; }
        }
    }

    // propagate changes in the tree edit view model back to the tree in the tree list
    private void SelectedTreeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_selectedTreeChanging) { return; }

        var tree = SelectedTree;
        if (tree == null) return;

        var vm = (TreeEditViewModel)sender;
        Debug.Assert(tree.TreeID == vm.Tree.TreeID);
        if (e.PropertyName == nameof(TreeEditViewModel.ErrorsAndWarnings))
        {
            tree.ErrorCount = vm.ErrorCount;
            tree.WarningCount = vm.WarningCount;

            //PlotTreeDataservice.RefreshErrorsAndWarnings(tree);
        }
        if (e.PropertyName == nameof(TreeEditViewModel.SpeciesCode))
        {
            tree.SpeciesCode = vm.SpeciesCode;
        }
        if (e.PropertyName == nameof(TreeEditViewModel.LiveDead))
        {
            tree.LiveDead = vm.LiveDead;
        }
        if (e.PropertyName == nameof(TreeEditViewModel.TreeCount))
        {
            tree.TreeCount = vm.TreeCount;
        }
        if (e.PropertyName == nameof(TreeEditViewModel.CountOrMeasure))
        {
            tree.CountOrMeasure = vm.CountOrMeasure;
        }
    }

    [DisallowNull]
    public Plot? Plot
    {
        get => _plot;
        set
        {
            if (_plot == value) return;

            var plotNumber = value.PlotNumber;
            var unitCode = value.CuttingUnitCode;
            CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);

            TallyPopulations = TallyPopulationDataservice.GetPlotTallyPopulationsByUnitCode(unitCode, plotNumber).ToArray();
            SetStratumFilter(null);
            PlotStrata = PlotStratumDataservice.GetPlot_Strata(unitCode, plotNumber, false).ToArray();

            Trees = NotifyTaskCompletion.Create<ObservableCollection<PlotTreeEntry>>(
                Task.Run(() => PlotTreeDataservice.GetPlotTrees(unitCode, plotNumber).ToObservableCollection()),
                success: (src, ea) =>
                {
                    var selectedTree = SelectedTree;
                    if (selectedTree != null)
                    {
                        SelectedTree = Trees!.Result.FirstOrDefault(y => y.TreeID == selectedTree.TreeID);
                    }
                });

            SetProperty(ref _plot, value);
            OnPropertyChanged(nameof(PlotNumber));
            OnPropertyChanged(nameof(Title));
        }
    }

    public int PlotNumber => Plot?.PlotNumber ?? 0;

    public string? UnitCode => CuttingUnit?.CuttingUnitCode;

    public CuttingUnit? CuttingUnit
    {
        get => _cuttingUnit;
        protected set
        {
            SetProperty(ref _cuttingUnit, value);
            OnPropertyChanged(nameof(UnitCode));
        }
    }

    public ICollection<TallyPopulation_Plot>? TallyPopulations
    {
        get { return _tallyPopulations; }
        set
        {
            SetProperty(ref _tallyPopulations, value);
            OnPropertyChanged(nameof(TalliesFiltered));
        }
    }

    public NotifyTaskCompletion<ObservableCollection<PlotTreeEntry>>? Trees
    {
        get => _trees;
        set => SetProperty(ref _trees, value);
    }

    public ICollection<TallyPopulation_Plot>? TalliesFiltered
    {
        get => _talliesFiltered;
        private set => SetProperty(ref _talliesFiltered, value);
    }

    public ICollection<Stratum>? Strata
    {
        get { return _strata; }
        set
        {
            SetProperty(ref _strata, value);
            //RaisePropertyChanged(nameof(StrataFilterOptions));
        }
    }

    //public IEnumerable<string> StrataFilterOptions
    //{
    //    get => _stratumFilterOptions;
    //    protected set => SetProperty(ref _stratumFilterOptions, value);
    //}

    //public ICollection<string> StrataFilterOptions => Strata.OrEmpty().Select(x => x.StratumCode).Append(STRATUM_FILTER_ALL).ToArray();

    public string? StratumFilter
    {
        get { return _stratumFilter; }
        set
        {
            SetProperty(ref _stratumFilter, value);
            OnPropertyChanged(nameof(TalliesFiltered));
        }
    }

    //public IList<PlotTreeEntry> Trees
    //{
    //    get { return _trees; }
    //    set
    //    {
    //        SetProperty(ref _trees, value);
    //        // if we have a selected tree refresh it with a matching tree in the new trees collection
    //        var selectedTree = SelectedTree;
    //        if (selectedTree != null)
    //        {
    //            SelectedTree = Trees.FirstOrDefault(x => x.TreeID == selectedTree.TreeID);
    //        }
    //    }
    //}

    public Plot_Stratum[]? PlotStrata
    {
        get => _plotStrata;
        private set => SetProperty(ref _plotStrata, value);
    }

    [RelayCommand]
    public void SelectTree(object obj)
    {
        var tree = obj as PlotTreeEntry;
        SelectedTree = tree;
    }

    protected override void OnInitialize(IDictionary<string, object> parameters)
    {
        if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        var plotID = parameters.GetValueOrDefault<string>(NavParams.PlotID);
        var unitCode = parameters.GetValueOrDefault<string>(NavParams.UNIT);
        var plotNumber = parameters.GetValueOrDefault<int>(NavParams.PLOT_NUMBER);

        if (string.IsNullOrWhiteSpace(plotID) == false)
        {
            Plot = PlotDataservice.GetPlot(plotID);
        }
        else
        {
            Plot = PlotDataservice.GetPlot(unitCode, plotNumber);
        }
    }

    public async Task TallyAsync(TallyPopulation_Plot pop)
    {
        if (!pop.InCruise) { return; }
        if (pop.IsEmpty)
        {
            await DialogService.ShowMessageAsync("To tally trees, goto plot edit page and uncheck stratum as NULL", "Stratum Is Marked As NULL");
            return;
        }

        var tree = await TallyService.TallyAsync(pop, UnitCode, PlotNumber);
        if (tree == null) { return; }
        ISoundService soundService = SoundService;

        soundService.SignalTallyAsync().FireAndForget();
        if (tree.CountOrMeasure == "M")
        {
            soundService.SignalMeasureTreeAsync().FireAndForget();
        }
        else if (tree.CountOrMeasure == "I")
        {
            soundService.SignalInsuranceTreeAsync().FireAndForget();
        }

        if (Trees.IsSuccessfullyCompleted)
        {
            Trees.Result.Add(tree);
        }

        pop.PlotTreeCount++;
        pop.TreeCount++;

        if (tree.CountOrMeasure == "M" && CruisersDataservice.PromptCruiserOnSample)
        {
            var cruiser = await DialogService.AskCruiserAsync();
            if (cruiser != null)
            {
                TreeDataservice.UpdateTreeInitials(tree.TreeID, cruiser);
            }
        }
    }

    public Task ShowEditTree(string treeID)
    {
        return NavigationService.ShowTreeEdit(treeID);
    }

    public Task ShowEditPlot()
    {
        return NavigationService.ShowPlotEdit(Plot.PlotID);
    }

    private Task ShowTallyPopulationDetails(TallyPopulation tallyPop)
    {
        if (tallyPop is null) { throw new ArgumentNullException(nameof(tallyPop)); }

        return NavigationService.ShowTallyPopulationInfo(UnitCode, PlotNumber, tallyPop.StratumCode, tallyPop.SampleGroupCode, tallyPop.SpeciesCode, tallyPop.LiveDead);
    }

    [RelayCommand]
    public void DeleteTree(PlotTreeEntry? tree)
    {
        if (tree is null) return;

        var treeID = tree.TreeID;
        TreeDataservice.DeleteTree(treeID);

        if (Trees.IsSuccessfullyCompleted)
        {
            Trees.Result?.Remove(tree);
        }

        var pop = TallyPopulations?.FirstOrDefault(x => x.StratumCode == tree.StratumCode
                                                        && x.SampleGroupCode == tree.SampleGroupCode
                                                        && (String.IsNullOrEmpty(x.SpeciesCode) || x.SpeciesCode == tree.SpeciesCode)
                                                        && (String.IsNullOrEmpty(x.LiveDead) || x.LiveDead == tree.LiveDead));

        if (pop != null)
        {
            pop.PlotTreeCount = Math.Max(0, pop.PlotTreeCount--);
            pop.TreeCount = Math.Max(0, pop.TreeCount--);
        }

        if (SelectedTree != null && SelectedTree.TreeID == treeID)
        {
            SelectedTree = null;
        }
    }

    [RelayCommand]
    public void SelectPreviousTree()
    {
        var selectedTree = SelectedTree;
        if (selectedTree == null) { return; }

        if (!Trees?.IsSuccessfullyCompleted ?? false) { return; }
        var treesCollection = Trees.Result;

        var i = treesCollection.IndexOf(selectedTree);
        if (i == -1) { return; }
        if (i < 1) { return; }

        var prevTree = (SelectPrevNextTreeSkipsCountTrees) ?
            treesCollection.ReverseSearch(x => x.TreeID != null && x.CountOrMeasure != "C", i - 1)
            : treesCollection.ReverseSearch(x => x.TreeID != null, i - 1);
        if (prevTree != null)
        {
            SelectTree(prevTree);
        }
    }

    [RelayCommand]
    public void SelectNextTree()
    {
        var selectedTree = SelectedTree;
        if (selectedTree == null) { return; }

        if (!Trees.IsSuccessfullyCompleted) { return; }
        var treesCollection = Trees.Result;

        var i = treesCollection.IndexOf(selectedTree);
        if (i == -1) { return; }
        if (i == treesCollection.Count - 1) { return; }

        var nextTree = (SelectPrevNextTreeSkipsCountTrees) ?
            treesCollection.Search(x => x.TreeID != null && x.CountOrMeasure != "C", i + 1)
            : treesCollection.Search(x => x.TreeID != null, i + 1);
        if (nextTree != null)
        {
            SelectTree(nextTree);
        }
    }

    [RelayCommand]
    public void SetStratumFilter(Plot_Stratum? stratum)
    {
        if (stratum == null)
        {
            TalliesFiltered = TallyPopulations.ToArray();
        }
        else
        {
            TalliesFiltered = TallyPopulations.Where(x => x.StratumCode == stratum.StratumCode).ToArray();
        }
    }
}