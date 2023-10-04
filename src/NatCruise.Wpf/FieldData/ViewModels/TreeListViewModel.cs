using CruiseDAL.Schema;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TreeListViewModel : ViewModelBase
    {
        private Dictionary<string, PropertyInfo> TreeProperties { get; }
        private IEnumerable<TreeEx> _trees;
        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<TreeField> _fields;
        private int? _plotNumber;
        private TreeEx _selectedTree;
        private TreeEditViewModel _treeEditViewModel;
        private ICommand _addTreeCommand;
        private ICommand _deleteTreeCommand;

        private readonly IEnumerable<TreeField> COMMON_TREEFIELDS = new[]
        {
            new TreeField{DbType = "TEXT", Heading = "Cutting Unit", Field = nameof(Tree.CuttingUnitCode)},
            new TreeField{DbType = "TEXT", Heading = "Plot Number", Field = nameof(Tree.PlotNumber)},
            new TreeField{DbType = "TEXT", Heading = "Tree Number", Field = nameof(Tree.TreeNumber)},
            new TreeField{DbType = "TEXT", Heading = "Stratum", Field = nameof(Tree.StratumCode)},
            new TreeField{DbType = "TEXT", Heading = "Sample Group", Field = nameof(Tree.SampleGroupCode)},
            new TreeField{DbType = "TEXT", Heading = "Species", Field = nameof(Tree.SpeciesCode)},
            new TreeField{DbType = "TEXT", Heading = "Live/Dead", Field = nameof(Tree.LiveDead)},
            new TreeField{DbType = "TEXT", Heading = "Count/Measure", Field = nameof(Tree.CountOrMeasure)},
        };

        private readonly IEnumerable<TreeField> ThreePFields = new[]
        {
             new TreeField{DbType = TreeFieldTableDefinition.DBTYPE_INTEGER, Heading = "KPI", Field = nameof(TreeEx.KPI)},
             new TreeField{DbType = TreeFieldTableDefinition.DBTYPE_BOOLEAN, Heading = "STM", Field = nameof(TreeEx.STM)},
        };

        public static readonly IEnumerable<string> LOCKED_FIELDS = new[]
        {
            nameof(Tree.TreeID),
            nameof(Tree.CuttingUnitCode),
            nameof(Tree.PlotNumber),
            nameof(TreeEx.STM),
            nameof(TreeEx.KPI),
        };

        public TreeListViewModel(ITreeDataservice treeDataservice,
                                 IPlotTreeDataservice plotTreeDataservice,
                                 ITreeFieldDataservice treeFieldDataservice,
                                 ICuttingUnitDataservice cuttingUnitDataservice,
                                 IPlotDataservice plotDataservice,
                                 IStratumDataservice stratumDataservice,
                                 ISampleGroupDataservice sampleGroupDataservice,
                                 ISubpopulationDataservice subpopulationDataservice,
                                 INatCruiseDialogService natCruiseDialogService,
                                TreeEditViewModel treeEditViewModel,
                                LogListViewModel treeLogListViewModel)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            PlotTreeDataservice = plotTreeDataservice ?? throw new ArgumentNullException(nameof(plotTreeDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));

            DialogService = natCruiseDialogService ?? throw new ArgumentNullException(nameof(natCruiseDialogService));
            TreeEditViewModel = treeEditViewModel ?? throw new ArgumentNullException(nameof(treeEditViewModel));
            LogListViewModel = treeLogListViewModel ?? throw new ArgumentNullException(nameof(treeLogListViewModel));

            TreeProperties = typeof(TreeEx)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .ToDictionary(x => x.Name.ToLower());
        }

        public event EventHandler TreeAdded;

        public ICommand AddTreeCommand => _addTreeCommand ??= new DelegateCommand(() => AddTree().FireAndForget());
        public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new DelegateCommand(() => DeleteTree().FireAndForget());

        public ITreeDataservice TreeDataservice { get; }
        public IPlotTreeDataservice PlotTreeDataservice { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public IPlotDataservice PlotDataservice { get; }
        public IStratumDataservice StratumDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISubpopulationDataservice SubpopulationDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

        public LogListViewModel LogListViewModel { get; }

        public TreeEditViewModel TreeEditViewModel
        {
            get => _treeEditViewModel;
            private set
            {
                if (_treeEditViewModel != null) { _treeEditViewModel.TreeFieldValueChanged -= _treeEditViewModel_TreeValueChanged; }
                _treeEditViewModel = value;
                if (value != null) { _treeEditViewModel.TreeFieldValueChanged += _treeEditViewModel_TreeValueChanged; }
            }
        }

        private void _treeEditViewModel_TreeValueChanged(object sender, TreeFieldValueChangedEventArgs e)
        {
            var selectedTree = SelectedTree;
            if (selectedTree == null) { return; }

            // when a field value is updated in the TreeEditViewModel
            /// we want to reflect that change in the datagrid

            var field = e.Field.ToLowerInvariant();
            if (TreeProperties.TryGetValue(field, out var treeProp))
            {
                treeProp.SetValue(selectedTree, e.Value);
            }
        }

        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set
            {
                SetProperty(ref _cuttingUnitCode, value);
                Load();
            }
        }

        public string StratumCode
        {
            get => _stratumCode;
            set
            {
                SetProperty(ref _stratumCode, value);
                Load();
            }
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set
            {
                SetProperty(ref _sampleGroupCode, value);
                Load();
            }
        }

        public int? PlotNumber
        {
            get => _plotNumber;
            set
            {
                SetProperty(ref _plotNumber, value);
                Load();
            }
        }

        public IEnumerable<TreeEx> Trees
        {
            get => _trees;
            set
            {
                
                if (_trees != null)
                {
                    foreach (var tree in _trees)
                    { tree.PropertyChanged -= Tree_PropertyChanged; }
                }
                string selectedTreeID = SelectedTree?.TreeID;
                SetProperty(ref _trees, value);
                if (value != null)
                {
                    foreach (var tree in value)
                    { tree.PropertyChanged += Tree_PropertyChanged; }

                    if (selectedTreeID != null)
                    {
                        SelectedTree = value.FirstOrDefault(x => x.TreeID == selectedTreeID);
                    }
                }
            }
        }

        private void Tree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var tree = sender as TreeEx;
            //if (tree == null) { return; }
            //var propertyName = e.PropertyName;

            //if (LOCKED_FIELDS.Contains(propertyName)) { return; }

            //if (propertyName == nameof(Tree.TreeNumber))
            //{
            //    var newTreeNumber = tree.TreeNumber;
            //    var unit = tree.CuttingUnitCode;
            //    var plotNumber = tree.PlotNumber;
            //    if (TreeDataservice.IsTreeNumberAvalible(unit, newTreeNumber, plotNumber: plotNumber))
            //    {
            //        TreeDataservice.UpdateTree(tree);
            //    }
            //    else
            //    {
            //        var treeNumber = TreeDataservice.GetTreeNumber(tree.TreeID);
            //        tree.TreeNumber = treeNumber.Value;
            //        NatCruiseDialogService.ShowMessageAsync("Tree Number Already Exists");
            //    }
            //}
            //else
            //{
            //    TreeDataservice.UpdateTree(tree);
            //}
        }

        public TreeEx SelectedTree
        {
            get => _selectedTree;
            set
            {
                SetProperty(ref _selectedTree, value);
                TreeEditViewModel.Tree = value;
                if (value != null)
                {
                    LogListViewModel.TreeID = value.TreeID;
                }
            }
        }

        public IEnumerable<TreeField> Fields
        {
            get => _fields;
            protected set
            {
                SetProperty(ref _fields, value);
            }
        }

        public override void Load()
        {
            base.Load();

            var unitCode = CuttingUnitCode;

            var trees = TreeDataservice.GetTrees(cuttingUnitCode: unitCode,
                stratumCode: StratumCode,
                sampleGroupCode: SampleGroupCode,
                plotNumber: PlotNumber);
            Trees = trees;

            var fields = COMMON_TREEFIELDS.Concat(TreeFieldDataservice.GetTreeFieldsUsedInCruise());

            bool has3p = false;
            if(string.IsNullOrEmpty(unitCode))
            {
                has3p = StratumDataservice.GetStrata().Any(x => CruiseMethods.THREE_P_METHODS.Contains(x.Method));
            }
            else
            {
                has3p = CuttingUnitDataservice.GetCuttingUnitStrataSummary(unitCode).Methods.Any(x => CruiseMethods.THREE_P_METHODS.Contains(x));
            }

            if(has3p)
            {
                fields = fields.Concat(ThreePFields);
            }
            
            Fields = fields.ToArray();
        }

        public async Task AddTree()
        {
            var cuttingUnitCode = CuttingUnitCode;
            var stratumCode = StratumCode;
            var sampleGroupCode = SampleGroupCode;
            var plotNumber = PlotNumber;

            if (cuttingUnitCode is null)
            {
                DialogService.ShowMessageAsync("Please Select Cutting Unit First").FireAndForget();
                return;
            }

            if (stratumCode is null)
            {
                var strata = StratumDataservice.GetStratumCodes(cuttingUnitCode).ToArray();
                if (strata.Length > 1)
                {
                    var selection = await DialogService.AskValueAsync("Select Stratum", strata);
                    if (selection is null) return;

                    stratumCode = selection;
                }
                else { stratumCode = strata[0]; }
            }

            if (sampleGroupCode is null)
            {
                var sampleGroups = SampleGroupDataservice.GetSampleGroupCodes(stratumCode).ToArray();
                if (sampleGroups.Length > 1)
                {
                    var selection = await DialogService.AskValueAsync("Select Sample Group", sampleGroups);
                    if (selection is null) return;

                    sampleGroupCode = selection;
                }
                else { sampleGroupCode = sampleGroups[0]; }
            }

            var sg = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);

            var defaultLD = sg.DefaultLiveDead;
            var subPops = SubpopulationDataservice.GetSubpopulations(stratumCode, sampleGroupCode)
                .ToDictionary(x => x.SpeciesCode + ((x.LiveDead != defaultLD) ? $" ({x.LiveDead})" : ""));

            // if there is only one sub pop initialize selected subpop to is
            // otherwise we will need to ask the user which subpop
            Subpopulation selectedSubPop = (subPops.Count == 1) ? subPops.Values.First() : null;
            if (selectedSubPop is null)
            {
                var subPopAlias = subPops.Keys.ToArray();
                var selectedSubPopAlias = await DialogService.AskValueAsync("Select Sub-Population", subPopAlias);
                if (selectedSubPopAlias is null) return;
                subPops.TryGetValue(selectedSubPopAlias, out selectedSubPop);
            }

            string treeID = null;
            var cruiseMethod = StratumDataservice.GetCruiseMethod(stratumCode);
            if (CruiseMethods.PLOT_METHODS.Contains(cruiseMethod))
            {
                if (plotNumber is null)
                {
                    var plotNumbers = PlotDataservice.GetPlotNumbersOrdered(cuttingUnitCode, stratumCode)
                        .Select(x => x.ToString()).ToArray();
                    if(plotNumbers.Length is 0)
                    {
                        DialogService.ShowMessageAsync($"Unit Contains no Plots in Stratum {stratumCode}").FireAndForget();
                        return;
                    }
                    var result = await DialogService.AskValueAsync("Select Plot Number", plotNumbers);
                    if (result is null || !int.TryParse(result, out var iResult)) return;
                    plotNumber = iResult;
                }

                string countOrMeasure = "M";
                if (CruiseMethods.TALLY_METHODS.Contains(cruiseMethod))
                {
                    var result = await DialogService.AskValueAsync("Count, Measure or Insurance tree?", "Count", "Measure", "Insurance");
                    if (result is null) { return; }
                    countOrMeasure = result[0].ToString();
                }

                if (cruiseMethod == CruiseMethods.P3P || cruiseMethod == CruiseMethods.F3P)
                {
                    var kpi = await DialogService.AskKPIAsync(sg.MaxKPI, sg.MinKPI);
                    if (kpi is null) { return; }
                    var isSTM = kpi is -1;
                    PlotTreeDataservice.CreatePlotTree(cuttingUnitCode,
                        plotNumber.Value,
                        stratumCode,
                        sampleGroupCode,
                        selectedSubPop.SpeciesCode,
                        selectedSubPop.LiveDead,
                        countMeasure: countOrMeasure,
                        kpi: isSTM ? 0 : kpi.Value,
                        stm: isSTM);
                }
                else
                {
                    PlotTreeDataservice.CreatePlotTree(cuttingUnitCode,
                        plotNumber.Value,
                        stratumCode,
                        sampleGroupCode,
                        selectedSubPop.SpeciesCode,
                        selectedSubPop.LiveDead,
                        countMeasure: countOrMeasure);
                }
            }
            else if (cruiseMethod == CruiseMethods.THREEP || cruiseMethod == CruiseMethods.S3P)
            {
                var kpi = await DialogService.AskKPIAsync(sg.MaxKPI, sg.MinKPI);
                if (kpi is null) { return; }

                var isSTM = kpi is -1;
                treeID = TreeDataservice.InsertManualTree(cuttingUnitCode,
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

                treeID = TreeDataservice.InsertManualTree(cuttingUnitCode,
                    stratumCode,
                    sampleGroupCode,
                    species: selectedSubPop.SpeciesCode,
                    liveDead: selectedSubPop.LiveDead,
                    treeCount: treeCount);
            }

            OnTreeAdded(treeID);
        }

        protected void OnTreeAdded(string treeID)
        {
            Load();
            var tree = Trees.FirstOrDefault(x => x.TreeID == treeID);
            SelectedTree = tree;

            TreeAdded?.Invoke(this, EventArgs.Empty);
        }

        public Task DeleteTree()
        {
            var tree = SelectedTree;
            if (tree is null) return Task.CompletedTask;

            return DeleteTree(tree);
        }

        public async Task DeleteTree(Tree tree)
        {
            if (tree == null) { return; }

            var result = await DialogService.AskValueAsync("Delete Tree?", "yes", "no");
            if (result is "yes")
            {
                TreeDataservice.DeleteTree(tree.TreeID);
                Load();
            }
        }
    }
}