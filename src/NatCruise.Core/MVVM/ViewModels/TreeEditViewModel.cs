using CommunityToolkit.Mvvm.ComponentModel;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class TreeFieldValueChangedEventArgs
    {
        public string Field { get; set; }
        public object Value { get; set; }
    }

    public class TreeEditViewModel : ViewModelBase
    {
        private ICommand _showLogsCommand;
        private IEnumerable<string> _stratumCodes;
        private IEnumerable<string> _sampleGroupCodes;
        private IEnumerable<TreeError> _errorsAndWarnings;
        private IEnumerable<TreeFieldValue> _treeFieldValues;
        private TreeEx _tree;
        private DelegateCommand<TreeError> _showEditTreeErrorCommand;
        private IEnumerable<string> _cruisers;
        private string _cruiseMethod;
        private IEnumerable<string> _countOrMeasureOptions;
        private IEnumerable<string> _speciesOptions;
        private int _errorCount;
        private int _warningCount;

        private bool IsTreeChanging { get; set; }

        public IStratumDataservice StratumDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISpeciesDataservice SpeciesDataservice { get; }
        public ISubpopulationDataservice SubpopulationDataservice { get; }
        public ITreeDataservice TreeDataservice { get; }
        public ITreeErrorDataservice TreeErrorDataservice { get; }
        public ITreeFieldValueDataservice TreeFieldValueDataservice { get; }
        public ICruisersDataservice CruisersDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public INatCruiseNavigationService NavigationService { get; }
        public ICruiseLogDataservice CruiseLogDataservice { get; }
        public ILoggingService LoggingService { get; }

        public event EventHandler<TreeFieldValueChangedEventArgs> TreeFieldValueChanged;

        public bool UseSimplifiedTreeFields { get; set; } = false;

        public IEnumerable<string> Cruisers
        {
            get => _cruisers;
            set => SetProperty(ref _cruisers, value);
        }

        public IEnumerable<TreeError> ErrorsAndWarnings
        {
            get => _errorsAndWarnings;
            set => SetProperty(ref _errorsAndWarnings, value);
        }

        public IEnumerable<TreeFieldValue> TreeFieldValues
        {
            get => _treeFieldValues;
            set
            {
                if (_treeFieldValues != null)
                {
                    foreach (var tfv in _treeFieldValues)
                    {
                        tfv.ValueChanged += treeFieldValue_ValueChanged;
                    }
                }
                SetProperty(ref _treeFieldValues, value);
                if (value != null)
                {
                    foreach (var tfv in value)
                    {
                        tfv.ValueChanged += treeFieldValue_ValueChanged;
                    }
                }
            }
        }

        private void treeFieldValue_ValueChanged(object sender, EventArgs e)
        {
            var treeFieldValue = (TreeFieldValue)sender;

            try
            {
                TreeFieldValueDataservice.UpdateTreeFieldValue(treeFieldValue);
                treeFieldValue.DbError = null;
                RefreshErrorsAndWarnings();

                // update value on Tree object to reflect change in Tree List View as well.
                //if (TreeProperties.TryGetValue(treeFieldValue.Field.ToLower(), out var treeProp))
                //{
                //    treeProp.SetValue(Tree, treeFieldValue.Value);
                //}

                var field = treeFieldValue.Field;
                TreeFieldValueChanged?.Invoke(this, new TreeFieldValueChangedEventArgs
                {
                    Field = field,
                    Value = treeFieldValue.Value,
                });
            }
            catch (FMSC.ORM.ConstraintException ex)
            {
                treeFieldValue.DbError = "Db Constraint Exception";
                LoggingService.LogException(nameof(TreeEditViewModel), "treeFieldValue_PropertyChanged", ex,
                    new Dictionary<string, string>()
                    {
                                { "Field", treeFieldValue.Field},
                                { "Value", treeFieldValue.Value?.ToString() ?? "null"}
                    });
            }
        }

        public string TreeID => Tree?.TreeID;

        public TreeEx Tree
        {
            get { return _tree; }
            set
            {
                IsTreeChanging = true;
                try
                {
                    OnTreeChanged(value);
                    SetProperty(ref _tree, value);
                    OnPropertyChanged(nameof(TreeID));
                    OnPropertyChanged(nameof(CountOrMeasure));
                    OnPropertyChanged(nameof(TreeNumber));
                    OnPropertyChanged(nameof(StratumCode));
                    OnPropertyChanged(nameof(SampleGroupCode));
                    //RaisePropertyChanged(nameof(SubPopulation));
                    OnPropertyChanged(nameof(SpeciesCode));
                    OnPropertyChanged(nameof(LiveDead));
                    OnPropertyChanged(nameof(Remarks));
                    OnPropertyChanged(nameof(Initials));
                    OnPropertyChanged(nameof(TreeCount));
                }
                finally
                {
                    IsTreeChanging = false;
                }
            }
        }

        private void OnTreeChanged(TreeEx tree)
        {
            if (tree != null)
            {
                var unitCode = tree.CuttingUnitCode;
                var stratumCodes = StratumDataservice.GetStratumCodesByUnit(unitCode);
                StratumCodes = stratumCodes;

                RefreshCruiseMethod(tree);
                RefreshSampleGroups(tree);
                RefreshSpeciesOptions(tree);
                RefreshTreeFieldValues(tree);
                RefreshErrorsAndWarnings(tree);
                RefreshCruisers(tree);

                var cruiseMethod = StratumDataservice.GetCruiseMethod(tree.StratumCode);
                if (CruiseDAL.Schema.CruiseMethods.PLOT_METHODS.Contains(cruiseMethod))
                {
                    CountOrMeasureOptions = new[] { "C", "M", "I" };
                }
                else if (cruiseMethod == CruiseDAL.Schema.CruiseMethods.FIXCNT)
                {
                    CountOrMeasureOptions = new[] { "C" };
                }
                else
                {
                    CountOrMeasureOptions = new[] { "M", "I" };
                }
            }
        }

        public string Initials
        {
            get => Tree?.Initials;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.Initials;
                if (oldValue != value)
                {
                    TreeDataservice.UpdateTreeInitials(tree.TreeID, value);
                    tree.Initials = value;
                }
            }
        }

        public string Remarks
        {
            get => Tree?.Remarks;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.Remarks;
                if (value != oldValue)
                {
                    TreeDataservice.UpdateTreeRemarks(tree.TreeID, value);
                    tree.Remarks = value;
                }
            }
        }

        public int TreeCount
        {
            get => (Tree != null) ? TreeDataservice.GetTreeCount(Tree.TreeID) : 0;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                TreeDataservice.UpdateTreeCount(tree.TreeID, value);
                OnPropertyChanged();
            }
        }

        public int ErrorCount
        {
            get => _errorCount;
            protected set => SetProperty(ref _errorCount, value);
        }

        public int WarningCount
        {
            get => _warningCount;
            protected set => SetProperty(ref _warningCount, value);
        }


        #region CountOrMeasure

        public string CountOrMeasure
        {
            get => Tree?.CountOrMeasure;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.CountOrMeasure;
                tree.CountOrMeasure = value;
                if (oldValue == value) { return; }
                SaveTree();

                CruiseLogDataservice.Log($"Tree.CountOrMeasure Changed |oldCM:{oldValue}|newCM:{value}|", treeID: tree.TreeID, fieldName:"CountOrMeasure", tableName:"Tree");
                RefreshErrorsAndWarnings();
                OnPropertyChanged();
            }
        }

        #endregion CountOrMeasure

        public IEnumerable<string> CountOrMeasureOptions
        {
            get => _countOrMeasureOptions;
            set => SetProperty(ref _countOrMeasureOptions, value);
        }

        #region TreeNumber

        public int TreeNumber
        {
            get
            {
                return Tree?.TreeNumber ?? 0;
            }
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.TreeNumber;
                if (OnTreeNumberChanging(tree, oldValue, value))
                {
                    Tree.TreeNumber = value;
                    OnTreeNumberChanged(oldValue, value);
                }
                OnPropertyChanged(nameof(TreeNumber));
            }
        }

        private void OnTreeNumberChanged(int oldValue, int value)
        {
            SaveTree();
        }

        private bool OnTreeNumberChanging(Tree tree, int oldValue, int newValue)
        {
            if (oldValue == newValue) { return false; }

            if (TreeDataservice.IsTreeNumberAvalible(tree.CuttingUnitCode, newValue, tree.PlotNumber, tree.StratumCode))
            {
                return true;
            }
            else
            {
                DialogService.ShowMessageAsync("Tree Number already taken");
                return false;
            }
        }

        #endregion TreeNumber

        #region Stratum

        public IEnumerable<string> StratumCodes
        {
            get => _stratumCodes;
            set => SetProperty(ref _stratumCodes, value);
        }

        public string StratumCode
        {
            get { return Tree?.StratumCode; }
            set
            {
                if (IsTreeChanging) { return; }
                var oldValue = StratumCode;
                if (oldValue == value || string.IsNullOrEmpty(value)) { return; }
                HandleStratumChanged(oldValue, value).FireAndForget();
            }
        }

        protected async Task HandleStratumChanged(string oldValue, string newValue)
        {
            var tree = Tree;
            if (tree != null)
            {
                var curSG = tree.SampleGroupCode;
                var curSp = tree.SpeciesCode;

                var newSg = await CoerceSampleGroupAsync(newValue);
                if (String.IsNullOrEmpty(newSg))
                {
                    // cancel stratum change if no valid SG selected
                    OnPropertyChanged(nameof(StratumCode));
                    return;
                }
                tree.SampleGroupCode = newSg;
                OnPropertyChanged(nameof(SampleGroupCode));

                var newSp = await CoerceSpeciesAsyn(newValue, newSg);
                if (!String.IsNullOrEmpty(newSp))
                {
                    tree.SpeciesCode = newSp;
                    OnPropertyChanged(nameof(SpeciesCode));
                }
                
                tree.StratumCode = newValue;

                RefreshCruiseMethod(tree);
                RefreshSampleGroups(tree);
                RefreshSpeciesOptions(tree);
                RefreshTreeFieldValues(tree);
                RefreshErrorsAndWarnings(tree);

                SaveTree(tree);
                CruiseLogDataservice.Log($"Update Tree.StratumCode |oldSt:{oldValue}|newSt:{newValue}|oldSg:{curSG}|newSg:{tree.SampleGroupCode}|oldSp:{curSp}|newSp:{tree.SpeciesCode}|",
                    treeID: tree.TreeID,
                    fieldName: "StratumCode",
                    tableName: "Tree");
            }

            OnPropertyChanged(nameof(StratumCode));
        }

        #endregion Stratum

        #region SampleGroup

        public IEnumerable<string> SampleGroupCodes
        {
            get
            {
                return _sampleGroupCodes;
            }
            set
            {
                SetProperty(ref _sampleGroupCodes, value);
            }
        }

        public string SampleGroupCode
        {
            get { return Tree?.SampleGroupCode; }
            set
            {
                if (IsTreeChanging) { return; }
                var oldValue = SampleGroupCode;
                if (oldValue == value || String.IsNullOrEmpty(value)) { return; }
                HandleSampleGroupChanged(oldValue, value).FireAndForget();
            }
        }

        protected async Task HandleSampleGroupChanged(string oldValue, string newValue)
        {
            var tree = Tree;
            if (tree != null)
            {
                var curSt = tree.StratumCode;
                var curSp = tree.SpeciesCode;

                var newSp = await CoerceSpeciesAsyn(curSt, newValue);
                if (!String.IsNullOrEmpty(newSp))
                {
                    tree.SpeciesCode = newSp;
                    OnPropertyChanged(nameof(SpeciesCode));
                }
                
                tree.SampleGroupCode = newValue;

                RefreshSpeciesOptions(tree);
                RefreshErrorsAndWarnings(tree);

                SaveTree(tree);
                CruiseLogDataservice.Log($"Update Tree.SampleGroup |oldSG:{oldValue}|newSG:{newValue}|oldSp:{curSp}|newSp:{tree.SpeciesCode}|",
                    treeID: tree.TreeID,
                    fieldName: "SampleGroupCode",
                    tableName: "Tree");
            }
            OnPropertyChanged(nameof(StratumCode));
        }

        public async Task<string> CoerceSampleGroupAsync(string stratumCode)
        {
            var sgCodes = SampleGroupDataservice.GetSampleGroupCodes(stratumCode).ToArray();
            var curSgCode = Tree.SampleGroupCode;
            if (!sgCodes.Contains(curSgCode))
            {
                var selectedSgCode = await DialogService.AskValueAsync("Select Sample Group", sgCodes);
                return selectedSgCode;
            }
            return curSgCode;
        }

        #endregion SampleGroup

        #region Species

        public IEnumerable<string> SpeciesOptions
        {
            get => _speciesOptions;
            protected set => SetProperty(ref _speciesOptions, value);
        }

        public string SpeciesCode
        {
            get => Tree?.SpeciesCode;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.SpeciesCode;
                if (oldValue == value) { return; }
                tree.SpeciesCode = value;

                SaveTree(tree);
                RefreshErrorsAndWarnings(tree);
                OnPropertyChanged();
            }
        }

        protected async Task<string> CoerceSpeciesAsyn(string stratum, string sg)
        {
            var speciesCodes = SpeciesDataservice.GetSpeciesCodes(stratum, sg).ToArray();
            var curSpecies = Tree.SpeciesCode;
            if (!speciesCodes.Contains(curSpecies))
            {
                var selectedSpecies = await DialogService.AskValueAsync("Select Species", speciesCodes);
                return selectedSpecies;
            }
            return curSpecies;
        }

        #endregion Species

        #region LiveDead

        public IEnumerable<string> LiveDeadOptions => new string[] { "L", "D" };

        public string LiveDead
        {
            get => Tree?.LiveDead;
            set
            {
                if (IsTreeChanging) { return; }
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.LiveDead;
                if (oldValue == value) { return; }
                tree.LiveDead = value;

                SaveTree(tree);
                RefreshErrorsAndWarnings(tree);
                OnPropertyChanged();
            }
        }

        #endregion LiveDead

        public string CruiseMethod
        {
            get => _cruiseMethod;
            protected set => SetProperty(ref _cruiseMethod, value);
        }

        public readonly string[] GradeOptions = new[] { "0", "1", "2", "3", "4", "5", "6", "6", "8", "9" };

        public ICommand ShowLogsCommand => _showLogsCommand ?? (_showLogsCommand = new DelegateCommand(ShowLogs));

        public ICommand ShowEditTreeErrorCommand => _showEditTreeErrorCommand ?? (_showEditTreeErrorCommand = new DelegateCommand<TreeError>(ShowEditTreeError));

        private void ShowEditTreeError(TreeError treeError)
        {
            if (treeError.Level != "W"
                || treeError.TreeAuditRuleID == null)
            { return; }
            else
            {
                //NavigationService.NavigateAsync("TreeErrorEdit",
                //    new Prism.Navigation.NavigationParameters($"{NavParams.TreeID}={treeError.TreeID}&{NavParams.TreeAuditRuleID}={treeError.TreeAuditRuleID}"));

                NavigationService.ShowTreeErrorEdit(treeError.TreeID, treeError.TreeAuditRuleID);
            }
        }

        public void ShowLogs()
        {
            NavigationService.ShowLogsList(Tree.TreeID);
        }

        public TreeEditViewModel(
            IStratumDataservice stratumDataservice,
            ISampleGroupDataservice sampleGroupDataservice,
            ISpeciesDataservice speciesDataservice,
            ISubpopulationDataservice subpopulationDataservice,
            ITreeDataservice treeDataservice,
            ITreeErrorDataservice treeErrorDataservice,
            ITreeFieldValueDataservice treeFieldValueDataservice,
            ICruiseLogDataservice cruiseLogDataservice,
            ICruisersDataservice cruisersDataservice,
            INatCruiseDialogService dialogService,
            INatCruiseNavigationService navigationService,
            ILoggingService loggingService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(speciesDataservice));
            SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TreeErrorDataservice = treeErrorDataservice ?? throw new ArgumentNullException(nameof(treeErrorDataservice));
            TreeFieldValueDataservice = treeFieldValueDataservice ?? throw new ArgumentNullException(nameof(treeFieldValueDataservice));
            CruiseLogDataservice = cruiseLogDataservice ?? throw new ArgumentNullException(nameof(cruiseLogDataservice));
            CruisersDataservice = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var treeID = parameters.GetValue<string>(NavParams.TreeID);
            Load(treeID);
        }

        public void Load(string treeID)
        {
            if (treeID is null) { throw new ArgumentNullException(nameof(treeID)); }
            Tree = TreeDataservice.GetTree(treeID) ?? throw new NullReferenceException("GetTree returned null"); ;
        }

        private void RefreshCruiseMethod(Tree tree)
        {
            var stratumCode = tree?.StratumCode;
            if (string.IsNullOrEmpty(stratumCode) == false)
            { CruiseMethod = StratumDataservice.GetCruiseMethod(stratumCode); }
            else
            { CruiseMethod = null; }
        }

        private void RefreshSampleGroups(Tree tree)
        {
            var stratum = tree.StratumCode;
            var sampleGroups = SampleGroupDataservice.GetSampleGroupCodes(stratum);
            SampleGroupCodes = sampleGroups;
        }

        private void RefreshSpeciesOptions(TreeEx tree)
        {
            var stratum = tree.StratumCode;
            var sampleGroup = tree.SampleGroupCode;
            SpeciesOptions = SpeciesDataservice.GetSpeciesCodes(stratum, sampleGroup);
        }

        private void RefreshTreeFieldValues(Tree tree)
        {
            var treeFieldValues = TreeFieldValueDataservice.GetTreeFieldValues(tree.TreeID);
            TreeFieldValues = treeFieldValues;
        }

        public void RefreshErrorsAndWarnings()
        {
            RefreshErrorsAndWarnings(Tree);
        }

        protected void RefreshErrorsAndWarnings(TreeEx tree)
        {
            if (tree == null) { return; }

            var errorsAndWarnings = TreeErrorDataservice.GetTreeErrors(tree.TreeID).ToArray();
            var warnings = errorsAndWarnings
                .Where(x => x.Level == ErrorBase.LEVEL_WARNING && !x.IsResolved)
                .ToDictionary(x => x.Field, StringComparer.OrdinalIgnoreCase, ToDictionaryConflictOption.Ignore);

            var errors = errorsAndWarnings
                .Where(x => x.Level == ErrorBase.LEVEL_ERROR)
                .ToDictionary(x => x.Field, StringComparer.OrdinalIgnoreCase, ToDictionaryConflictOption.Ignore);

            foreach (var tf in TreeFieldValues)
            {
                var field = tf.Field;

                tf.Error = errors.GetValueOrDefault(field) ?? warnings.GetValueOrDefault(field);
            }

            ErrorsAndWarnings = errorsAndWarnings;

            tree.ErrorCount = errors.Count;
            tree.WarningCount = warnings.Count;
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(WarningCount));
        }

        protected void RefreshCruisers(TreeEx tree)
        {
            var cruisers = CruisersDataservice.GetCruisers().ToArray();
            if (cruisers.Any())
            {
                var initials = tree.Initials;
                if (!string.IsNullOrEmpty(initials)
                    && !cruisers.Contains(initials, StringComparer.OrdinalIgnoreCase))
                {
                    cruisers = cruisers.Append(initials).ToArray();
                }
            }

            Cruisers = cruisers;
        }

        public void SaveTree()
        {
            SaveTree(Tree);
        }

        protected void SaveTree(Tree tree)
        {
            if (tree != null)
            {
                try
                {
                    TreeDataservice.UpdateTree(tree);
                }
                catch (Exception e)
                {
                    LoggingService.LogException(nameof(TreeEditViewModel), "SaveTree", e);
                    DialogService.ShowMessageAsync("Save Tree Error - Invalid Field Value");
                }
            }
        }
    }
}