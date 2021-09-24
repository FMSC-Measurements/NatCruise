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
        private Command _addTreeCommand;
        private Command<TreeStub> _showLogsCommand;
        private string _unitCode;
        private bool _onlyShowTreesWithErrorsOrWarnings;
        private IEnumerable<TreeField> _treeFields;

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

        public IEnumerable<Tree_Ex> Trees => AllTrees?.Where(x => !OnlyShowTreesWithErrorsOrWarnings || x.ErrorCount > 0 || x.WarningCount > 0);

        public string[] StratumCodes { get; set; }

        public ICommand AddTreeCommand => _addTreeCommand ??= new Command(AddTreeAsync);

        public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new Command<TreeStub>(DeleteTree);

        public ICommand EditTreeCommand => _editTreeCommand ??= new Command((tree) =>
                    {
                        if (tree != null) { ShowEditTree((Tree_Ex)tree); }
                    });

        public ICommand ShowLogsCommand => _showLogsCommand ??= new Command<TreeStub>((tree) =>
                    {
                        if (tree != null) { ShowLogsAsync(tree); }
                    });

        public string UnitCode
        {
            get => _unitCode;
            set => SetProperty(ref _unitCode, value);
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

            var unitCode = UnitCode = parameters.GetValue<string>(NavParams.UNIT);

            if (IsLoaded is false)
            {
                TreeFields = TreeFieldDataservice.GetNonPlotTreeFields(unitCode);
                StratumCodes = CuttingUnitDatastore.GetStrataProxiesByUnitCode(UnitCode).Select(x => x.StratumCode).ToArray();
            }
            AllTrees = TreeDataservice.GetTreesByUnitCode(unitCode).ToObservableCollection();
        }

        public async void AddTreeAsync()
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
                            var kpi = await DialogService.AskKPIAsync((int)sg.MinKPI, (int)sg.MaxKPI);
                            if (kpi is null) { return; }

                            var tree_guid = TreeDataservice.InsertManualTree(UnitCode,
                                stratumCode,
                                sampleGroupCode: sampleGroupCode,
                                species: selectedSubPop.SpeciesCode,
                                liveDead: selectedSubPop.LiveDead,
                                kpi: kpi.Value);
                            var newTree = TreeDataservice.GetTree(tree_guid);

                            AllTrees.Add(newTree);
                        }
                    }
                    else
                    {
                        var tree_guid = TreeDataservice.InsertManualTree(UnitCode, stratumCode, sampleGroupCode);
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

        public void ShowEditTree(Tree_Ex tree)
        {
            //NavigationService.ShowPrivacyPolicy();
            NavigationService.ShowTreeEdit(tree.TreeID);
        }

        private void DeleteTree(TreeStub tree)
        {
            if (tree == null) { return; }

            DialogService.AskCancelAsync("Delete Tree?", "", true);
            TreeDataservice.DeleteTree(tree.TreeID);
        }

        public Task ShowLogsAsync(TreeStub tree)
        {
            return NavigationService.ShowLogsList(tree.TreeID);
        }
    }
}