using CruiseDAL.Schema;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Util;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeListViewModel : XamarinViewModelBase
    {
        private ICommand _deleteTreeCommand;
        private Command _editTreeCommand;
        private ICollection<Tree_Ex> _allTrees;
        private ICommand _addTreeCommand;
        private ICommand _showLogsCommand;
        private string _unitCode;
        private bool _onlyShowTreesWithErrorsOrWarnings;
        private IEnumerable<TreeField> _treeFields;
        private CuttingUnit _cuttingUnit;

        public string Title => $"Unit {UnitCode} - {CuttingUnit?.Description} Trees";

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            protected set => SetProperty(ref _treeFields, value);
        }

        public ICollection<Tree_Ex> AllTrees
        {
            get { return _allTrees; }
            protected set
            {
                SetProperty(ref _allTrees, value);
                RaisePropertyChanged(nameof(Trees));
            }
        }

        public bool OnlyShowTreesWithErrorsOrWarnings
        {
            get => _onlyShowTreesWithErrorsOrWarnings;
            set
            {
                SetProperty(ref _onlyShowTreesWithErrorsOrWarnings, value);
                RaisePropertyChanged(nameof(Trees));
            }
        }

        public IEnumerable<Tree_Ex> Trees
        {
            get
            {
                if(OnlyShowTreesWithErrorsOrWarnings)
                { return AllTrees.Where(x => x.ErrorCount > 0 || x.WarningCount > 0); }
                else
                { return AllTrees; }
            }
        }

        public string[] StratumCodes { get; set; }

        public ICommand AddTreeCommand => _addTreeCommand ??= new Command(() => AddTree().FireAndForget());
        public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new Command((t) => DeleteTree(t as Tree_Ex).FireAndForget());
        public ICommand EditTreeCommand => _editTreeCommand ??= new Command((t) => ShowEditTree(t as Tree_Ex).FireAndForget());
        public ICommand ShowLogsCommand => _showLogsCommand ??= new Command((t) => ShowLogsAsync(t as Tree_Ex).FireAndForget());

        public string UnitCode => CuttingUnit?.CuttingUnitCode;

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

        protected ICuttingUnitDataservice CuttingUnitDatastore { get; }
        protected ITreeDataservice TreeDataservice { get; }
        protected ICruiseDialogService DialogService { get; }
        protected ICruiseNavigationService NavigationService { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }

        public event EventHandler TreeAdded;

        public TreeListViewModel(
            ICruiseDialogService dialogService,
            ICruiseNavigationService navigationService,
            ICuttingUnitDataservice cuttingUnitDatastore,
            ITreeDataservice treeDataservice,
            ITreeFieldDataservice treeFieldDataservice)
        {
            CuttingUnitDatastore = cuttingUnitDatastore ?? throw new ArgumentNullException(nameof(cuttingUnitDatastore));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var cuttingUnit = CuttingUnit = CuttingUnitDatastore.GetUnit(unitCode);

            if (IsLoaded is false)
            {
                TreeFields = TreeFieldDataservice.GetNonPlotTreeFields(unitCode);
                StratumCodes = CuttingUnitDatastore.GetStrataProxiesByUnitCode(unitCode).Select(x => x.StratumCode).ToArray();
            }
            AllTrees = TreeDataservice.GetTreesByUnitCode(unitCode).ToObservableCollection();
        }

        public async Task AddTree()
        {
            var stratumCode = await DialogService.AskValueAsync("Select Stratum", StratumCodes);

            if (stratumCode != null)
            {
                var sampleGroups = CuttingUnitDatastore.GetSampleGroupCodes(stratumCode).OrEmpty()
                    .ToArray();

                var sampleGroupCode = await DialogService.AskValueAsync("Select Sample Group", sampleGroups);

                if (sampleGroupCode != null)
                {
                    var cruiseMethod = CuttingUnitDatastore.GetCruiseMethod(stratumCode);
                    if (cruiseMethod == CruiseMethods.THREEP)
                    {
                        var sg = CuttingUnitDatastore.GetSampleGroup(stratumCode, sampleGroupCode);

                        var defaultLD = sg.DefaultLiveDead;
                        var subPops = CuttingUnitDatastore.GetSubPopulations(stratumCode, sampleGroupCode)
                            .ToDictionary(x => x.SpeciesCode + ((x.LiveDead != defaultLD) ? $" ({x.LiveDead})" : ""));

                        var subPopAlias = subPops.Keys.ToArray();
                        var selectedSubPopAlias = await DialogService.AskValueAsync("Select Sub-Population", subPopAlias);
                        if (subPops.TryGetValue(selectedSubPopAlias, out var selectedSubPop))
                        {
                            var kpi = await DialogService.AskKPIAsync((int)sg.MaxKPI, (int)sg.MinKPI);
                            if (kpi is null) { return; }

                            var isSTM = kpi is -1;
                            var tree_guid = TreeDataservice.InsertManualTree(UnitCode,
                                    stratumCode,
                                    sampleGroupCode: sampleGroupCode,
                                    species: selectedSubPop.SpeciesCode,
                                    liveDead: selectedSubPop.LiveDead,
                                    treeCount: 0,
                                    kpi: isSTM ? 0 : kpi.Value,
                                    stm: isSTM);
                            var newTree = TreeDataservice.GetTree(tree_guid);
                            AllTrees.Add(newTree);
                        }
                    }
                    else
                    {
                        var tree_guid = TreeDataservice.InsertManualTree(UnitCode, stratumCode, sampleGroupCode, treeCount: 0);
                        var newTree = TreeDataservice.GetTree(tree_guid);

                        AllTrees.Add(newTree);
                    }
                    OnTreeAdded(null);
                }
            }
        }

        public void OnTreeAdded(EventArgs e)
        {
            //reset OnlyShowTreesWithErrorsOrWarnings value to ensure new tree is shown
            OnlyShowTreesWithErrorsOrWarnings = false;
            RaisePropertyChanged(nameof(Trees));

            TreeAdded?.Invoke(this, e);
        }

        public Task ShowEditTree(Tree_Ex tree)
        {
            if (tree == null) { return Task.CompletedTask; }
            return NavigationService.ShowTreeEdit(tree.TreeID);
        }

        private async Task DeleteTree(Tree_Ex tree)
        {
            if (tree == null) { return; }

            var result = await DialogService.AskValueAsync("Delete Tree?", "yes", "no");
            if (result is "yes")
            {
                TreeDataservice.DeleteTree(tree.TreeID);
                AllTrees.Remove(tree);
                RaisePropertyChanged(nameof(Trees));
            }
            
        }

        public Task ShowLogsAsync(Tree_Ex tree)
        {
            return NavigationService.ShowLogsList(tree.TreeID);
        }
    }
}