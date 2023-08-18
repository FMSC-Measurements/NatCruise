using FScruiser.XF.Services;
using NatCruise.Async;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotTallyViewModel : ViewModelBase
    {
        private const string STRATUM_FILTER_ALL = "All";

        private int _plotNumber;
        private ICollection<TallyPopulation_Plot> _tallyPopulations;
        private ICollection<Stratum> _strata;
        private string _stratumFilter = STRATUM_FILTER_ALL;
        private IList<PlotTreeEntry> _trees;

        private Plot _plot;

        private TreeEditViewModel _selectedTreeViewModel;
        private CuttingUnit _cuttingUnit;
        private PlotTreeEntry _selectedTree;

        public event EventHandler TreeAdded;

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
        public IContainerProvider ContainerProvider { get; }
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
            IContainerProvider containerProvider,
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
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            TallyService = tallyService ?? throw new ArgumentNullException(nameof(tallyService));
            PlotTreeDataservice = plotTreeDataservice ?? throw new ArgumentNullException(nameof(plotTreeDataservice));
        }

        #region commands

        private ICommand _editPlotCommand;
        private ICommand _tallyCommand;
        private ICommand _editTreeCommand;
        private ICommand _deleteTreeCommand;
        private ICommand _selectPreviouseTreeCommand;
        private ICommand _selectNextTreeCommand;
        private IEnumerable<string> _stratumFilterOptions;
        private ICommand _showTallyPopulationDetailsCommand;
        private ICommand _clearStratumFilter;
        private ICommand _setStratumFilterCommand;
        private Plot_Stratum[] _plotStrata;
        private ICollection<TallyPopulation_Plot> _talliesFiltered;

        public ICommand SelectTreeCommand => new Command<object>(SelectTree);

        public ICommand EditTreeCommand => _editTreeCommand ??= new Command<string>(x => ShowEditTree(x).FireAndForget());

        public ICommand ShowTallyPopulationDetailsCommand => _showTallyPopulationDetailsCommand ??= new Command<TallyPopulation>(x => ShowTallyPopulationDetails(x).FireAndForget());

        public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new Command<string>(DeleteTree);

        public ICommand TallyCommand => _tallyCommand ??= new Command<TallyPopulation_Plot>(x => TallyAsync(x).FireAndForget());

        public ICommand EditPlotCommand => _editPlotCommand ??= new Command(() => ShowEditPlot());

        public ICommand SelectPreviousTreeCommand => _selectPreviouseTreeCommand ??= new DelegateCommand(SelectPreviousTree);

        public ICommand SelectNextTreeCommand => _selectNextTreeCommand ??= new DelegateCommand(SelectNextTree);

        public ICommand SetStratumFilterCommand => _setStratumFilterCommand ??= new DelegateCommand<Plot_Stratum>(SetStratumFilter);

        #endregion commands

        public bool SelectPrevNextTreeSkipsCountTrees { get; set; }

        public PlotTreeEntry SelectedTree
        {
            get => _selectedTree;
            set
            {
                if (object.ReferenceEquals(_selectedTree, value)) return;

                var treeID = value?.TreeID;
                if (treeID != null)
                {
                    var treeVM = ContainerProvider.Resolve<TreeEditViewModel>((typeof(ICruiseNavigationService), NavigationService));
                    treeVM.UseSimplifiedTreeFields = true;
                    treeVM.Initialize(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, treeID } });
                    treeVM.Load();

                    SetProperty(ref _selectedTree, value);
                    SelectedTreeViewModel = treeVM;
                }
                else
                {
                    SetProperty(ref _selectedTree, null);
                    SelectedTreeViewModel = null;
                }
            }
        }

        public TreeEditViewModel SelectedTreeViewModel
        {
            get => _selectedTreeViewModel;
            protected set
            {
                if (_selectedTreeViewModel != null)
                { _selectedTreeViewModel.PropertyChanged -= SelectedTreeViewModel_PropertyChanged; }
                SetProperty(ref _selectedTreeViewModel, value);
                if (value != null)
                { value.PropertyChanged += SelectedTreeViewModel_PropertyChanged; }
            }
        }

        private void SelectedTreeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (TreeEditViewModel)sender;
            var tree = SelectedTree;
            if (tree == null) return;
            if (e.PropertyName == nameof(TreeEditViewModel.ErrorsAndWarnings))
            {
                PlotTreeDataservice.RefreshErrorsAndWarnings(tree);
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

        public Plot Plot
        {
            get => _plot;
            set => SetProperty(ref _plot, value);
        }

        public int PlotNumber
        {
            get { return _plotNumber; }
            set { SetProperty(ref _plotNumber, value); }
        }

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            protected set
            {
                SetProperty(ref _cuttingUnit, value);
                RaisePropertyChanged(nameof(UnitCode));
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string UnitCode => CuttingUnit?.CuttingUnitCode;

        public bool IsRecon { get; private set; }

        public ICollection<TallyPopulation_Plot> TallyPopulations
        {
            get { return _tallyPopulations; }
            set
            {
                SetProperty(ref _tallyPopulations, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public ICollection<TallyPopulation_Plot> TalliesFiltered
        {
            get => _talliesFiltered;
            private set => SetProperty(ref _talliesFiltered, value);
        }

        public ICollection<Stratum> Strata
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

        public string StratumFilter
        {
            get { return _stratumFilter; }
            set
            {
                SetProperty(ref _stratumFilter, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public IList<PlotTreeEntry> Trees
        {
            get { return _trees; }
            set
            {
                SetProperty(ref _trees, value);
                // if we have a selected tree refresh it with a matching tree in the new trees collection
                var selectedTree = SelectedTree;
                if (selectedTree != null)
                {
                    SelectedTree = Trees.FirstOrDefault(x => x.TreeID == selectedTree.TreeID);
                }
            }
        }

        public Plot_Stratum[] PlotStrata
        {
            get => _plotStrata;
            private set => SetProperty(ref _plotStrata, value);
        }

        protected void OnTreeAdded()
        {
            TreeAdded?.Invoke(this, null);
        }

        public void SelectTree(object obj)
        {
            var tree = obj as PlotTreeEntry;
            SelectedTree = tree;
        }

        protected override void Load(IParameters parameters)
        {
            //if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            //var plotID = parameters.GetValue<string>(NavParams.PlotID);
            //var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            //var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            //Plot plot = null;
            //if (string.IsNullOrWhiteSpace(plotID) == false)
            //{
            //    plot = PlotDataservice.GetPlot(plotID);
            //    unitCode = plot.CuttingUnitCode;
            //}
            //else
            //{
            //    plot = PlotDataservice.GetPlot(unitCode, plotNumber);
            //}

            //var cuttingUnit = CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);

            //TallyPopulations = TallyPopulationDataservice.GetPlotTallyPopulationsByUnitCode(plot.CuttingUnitCode, plot.PlotNumber).ToArray();
            //SetStratumFilter(null);
            // PlotStrata = PlotStratumDataservice.GetPlot_Strata(plot.CuttingUnitCode, plot.PlotNumber, false).ToArray();


            //Trees = PlotTreeDataservice.GetPlotTrees(plot.CuttingUnitCode, plot.PlotNumber).ToObservableCollection();

            //Plot = plot;
            //PlotNumber = plot.PlotNumber;

            //RaisePropertyChanged(nameof(Title));

            RunLoadAsync(parameters).FireAndForget();
        }

        public Task RunLoadAsync(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var plotID = parameters.GetValue<string>(NavParams.PlotID);
            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            return Task.Factory.StartNew(() =>
            {
                Plot plot = null;
                if (!string.IsNullOrWhiteSpace(plotID))
                {
                    plot = PlotDataservice.GetPlot(plotID);
                    unitCode = plot.CuttingUnitCode;
                }
                else
                {
                    plot = PlotDataservice.GetPlot(unitCode, plotNumber);
                }

                CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);

                TallyPopulations = TallyPopulationDataservice.GetPlotTallyPopulationsByUnitCode(plot.CuttingUnitCode, plot.PlotNumber).ToArray();
                SetStratumFilter(null);
                PlotStrata = PlotStratumDataservice.GetPlot_Strata(plot.CuttingUnitCode, plot.PlotNumber, false).ToArray();


                Trees = PlotTreeDataservice.GetPlotTrees(plot.CuttingUnitCode, plot.PlotNumber).ToObservableCollection();

                Plot = plot;
                PlotNumber = plot.PlotNumber;

                RaisePropertyChanged(nameof(Title));
            }

            );
        }


        public async Task TallyAsync(TallyPopulation_Plot pop)
        {
            if (pop.InCruise == false) { return; }

            var dialogService = DialogService;

            if (pop.IsEmpty)
            {
                await dialogService.ShowMessageAsync("To tally trees, goto plot edit page and uncheck stratum as empty", "Stratum Is Marked As Empty");
                return;
            }

            var tree = await TallyService.TallyAsync(pop, UnitCode, PlotNumber);
            if (tree == null) { return; }
            ISoundService soundService = SoundService;

            soundService.SignalTallyAsync().FireAndForget();

            _trees.Add(tree);
            OnTreeAdded();

            if (tree.CountOrMeasure == "M")
            {
                soundService.SignalMeasureTreeAsync().FireAndForget();

                if (CruisersDataservice.PromptCruiserOnSample)
                {
                    var cruiser = await DialogService.AskCruiserAsync();
                    if (cruiser != null)
                    {
                        TreeDataservice.UpdateTreeInitials(tree.TreeID, cruiser);
                    }
                }
            }
            else if (tree.CountOrMeasure == "I")
            {
                soundService.SignalInsuranceTreeAsync().FireAndForget();
            }
            pop.PlotTreeCount++;
            pop.TreeCount++;
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

        public void DeleteTree(string tree_guid)
        {
            var tree = Trees.Single(x => x.TreeID == tree_guid);
            DeleteTree(tree);
        }

        public void DeleteTree(PlotTreeEntry tree)
        {
            var treeID = tree.TreeID;
            TreeDataservice.DeleteTree(treeID);
            Trees.Remove(tree);

            var pop = TallyPopulations.FirstOrDefault(x => x.StratumCode == tree.StratumCode
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

        public void SelectPreviousTree()
        {
            var selectedTree = SelectedTree;
            if (selectedTree == null) { return; }

            var trees = Trees;
            var i = trees.IndexOf(selectedTree);
            if (i == -1) { return; }
            if (i < 1) { return; }

            var prevTree = (SelectPrevNextTreeSkipsCountTrees) ?
                trees.ReverseSearch(x => x.TreeID != null && x.CountOrMeasure != "C", i - 1)
                : trees.ReverseSearch(x => x.TreeID != null, i - 1);
            if (prevTree != null)
            {
                SelectTree(prevTree);
            }
        }

        public void SelectNextTree()
        {
            var selectedTree = SelectedTree;
            if (selectedTree == null) { return; }

            var trees = Trees;
            var i = trees.IndexOf(selectedTree);
            if (i == -1) { return; }
            if (i == trees.Count - 1) { return; }

            var nextTree = (SelectPrevNextTreeSkipsCountTrees) ?
                trees.Search(x => x.TreeID != null && x.CountOrMeasure != "C", i + 1)
                : trees.Search(x => x.TreeID != null, i + 1);
            if (nextTree != null)
            {
                SelectTree(nextTree);
            }
        }

        public void SetStratumFilter(Plot_Stratum stratum)
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
}