using FScruiser.Models;
using FScruiser.Models.Validators;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    //TODO the validation of species and sample group could use some improvement
    // rightnow it is done when saving the tree. It is important that we validate sampleGroup before saving,
    // because we do want to prevent saving a tree that is going to throw a database exception
    // but validation requires that the sampleGroup options be updated before.
    // Currently this is done but not in a very elegant way.
    // also it would be nice if the view model had a Errors property that exposed a observable dictionary
    // which exposed all the errors rather than having properties to indecated if a property had an error

    public class TreeEditViewModel : ViewModelBase
    {
        private Command _showLogsCommand;
        private IEnumerable<string> _stratumCodes;
        private IEnumerable<string> _sampleGroupCodes;
        private IEnumerable<SubPopulation> _subPopulations;
        private IEnumerable<TreeError> _errorsAndWarnings;
        private IEnumerable<TreeFieldValue> _treeFieldValues;
        private Tree_Ex _tree;
        private bool _hasSampleGroupError;
        private bool _hasSpeciesError;
        private Command<TreeError> _showEditTreeErrorCommand;
        private IEnumerable<string> _cruisers;
        private string _initials;

        protected ICuttingUnitDatastore Datastore { get; set; }
        public ICruisersDataservice CruisersDataservice { get; }
        protected IDialogService DialogService { get; set; }

        public bool UseSimplifiedTreeFields { get; set; } = false;

        public IEnumerable<string> Cruisers
        {
            get => _cruisers;
            set => SetValue(ref _cruisers, value);
        }

        public IEnumerable<TreeError> ErrorsAndWarnings
        {
            get => _errorsAndWarnings;
            set => SetValue(ref _errorsAndWarnings, value);
        }

        public IDictionary<string, string> Errors { get; set; }

        public IEnumerable<TreeFieldValue> TreeFieldValues
        {
            get => _treeFieldValues;
            set => SetValue(ref _treeFieldValues, value);
        }

        public Tree_Ex Tree
        {
            get { return _tree; }
            protected set
            {
                SetValue(ref _tree, value);
                RaisePropertyChanged(nameof(TreeNumber));
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(SampleGroupCode));
                //RaisePropertyChanged(nameof(SubPopulation));
                RaisePropertyChanged(nameof(Species));
                RaisePropertyChanged(nameof(LiveDead));
                RaisePropertyChanged(nameof(Remarks));
            }
        }

        public string Initials
        {
            get => _initials;
            set => SetValue(ref _initials, value);
        }

        public string Remarks
        {
            get => Tree?.Remarks;
            set
            {
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.Remarks;
                if(value != oldValue)
                {
                    Datastore.UpdateTreeRemarks(tree.TreeID, value);
                }
            }
        }

        public bool HasSampleGroupError { get => _hasSampleGroupError; set => SetValue(ref _hasSampleGroupError, value); }

        public bool HasSpeciesError { get => _hasSpeciesError; set => SetValue(ref _hasSpeciesError, value); }

        #region TreeNumber

        public int TreeNumber
        {
            get
            {
                return Tree?.TreeNumber ?? 0;
            }
            set
            {
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.TreeNumber;
                if (OnTreeNumberChanging(tree, oldValue, value))
                {
                    Tree.TreeNumber = value;
                    OnTreeNumberChanged(oldValue, value);
                }
            }
        }

        private void OnTreeNumberChanged(int oldValue, int value)
        {
            SaveTree();
        }

        private bool OnTreeNumberChanging(Tree tree, int oldValue, int newValue)
        {
            if (oldValue == newValue) { return false; }

            if (Datastore.IsTreeNumberAvalible(tree.CuttingUnitCode, newValue, tree.PlotNumber))
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
            set => SetValue(ref _stratumCodes, value);
        }

        public string StratumCode
        {
            get { return Tree?.StratumCode; }
            set
            {
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

        private void OnStratumChanged(Tree tree, string oldValue, string newValue)
        {
            //Dataservice.LogMessage($"Tree Stratum Tree_GUID:{tree.Tree_GUID} OldStratumCode:{oldValue} NewStratumCode:{newValue}", "I");

            //if (SampleGroups.Any(x => x.Code == newValue) == false)
            //{
            //    Tree.SampleGroupCode = "";
            //}


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

            if (curStratumCode != null)
            {
                //if (!DialogService.AskYesNoAsync("You are changing the stratum of a tree" +
                //    ", are you sure you want to do this?", "!").Result)
                //{
                //    return false;//do not change stratum
                //}
                //else
                //{
                return true;
                //}
            }
            else
            {
                return true;
            }
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
                SetValue(ref _sampleGroupCodes, value);
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

        private void OnSampleGroupChanged(Tree tree, string oldValue, string newValue)
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
            if (string.IsNullOrWhiteSpace(oldValue)) { return true; }
            else
            {
                //TODO find a way to conferm sampleGroup canges
                //if (!DialogService.AskYesNoAsync("You are changing the Sample Group of a tree, are you sure you want to do this?"
                //    , "!"
                //    , true).Result)
                //{
                //    return false;
                //}
                //else
                //{
                return true;
                //}
            }
        }

        #endregion SampleGroup

        //#region SubPopulation

        protected IEnumerable<SubPopulation> SubPopulations
        {
            get => _subPopulations;
            set
            {
                SetValue(ref _subPopulations, value);
                //RaisePropertyChanged(nameof(SubPopulation));
                RaisePropertyChanged(nameof(SpeciesOptions));
            }
        }

        //public SubPopulation SubPopulation
        //{
        //    get
        //    {
        //        var tree = Tree;
        //        if(tree == null) { return null; }

        //        return SubPopulations.OrEmpty()
        //        .Where(x => x.Species == tree.Species && x.LiveDead == tree.LiveDead)
        //        .FirstOrDefault();
        //    }

        //    set
        //    {
        //        var tree = Tree;
        //        if(tree == null) { return; }

        //        if (value != null)
        //        {
        //            tree.Species = value.Species;
        //            tree.LiveDead = value.LiveDead;
        //        }
        //        else
        //        {
        //            tree.Species = null;
        //            tree.LiveDead = null;
        //        }
        //        OnSubPopulationChanged(tree);
        //    }
        //}

        //protected void OnSubPopulationChanged(Tree tree)
        //{
        //    SaveTree(tree);
        //    RefreshErrorsAndWarnings(tree);
        //}

        //#endregion SubPopulation

        #region Species

        public IEnumerable<string> SpeciesOptions
        {
            get
            {
                var tree = Tree;

                return _subPopulations.OrEmpty()
                  .Select(x => x.Species)
                  .ToArray();
            }
        }

        public string Species
        {
            get => Tree?.Species;
            set
            {
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.Species;
                if (OnSpeciesChanging(oldValue, value))
                {
                    tree.Species = value;
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

        private void OnSpeciesChanged(Tree tree, string value)
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
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.LiveDead;
                tree.LiveDead = value;
                OnLiveDeadChanged(tree, oldValue);
            }
        }

        private void OnLiveDeadChanged(Tree tree, object oldValue)
        {
            SaveTree(tree);
            RefreshErrorsAndWarnings(tree);
        }

        #endregion LiveDead

        public ICommand ShowLogsCommand => _showLogsCommand ?? (_showLogsCommand = new Command(ShowLogs));

        public ICommand ShowEditTreeErrorCommand => _showEditTreeErrorCommand ?? (_showEditTreeErrorCommand = new Command<TreeError>(ShowEditTreeError));
        private void ShowEditTreeError(TreeError treeError)
        {
            if (treeError.Level != "W"
                || treeError.TreeAuditRuleID == null)
            { return; }
            else
            {

                NavigationService.NavigateAsync("TreeErrorEdit",
                    new Prism.Navigation.NavigationParameters($"{NavParams.TreeID}={treeError.TreeID}&{NavParams.TreeAuditRuleID}={treeError.TreeAuditRuleID}"));
            }
        }

        public TreeEditViewModel(IDataserviceProvider datastoreProvider
            , IDialogService dialogService)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
            CruisersDataservice = datastoreProvider.Get<ICruisersDataservice>();
            DialogService = dialogService;
        }

        public TreeEditViewModel(IDataserviceProvider datastoreProvider
            , IDialogService dialogService, INavigationService navigationService) : base(navigationService)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
            CruisersDataservice = datastoreProvider.Get<ICruisersDataservice>();
            DialogService = dialogService;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Tree = null;//unwire tree
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var treeID = parameters.GetValue<string>(NavParams.TreeID);

            var datastore = Datastore;

            var tree = datastore.GetTree(treeID);

            var unitCode = tree.CuttingUnitCode;
            var stratumCodes = datastore.GetStratumCodesByUnit(unitCode);
            StratumCodes = stratumCodes;

            RefreshSampleGroups(tree);
            RefreshSubPopulations(tree);
            RefreshTreeFieldValues(tree);
            RefreshErrorsAndWarnings(tree);

            Cruisers = CruisersDataservice.GetCruisers().ToArray();

            Tree = tree;
        }

        private void RefreshSampleGroups(Tree tree)
        {
            var stratum = tree.StratumCode;
            var sampleGroups = Datastore.GetSampleGroupCodes(stratum);
            SampleGroupCodes = sampleGroups;
        }

        private void RefreshSubPopulations(Tree tree)
        {
            var stratumCode = tree.StratumCode;
            var sampleGroupCode = tree.SampleGroupCode;

            var subPopulations = Datastore.GetSubPopulations(stratumCode, sampleGroupCode);
            SubPopulations = subPopulations;
        }

        private void RefreshTreeFieldValues(Tree tree)
        {
            var treeFieldValues = Datastore.GetTreeFieldValues(tree.TreeID);

            foreach (var tfv in treeFieldValues)
            {
                tfv.PropertyChanged += treeFieldValue_PropertyChanged;
            }

            TreeFieldValues = treeFieldValues;
        }

        private void treeFieldValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var treeFieldValue = (TreeFieldValue)sender;
            Datastore.UpdateTreeFieldValue(treeFieldValue);
            RefreshErrorsAndWarnings();
        }

        public void RefreshErrorsAndWarnings()
        {
            RefreshErrorsAndWarnings(Tree);
        }

        protected void RefreshErrorsAndWarnings(Tree tree)
        {
            if (tree == null) { return; }

            ErrorsAndWarnings = Datastore.GetTreeErrors(tree.TreeID);
        }

        public void ShowLogs()
        {
            NavigationService.NavigateAsync("Logs", new NavigationParameters($"Tree_Guid={Tree.TreeID}"));
        }

        protected void SaveTree()
        {
            SaveTree(Tree);
        }

        protected void SaveTree(Tree tree)
        {
            HasSampleGroupError = false;
            HasSpeciesError = false;

            if (tree != null)
            {
                if (ValidateSampleGroupCode(tree.SampleGroupCode)
                    && ValidateSpecies(tree.Species))
                {
                    try
                    {
                        Datastore.UpdateTree(tree);
                    }
                    catch (Exception)
                    {
                        //TODO notify error to view
                    }
                }
            }
        }

        //public static void SetTreeTDV(Tree tree, TreeDefaultValueDO tdv)
        //{
        //    if (tdv != null)
        //    {
        //        tree.TreeDefaultValue_CN = tdv.TreeDefaultValue_CN;
        //        tree.Species = tdv.Species;

        //        tree.LiveDead = tdv.LiveDead;
        //        tree.Grade = tdv.TreeGrade;
        //        tree.FormClass = tdv.FormClass;
        //        tree.RecoverablePrimary = tdv.Recoverable;
        //        //tree.HiddenPrimary = tdv.HiddenPrimary;//#367
        //    }
        //    else
        //    {
        //        tree.TreeDefaultValue_CN = null;
        //        tree.Species = string.Empty;
        //        tree.LiveDead = string.Empty;
        //        tree.Grade = string.Empty;
        //        tree.FormClass = 0;
        //        tree.RecoverablePrimary = 0;
        //        //this.HiddenPrimary = 0;
        //    }
        //}
    }
}