using CruiseDAL.Schema;
using FScruiser.XF.Services;
using NatCruise;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
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

namespace FScruiser.XF.ViewModels
{
    public class TallyViewModel : ViewModelBase
    {
        public static readonly string STRATUM_FILTER_ALL = "All";

        private IEnumerable<TallyPopulationEx> _tallies;
        private IEnumerable<string> _stratumFilterOptions;
        private string _selectedStratumCode = STRATUM_FILTER_ALL;

        private IList<TallyEntry> _tallyFeed;

        public event EventHandler TallyEntryAdded;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public IList<TallyEntry> TallyFeed
        {
            get { return _tallyFeed; }
            set { SetProperty(ref _tallyFeed, value); }
        }

        public IEnumerable<TallyPopulationEx> Tallies
        {
            get { return _tallies; }
            protected set
            {
                SetProperty(ref _tallies, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
                RaisePropertyChanged(nameof(StrataFilterOptions));
            }
        }

        public IEnumerable<string> StrataFilterOptions
        {
            get => _stratumFilterOptions;
            set => SetProperty(ref _stratumFilterOptions, value);
        }

        public string SelectedStratumCode
        {
            get { return _selectedStratumCode; }
            set
            {
                SetProperty(ref _selectedStratumCode, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public IEnumerable<TallyPopulationEx> TalliesFiltered
        {
            get
            {
                var tallies = Tallies;
                var selectedStratum = SelectedStratumCode;

                if (tallies == null) { return Enumerable.Empty<TallyPopulationEx>(); }
                if (selectedStratum == STRATUM_FILTER_ALL)
                {
                    return tallies;
                }
                else
                {
                    return tallies.Where(x => x.StratumCode == selectedStratum).ToArray();
                }
            }
        }

        public ICommand SelectTallyEntryCommand => new DelegateCommand<object>(SelectTallyEntry);
        //public ICommand ChangeSelectedTallyEntryCommand => new Command(ChangeSelectedTallyEntry);

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            protected set
            {
                SetProperty(ref _cuttingUnit, value);
                RaisePropertyChanged(nameof(UnitCode));
            }
        }

        public string UnitCode => CuttingUnit.CuttingUnitCode;

        public TreeEditViewModel SelectedTreeViewModel
        {
            get => _selectedTreeViewModel;
            protected set
            {
                if(_selectedTreeViewModel != null)
                { _selectedTreeViewModel.PropertyChanged -= SelectedTreeViewModel_PropertyChanged; }
                SetProperty(ref _selectedTreeViewModel, value);
                if(value != null)
                { value.PropertyChanged += SelectedTreeViewModel_PropertyChanged; }
            }
        }

        public TallyEntry SelectedEntry
        {
            get => _selectedEntry;
            private set => SetProperty(ref _selectedEntry, value);
        }

        private void SelectedTreeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (TreeEditViewModel)sender;
            if(e.PropertyName == nameof(TreeEditViewModel.ErrorsAndWarnings))
            {
                var tallyEntry = SelectedEntry;
                if(tallyEntry != null)
                {
                    TallyDataservice.RefreshErrorsAndWarnings(tallyEntry);
                }
            }
            if(e.PropertyName == nameof(TreeEditViewModel.SpeciesCode))
            {
                var tallyEntry = SelectedEntry;
                if (tallyEntry != null)
                {
                    tallyEntry.SpeciesCode = vm.SpeciesCode;
                }
            }
            if (e.PropertyName == nameof(TreeEditViewModel.LiveDead))
            {
                var tallyEntry = SelectedEntry;
                if (tallyEntry != null)
                {
                    tallyEntry.LiveDead = vm.LiveDead;
                }
            }
        }

        #region Commands

        private ICommand _editTreeCommand;
        private ICommand _showTallyMenuCommand;
        private ICommand _stratumSelectedCommand;
        private ICommand _tallyCommand;
        private ICommand _untallyCommand;
        private ICommand _selectPreviouseTreeCommand;
        private ICommand _selectNextTreeCommand;
        private string _title;
        private TreeEditViewModel _selectedTreeViewModel;
        private TallyEntry _selectedEntry;
        private CuttingUnit _cuttingUnit;
        

        public ICommand ShowTallyMenuCommand => _showTallyMenuCommand ??= new DelegateCommand<TallyPopulationEx>((tp) => ShowTallyMenu(tp).FireAndForget());

        public ICommand TallyCommand => _tallyCommand ??= new DelegateCommand<TallyPopulationEx>((tp) => TallyAsync(tp).FireAndForget("TallyAsync"));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand ??= new DelegateCommand<string>(x => SetStratumFilter(x));

        public ICommand EditTreeCommand => _editTreeCommand ??= new DelegateCommand<string>((treeID) => EditTree(treeID).FireAndForget());

        public ICommand UntallyCommand => _untallyCommand ??= new DelegateCommand<string>(Untally);

        public ICommand SelectPreviousTreeCommand => _selectPreviouseTreeCommand ??= new DelegateCommand(SelectPreviousTree);

        public ICommand SelectNextTreeCommand => _selectNextTreeCommand ??= new DelegateCommand(SelectNextTree);

        #endregion Commands

        protected ICruiseNavigationService NavigationService { get; }
        public ITallyDataservice TallyDataservice { get; }
        public ITreeDataservice TreeDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorService { get; }
        public ICruisersDataservice CruisersDataService { get; }
        public ISoundService SoundService { get; }
        public IContainerProvider ContainerProvider { get; }
        public ITreeBasedTallyService TallyService { get; }
        public ILoggingService LoggingService { get; }

        public TallyViewModel(ICruiseNavigationService navigationService,
            ITallyDataservice tallyDataservice,
            ITreeDataservice treeDataservice,
            ICuttingUnitDataservice cuttingUnitDataservice,
            ITallyPopulationDataservice tallyPopulationDataservice,
            ISampleSelectorDataService sampleSelectorDataservice,
            //ICruiseDialogService cruiseDialogService,
            INatCruiseDialogService dialogService, 
            ISoundService soundService,
            ICruisersDataservice cruisersDataservice,
            IContainerProvider containerProvider,
            ITreeBasedTallyService tallyService,
            ILoggingService loggingService)
        {
            TallyDataservice = tallyDataservice ?? throw new ArgumentNullException(nameof(tallyDataservice));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            SampleSelectorService = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(sampleSelectorDataservice));
            //CruiseDialogService = cruiseDialogService ?? throw new ArgumentNullException(nameof(cruiseDialogService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            CruisersDataService = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));
            SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            TallyService = tallyService ?? throw new ArgumentNullException(nameof(tallyService));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public void SelectTallyEntry(object obj)
        {
            var tallyEntry = obj as TallyEntry;
            SelectedEntry = tallyEntry;
            var treeID = tallyEntry?.TreeID;
            if (treeID != null)
            {
                SetSelectedTreeViewModel(treeID);
            }
            else
            {
                SelectedTreeViewModel = null;
            }
        }

        protected void SetSelectedTreeViewModel(string treeID)
        {
            var treeVM = ContainerProvider.Resolve<TreeEditViewModel>((typeof(ICruiseNavigationService), NavigationService));
            treeVM.UseSimplifiedTreeFields = true;
            treeVM.Initialize(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, treeID } });
            treeVM.Load();

            SelectedTreeViewModel = treeVM;
        }

        public void SelectPreviousTree()
        {
            var selectedEntry = SelectedEntry;
            if (selectedEntry == null) { return; }

            var tallyFeed = TallyFeed;
            var i = tallyFeed.IndexOf(selectedEntry);
            if(i == -1) { return; }
            if(i < 1) { return; }

            var prevTallyEntry = tallyFeed.ReverseSearch(x => x.TreeID != null, i - 1);
            if (prevTallyEntry != null)
            {
                SelectTallyEntry(prevTallyEntry);
            }
        }

        public void SelectNextTree()
        {
            var selectedEntry = SelectedEntry;
            if (selectedEntry == null) { return; }

            var tallyFeed = TallyFeed;
            var i = tallyFeed.IndexOf(selectedEntry);
            if (i == -1) { return; }
            if (i == tallyFeed.Count - 1) { return; }

            var nextTallyEntry = tallyFeed.Search(x => x.TreeID != null, i + 1);
            if (nextTallyEntry != null)
            {
                SelectTallyEntry(nextTallyEntry);
            }
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var cuttingUnit = CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);

            Title = $"Unit {unitCode} - {cuttingUnit.Description}";

            var tallyPopulations = TallyPopulationDataservice.GetTallyPopulationsByUnitCode(UnitCode);
            var strata = tallyPopulations.Select(x => x.StratumCode)
                .Distinct()
                .Append(STRATUM_FILTER_ALL)
                .ToArray();

            if (strata.Count() > 2)
            {
                StrataFilterOptions = strata;
            }
            else
            { StrataFilterOptions = new string[0]; }

            // we need to reload tally pops on each load in case coming back from edit tree counts
            Tallies = tallyPopulations;

            
            var tf = TallyFeed;
            if(tf != null)
            {
                // HACK reassigning tally feed causes us to lose the scroll position
                // to maintain the scroll position we need update each item in the list in place
                // and add any new items
                // we should only need to add entries when coming back from edit tree counts and only be adding one entry when doing so

                //var newTallyEntries = new List<TallyEntry>();
                var tallyEntries = TallyDataservice.GetTallyEntriesByUnitCode(UnitCode).Reverse();

                var tfIDLookup = tf.ToDictionary(x => x.TallyLedgerID);

                foreach(var entry in tallyEntries)
                {
                    var eTlID = entry.TallyLedgerID;
                    if(tfIDLookup.ContainsKey(eTlID))
                    {
                        var e = tfIDLookup[eTlID];
                        e.TreeNumber = entry.TreeNumber;
                        e.StratumCode = entry.StratumCode;
                        e.SampleGroupCode = entry.SampleGroupCode;
                        e.SpeciesCode = entry.SpeciesCode;
                        e.LiveDead = entry.LiveDead;
                        e.CountOrMeasure = entry.CountOrMeasure;
                        e.WarningCount = entry.WarningCount;
                        e.ErrorCount = entry.ErrorCount;
                    }
                    else
                    {
                        tf.Add(entry);
                    }
                }
            }
            else
            {
                TallyFeed = TallyDataservice.GetTallyEntriesByUnitCode(UnitCode).Reverse().ToObservableCollection();
            }

            

            // refresh selected tree in case coming back from TreeEdit page

            RaisePropertyChanged(nameof(SelectedTreeViewModel));
        }

        private Task ShowTallyMenu(TallyPopulationEx tp)
        {
            return NavigationService.ShowTreeCountEdit(UnitCode, tp.StratumCode, tp.SampleGroupCode, tp.SpeciesCode, tp.LiveDead);
        }

        public Task EditTree(string treeID)
        {
            return NavigationService.ShowTreeEdit(treeID);
        }

        public async Task TallyAsync(TallyPopulationEx pop)
        {
            var entry = await TallyService.TallyAsync(UnitCode, pop);
            if (entry == null) { return; }
            SoundService.SignalTallyAsync().FireAndForget();

            pop.TreeCount = pop.TreeCount + entry.TreeCount;
            pop.SumKPI = pop.SumKPI + entry.KPI;

            TallyFeed.Add(entry);
            RaiseTallyEntryAdded();

            if (entry.CountOrMeasure == "M" || entry.CountOrMeasure == "I")
            {
                var method = pop.Method;
                var isInsuranceSample = entry.CountOrMeasure == "I";
                if (isInsuranceSample)
                {
                    SoundService.SignalInsuranceTreeAsync().FireAndForget();
                }
                else
                {
                    SoundService.SignalMeasureTreeAsync().FireAndForget();
                }

                if (CruisersDataService.PromptCruiserOnSample)
                {
                    var cruiser = await DialogService.AskCruiserAsync();
                    if (cruiser != null)
                    {
                        TreeDataservice.UpdateTreeInitials(entry.TreeID, cruiser);
                    }
                }
                else
                if (method != CruiseMethods.H_PCT)
                {
                    var sampleType = (isInsuranceSample) ? "Insurance Tree" : "Measure Tree";
                    DialogService.ShowNotification("Tree #" + entry.TreeNumber.ToString(), sampleType);
                }
            }
        }

        protected void RaiseTallyEntryAdded()
        {
            TallyEntryAdded?.Invoke(this, null);
        }

        public void Untally(string tallyLedgerID)
        {
            TallyDataservice.DeleteTallyEntry(tallyLedgerID);

            var tallyEntry = TallyFeed.First(x => x.TallyLedgerID == tallyLedgerID);
            var tallyPopulation = Tallies.FirstOrDefault(x => x.StratumCode == tallyEntry.StratumCode
            && x.SampleGroupCode == tallyEntry.SampleGroupCode
            && (x.SpeciesCode == tallyEntry.SpeciesCode || x.SpeciesCode is null)
            && (x.LiveDead == tallyEntry.LiveDead || x.LiveDead is null));
            if (tallyPopulation != null)
            {
                tallyPopulation.TreeCount -= tallyEntry.TreeCount;
                tallyPopulation.SumKPI -= tallyEntry.KPI;
            }

            var treeID = tallyEntry.TreeID;
            if (treeID != null)
            {
                if (SelectedTreeViewModel != null && SelectedTreeViewModel.TreeID == treeID)
                {
                    SelectedTreeViewModel = null;
                }
            }

            LoggingService.LogEvent("Untally", new Dictionary<string, string>()
            {
                {"CruiseID", TallyDataservice.CruiseID },
                {"CruiseID_CuttingUnit", $"{TallyDataservice.CruiseID}_{tallyEntry.CuttingUnitCode}" },
                {"Data", $"St: {tallyEntry.StratumCode}, Sg: {tallyEntry.SampleGroupCode}, Sp: {tallyEntry.SpeciesCode ?? "null"}, TreeID: {tallyEntry.TreeID ?? "null"}"  },
            });

            TallyFeed.Remove(tallyEntry);
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }
    }
}