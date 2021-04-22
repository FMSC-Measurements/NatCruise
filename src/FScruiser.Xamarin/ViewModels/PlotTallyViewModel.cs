using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using NatCruise.Util;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotTallyViewModel : XamarinViewModelBase
    {
        private const string STRATUM_FILTER_ALL = "All";

        private int _plotNumber;
        private string _unitCode;
        private ICollection<TallyPopulation_Plot> _tallyPopulations;
        private ICollection<StratumProxy> _strata;
        private string _stratumFilter = STRATUM_FILTER_ALL;
        private ICollection<TreeStub_Plot> _trees;
        private ICommand _tallyCommand;
        private Command _editPlotCommand;
        private Plot _plot;
        private ICommand _editTreeCommand;
        private ICommand _deleteTreeCommand;
        private TreeEditViewModel _selectedTreeViewModel;

        public event EventHandler TreeAdded;

        public ICuttingUnitDatastore Dataservice { get; }
        public ICruiseDialogService DialogService { get; }
        public ICruiseNavigationService NavigationService { get; }

        public ISampleSelectorDataService SampleSelectorDataService { get; private set; }
        public ITallySettingsDataService TallySettings { get; private set; }
        public ISoundService SoundService { get; private set; }
        public IContainerProvider ContainerProvider { get; }
        public IPlotTallyService TallyService { get; }

        public TreeEditViewModel SelectedTreeViewModel
        {
            get => _selectedTreeViewModel;
            protected set
            {
                SetProperty(ref _selectedTreeViewModel, value);
            }
        }

        public PlotTallyViewModel(ICruiseNavigationService navigationService,
            ICruiseDialogService dialogService,
            IDataserviceProvider datastoreProvider,
            ISoundService soundService,
            ITallySettingsDataService tallySettings,
            IContainerProvider containerProvider,
            IPlotTallyService tallyService)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            Dataservice = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            SampleSelectorDataService = datastoreProvider.GetDataservice<ISampleSelectorDataService>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            TallySettings = tallySettings ?? throw new ArgumentNullException(nameof(tallySettings));
            SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            TallyService = tallyService ?? throw new ArgumentNullException(nameof(tallyService));
        }

        public ICommand SelectTreeCommand => new Command<object>(SelectTree);

        public ICommand EditTreeCommand => _editTreeCommand
    ?? (_editTreeCommand = new Command<string>(ShowEditTree));

        public ICommand DeleteTreeCommand => _deleteTreeCommand
            ?? (_deleteTreeCommand = new Command<string>(DeleteTree));

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation_Plot>(async (x) => await this.TallyAsync(x)));

        public ICommand EditPlotCommand => _editPlotCommand
            ?? (_editPlotCommand = new Command(() => ShowEditPlot()));

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

        public string UnitCode
        {
            get { return _unitCode; }
            set { SetProperty(ref _unitCode, value); }
        }

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

        public ICollection<TallyPopulation_Plot> TalliesFiltered => TallyPopulations.OrEmpty()
            .Where(x => StratumFilter == STRATUM_FILTER_ALL || x.StratumCode == StratumFilter).ToArray();

        public ICollection<StratumProxy> Strata
        {
            get { return _strata; }
            set
            {
                SetProperty(ref _strata, value);
                RaisePropertyChanged(nameof(StrataFilterOptions));
            }
        }

        public ICollection<string> StrataFilterOptions => Strata.OrEmpty().Select(x => x.StratumCode).Append(STRATUM_FILTER_ALL).ToArray();

        public string StratumFilter
        {
            get { return _stratumFilter; }
            set
            {
                SetProperty(ref _stratumFilter, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public ICollection<TreeStub_Plot> Trees
        {
            get { return _trees; }
            set { SetProperty(ref _trees, value); }
        }

        protected void OnTreeAdded()
        {
            TreeAdded?.Invoke(this, null);
        }

        public void SelectTree(object obj)
        {
            var tree = obj as TreeStub_Plot;
            if (tree == null) { return; }
            var treeID = tree?.TreeID;
            if (treeID != null)
            {
                var treeVM = ContainerProvider.Resolve<TreeEditViewModel>();
                treeVM.UseSimplifiedTreeFields = true;
                treeVM.Initialize(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, treeID } });
                treeVM.Load();

                SelectedTreeViewModel = treeVM;
            }
            else
            {
                SelectedTreeViewModel = null;
            }
        }

        protected override void Load(IParameters parameters)
        {
            var plotID = parameters.GetValue<string>(NavParams.PlotID);

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            Plot plot = null;
            if (string.IsNullOrWhiteSpace(plotID) == false)
            {
                plot = Dataservice.GetPlot(plotID);
            }
            else
            {
                plot = Dataservice.GetPlot(unitCode, plotNumber);
            }

            TallyPopulations = Dataservice.GetPlotTallyPopulationsByUnitCode(plot.CuttingUnitCode, plot.PlotNumber).ToArray();
            Strata = Dataservice.GetPlotStrataProxies(plot.CuttingUnitCode).ToArray();
            Trees = Dataservice.GetPlotTreeProxies(plot.CuttingUnitCode, plot.PlotNumber).ToObservableCollection();

            Plot = plot;
            PlotNumber = plot.PlotNumber;
            UnitCode = plot.CuttingUnitCode;

            // refresh selected tree incase coming back from TreeEdit page
            RaisePropertyChanged(nameof(SelectedTreeViewModel));
        }

        public async Task TallyAsync(TallyPopulation_Plot pop)
        {
            if (pop.InCruise == false) { return; }

            ICruiseDialogService dialogService = DialogService;

            if (pop.IsEmpty)
            {
                await dialogService.ShowMessageAsync("To tally trees, goto plot edit page and unmark stratum as empty", "Stratum Is Marked As Empty");
                return;
            }

            var tree = await TallyService.TallyAsync(pop, UnitCode, PlotNumber);
            if(tree == null) { return; }

            _trees.Add(tree);
            OnTreeAdded();

            await HandleTally(pop, tree);
        }

        public void ShowEditTree(string treeID)
        {
            //NavigationService.NavigateAsync("Tree", new NavigationParameters() { { NavParams.TreeID, treeID } });
            NavigationService.ShowTreeEdit(treeID);
        }

        public async Task HandleTally(TallyPopulation_Plot population,
           TreeStub_Plot tree)
        {
            if (tree == null) { throw new ArgumentNullException(nameof(tree)); }
            ICruiseDialogService dialogService = DialogService;
            ITallySettingsDataService tallySettings = TallySettings;
            ISoundService soundService = SoundService;

            await soundService.SignalTallyAsync();
            if (tree.CountOrMeasure == "M")
            {
                await soundService.SignalMeasureTreeAsync();

                //if (tallySettings.EnableCruiserPopup)
                //{
                //    await dialogService.AskCruiserAsync(tree);
                //}
            }
            else if (tree.CountOrMeasure == "I")
            {
                await soundService.SignalInsuranceTreeAsync();
            }
        }

        public void ShowEditPlot()
        {
            //NavigationService.NavigateAsync("PlotEdit", new NavigationParameters($"{NavParams.PlotID}={Plot.PlotID}"));
            NavigationService.ShowPlotEdit(Plot.PlotID);
        }

        public void DeleteTree(string tree_guid)
        {
            Dataservice.DeleteTree(tree_guid);
            var tree = Trees.Where(x => x.TreeID == tree_guid).Single();
            Trees.Remove(tree);
        }

        public void DeleteTree(TreeStub_Plot tree)
        {
            Dataservice.DeleteTree(tree.TreeID);
            Trees.Remove(tree);
        }
    }
}