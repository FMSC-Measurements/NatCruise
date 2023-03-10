﻿using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private IEnumerable<Subpopulation> _subPopulations;
        private IEnumerable<TreeError> _errorsAndWarnings;
        private IEnumerable<TreeFieldValue> _treeFieldValues;
        private TreeEx _tree;
        private bool _hasSampleGroupError;
        private bool _hasSpeciesError;
        private DelegateCommand<TreeError> _showEditTreeErrorCommand;
        private IEnumerable<string> _cruisers;
        private string _cruiseMethod;
        private IEnumerable<string> _countOrMeasureOptions;
        private IEnumerable<string> _speciesOptions;

        private bool IsTreeChanging { get; set; }

        protected IStratumDataservice StratumDataservice { get; }
        protected ISampleGroupDataservice SampleGroupDataservice { get; }
        protected ISubpopulationDataservice SubpopulationDataservice { get; }
        protected ITreeDataservice TreeDataservice { get; }
        protected ITreeErrorDataservice TreeErrorDataservice { get; }
        protected ITreeFieldValueDataservice TreeFieldValueDataservice { get; }
        protected ICruisersDataservice CruisersDataservice { get; }
        protected INatCruiseDialogService DialogService { get; }
        protected INatCruiseNavigationService NavigationService { get; }
        protected ILoggingService LoggingService { get; }


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
                    RaisePropertyChanged(nameof(CountOrMeasure));
                    RaisePropertyChanged(nameof(TreeNumber));
                    RaisePropertyChanged(nameof(StratumCode));
                    RaisePropertyChanged(nameof(SampleGroupCode));
                    //RaisePropertyChanged(nameof(SubPopulation));
                    RaisePropertyChanged(nameof(SpeciesCode));
                    RaisePropertyChanged(nameof(LiveDead));
                    RaisePropertyChanged(nameof(Remarks));
                    RaisePropertyChanged(nameof(Initials));
                    RaisePropertyChanged(nameof(TreeCount));
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
                RefreshSubPopulations(tree);
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
            }
        }

        public bool HasSampleGroupError { get => _hasSampleGroupError; set => SetProperty(ref _hasSampleGroupError, value); }

        public bool HasSpeciesError { get => _hasSpeciesError; set => SetProperty(ref _hasSpeciesError, value); }

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
                OnCountOrMeasureChanged(oldValue, value);
            }
        }

        private void OnCountOrMeasureChanged(string oldValue, string value)
        {
            SaveTree();
            RefreshErrorsAndWarnings();
        }

        //private bool OnCountOrMeasureChangeing(Tree_Ex tree, string oldValue, string newValue)
        //{
        //    var stratum = tree.StratumCode;
        //    var cruiseMethod = CuttingUnitDatastore.GetCruiseMethod(stratum);
        //    var isPlotMethod = CruiseDAL.Schema.CruiseMethods.PLOT_METHODS.Contains(cruiseMethod);
        //    if (isPlotMethod == false)
        //    {
        //        //DialogService.ShowMessageAsync($"Cruise Method {cruiseMethod} does not allow changing Count or Measure value");
        //    }
        //    return isPlotMethod;
        //}

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
                RaisePropertyChanged(nameof(TreeNumber));
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
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = Tree.StratumCode;
                if (OnStratumChanging(oldValue, value))
                {
                    tree.StratumCode = value;
                    OnStratumChanged(tree, oldValue, value);
                }
            }
        }

        private void OnStratumChanged(TreeEx tree, string oldValue, string newValue)
        {
            //Dataservice.LogMessage($"Tree Stratum Tree_GUID:{tree.Tree_GUID} OldStratumCode:{oldValue} NewStratumCode:{newValue}", "I");

            //if (SampleGroups.Any(x => x.Code == newValue) == false)
            //{
            //    Tree.SampleGroupCode = "";
            //}

            RefreshCruiseMethod(tree);
            RefreshSampleGroups(tree);
            RefreshSubPopulations(tree);
            RefreshTreeFieldValues(tree);
            RefreshErrorsAndWarnings(tree);

            SaveTree(tree);
        }

        private bool OnStratumChanging(string oldValue, string newStratum)
        {
            if (oldValue == newStratum) { return false; }
            if (string.IsNullOrWhiteSpace(newStratum)) { return false; }
            var curStratumCode = StratumCode;
            if (string.IsNullOrWhiteSpace(curStratumCode) == false
                && curStratumCode == newStratum)
            { return false; }
            return true;

            //if (curStratumCode != null)
            //{
            //    if (!DialogService.AskYesNoAsync("You are changing the stratum of a tree" +
            //        ", are you sure you want to do this?", "!").Result)
            //    {
            //        return false;//do not change stratum
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    return true;
            //}
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
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.SampleGroupCode;
                if (OnSampleGroupChanging(oldValue, value))
                {
                    tree.SampleGroupCode = value;
                    OnSampleGroupChanged(tree, oldValue, value);
                }
            }
        }

        protected bool ValidateSampleGroupCode(string sgCode)
        {
            if (SampleGroupCodes == null || SampleGroupCodes.Contains(sgCode))
            {
                HasSampleGroupError = false;
                return true;
            }
            else
            {
                HasSampleGroupError = true;
                return false;
            }
        }

        private void OnSampleGroupChanged(TreeEx tree, string oldValue, string newValue)
        {
            //Dataservice.LogMessage($"Tree SampleGroupCanged, Tree_GUID:{Tree.Tree_GUID}, OldSG:{oldValue}, NewSG:{newValue}", "high");

            RefreshErrorsAndWarnings(tree);
            RefreshSubPopulations(tree);

            SaveTree(tree);
        }

        private bool OnSampleGroupChanging(string oldValue, string newSG)
        {
            if (string.IsNullOrWhiteSpace(newSG)) { return false; }
            if (oldValue == newSG) { return false; }
            return true;
            //if (string.IsNullOrWhiteSpace(oldValue)) { return true; }
            //else
            //{
            //    //TODO find a way to confirm sampleGroup changes
            //    if (!DialogService.AskYesNoAsync("You are changing the Sample Group of a tree, are you sure you want to do this?"
            //        , "!"
            //        , true).Result)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
        }

        #endregion SampleGroup

        // TODO remove SubPopulations?
        protected IEnumerable<Subpopulation> SubPopulations
        {
            get => _subPopulations;
            set
            {
                SetProperty(ref _subPopulations, value);

                SpeciesOptions = SubPopulations.OrEmpty()
                        .Select(x => x.SpeciesCode)
                        .ToArray();
            }
        }

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
                if (OnSpeciesChanging(oldValue, value))
                {
                    tree.SpeciesCode = value;

                    OnSpeciesChanged(tree, value);
                }
            }
        }

        protected bool ValidateSpecies(string value)
        {
            if (SpeciesOptions == null || SpeciesOptions.Contains(value))
            {
                HasSpeciesError = false;
                return true;
            }
            else
            {
                HasSpeciesError = true;
                return false;
            }
        }

        private void OnSpeciesChanged(TreeEx tree, string value)
        {
            SaveTree(tree);

            RefreshErrorsAndWarnings(tree);
        }

        private bool OnSpeciesChanging(string oldValue, string value)
        {
            return true;
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
                tree.LiveDead = value;
                OnLiveDeadChanged(tree, oldValue);
            }
        }

        private void OnLiveDeadChanged(TreeEx tree, object oldValue)
        {
            SaveTree(tree);
            RefreshErrorsAndWarnings(tree);
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
            ISubpopulationDataservice subpopulationDataservice,
            ITreeDataservice treeDataservice,
            ITreeErrorDataservice treeErrorDataservice,
            ITreeFieldValueDataservice treeFieldValueDataservice,
            INatCruiseDialogService dialogService,
            INatCruiseNavigationService navigationService,
            ICruisersDataservice cruisersDataservice,
            ILoggingService loggingService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TreeErrorDataservice = treeErrorDataservice ?? throw new ArgumentNullException(nameof(treeErrorDataservice));
            TreeFieldValueDataservice = treeFieldValueDataservice ?? throw new ArgumentNullException(nameof(treeFieldValueDataservice));
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

        private void RefreshSubPopulations(Tree tree)
        {
            var stratumCode = tree.StratumCode;
            var sampleGroupCode = tree.SampleGroupCode;

            var subPopulations = SubpopulationDataservice.GetSubpopulations(stratumCode, sampleGroupCode);
            SubPopulations = subPopulations;
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
                .ToDictionary(x => x.Field, StringComparer.OrdinalIgnoreCase);

            var errors = errorsAndWarnings
                .Where(x => x.Level == ErrorBase.LEVEL_ERROR)
                .ToDictionary(x => x.Field, StringComparer.OrdinalIgnoreCase);

            foreach (var tf in TreeFieldValues)
            {
                var field = tf.Field;

                tf.Error = errors.GetValueOrDefault(field) ?? warnings.GetValueOrDefault(field);
            }

            ErrorsAndWarnings = errorsAndWarnings;

            HasSpeciesError = errors.ContainsKey(nameof(Tree.SpeciesCode));

            //sample group is a required field in the data base, so we wont expect
            // to get any SG errors from GetTreeErrors
            //HasSampleGroupError = errors.ContainsKey(nameof(Tree.SampleGroupCode));

            tree.ErrorCount = errors.Count;
            tree.WarningCount = warnings.Count;
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
            HasSampleGroupError = false;
            HasSpeciesError = false;

            if (tree != null)
            {
                if (ValidateSampleGroupCode(tree.SampleGroupCode))
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
}
