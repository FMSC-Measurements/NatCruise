using CruiseDAL.Schema;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
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
    public class TallyViewModel : XamarinViewModelBase
    {
        public static readonly string STRATUM_FILTER_ALL = "All";

        private IEnumerable<TallyPopulation> _tallies;
        private IEnumerable<string> _stratumCodes;
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

        public IEnumerable<TallyPopulation> Tallies
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
            get => _stratumCodes;
            set => SetProperty(ref _stratumCodes, value);
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

        public IEnumerable<TallyPopulation> TalliesFiltered
        {
            get
            {
                var tallies = Tallies;
                var selectedStratum = SelectedStratumCode;

                if (tallies == null) { return Enumerable.Empty<TallyPopulation>(); }
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

        public ICommand SelectTallyEntryCommand => new Command<object>(SelectTallyEntry);
        //public ICommand ChangeSelectedTallyEntryCommand => new Command(ChangeSelectedTallyEntry);

        public string UnitCode
        {
            get => _unitCode;
            set
            {
                _unitCode = value;
            }
        }

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
            protected set => SetProperty(ref _selectedEntry, value);
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
        }

        #region Commands

        private ICommand _editTreeCommand;
        private ICommand _showTallyMenuCommand;
        private ICommand _stratumSelectedCommand;
        private ICommand _tallyCommand;
        private ICommand _untallyCommand;
        private string _title;
        private string _unitCode;
        private TreeEditViewModel _selectedTreeViewModel;
        private TallyEntry _selectedEntry;

        public ICommand ShowTallyMenuCommand => _showTallyMenuCommand
            ?? (_showTallyMenuCommand = new Command<TallyPopulation>(ShowTallyMenu));

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new DelegateCommand<TallyPopulation>(async (x) => await TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<string>(EditTree));

        public ICommand UntallyCommand => _untallyCommand
            ?? (_untallyCommand = new Command<string>(Untally));

        #endregion Commands

        protected ICruiseNavigationService NavigationService { get; }
        public ITallyDataservice TallyDataservice { get; }
        public ITreeDataservice TreeDataservice { get; }
        public ITallyPopulationDataservice TallyPopulationDataservice { get; }

        public ICruiseDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorService { get; }
        public ICruisersDataservice CruisersDataService { get; }
        public ISoundService SoundService { get; }
        public IContainerProvider ContainerProvider { get; }
        public ITreeBasedTallyService TallyService { get; }

        public TallyViewModel(ICruiseNavigationService navigationService,
            ITallyDataservice tallyDataservice,
            ITreeDataservice treeDataservice,
            ITallyPopulationDataservice tallyPopulationDataservice,
            ISampleSelectorDataService sampleSelectorDataservice,
            ICruiseDialogService dialogService,
            ISoundService soundService,
            ICruisersDataservice cruisersDataservice,
            IContainerProvider containerProvider,
            ITreeBasedTallyService tallyService)
        {
            TallyDataservice = tallyDataservice ?? throw new ArgumentNullException(nameof(tallyDataservice));
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            SampleSelectorService = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(sampleSelectorDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            CruisersDataService = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));
            SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            TallyService = tallyService ?? throw new ArgumentNullException(nameof(tallyService));
        }

        public void SelectTallyEntry(object obj)
        {
            var tallyEntry = obj as TallyEntry;
            SelectedEntry = tallyEntry;
            var treeID = tallyEntry?.TreeID;
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
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = UnitCode = parameters.GetValue<string>(NavParams.UNIT);

            Title = $"Unit {unitCode}";

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

            // we need to reload tally pops on each load incase coming back from edit tree counts
            Tallies = tallyPopulations;

            
            var tf = TallyFeed;
            if(tf != null)
            {
                // HACK reloading tally feed causes us to lose the scroll position
                // to mantain the scroll position we need to add new tally entry items to the existing
                // tally feed collection instead of reloading the whole collection
                // we should only need to add entries when coming back from edit tree counts and only be adding one entry when doing so

                var tallyEntries = TallyDataservice.GetTallyEntriesByUnitCode(UnitCode).Reverse();
                var newTe = tallyEntries.Except(tf, new TallyEntryComparer());

                foreach(var entry in newTe)
                {
                    tf.Add(entry);
                }
            }
            else
            {
                TallyFeed = TallyDataservice.GetTallyEntriesByUnitCode(UnitCode).Reverse().ToObservableCollection();
            }

            

            // refresh selected tree incase coming back from TreeEdit page

            RaisePropertyChanged(nameof(SelectedTreeViewModel));
        }

        private void ShowTallyMenu(TallyPopulation tp)
        {
            NavigationService.ShowTreeCountEdit(UnitCode, tp.StratumCode, tp.SampleGroupCode, tp.SpeciesCode, tp.LiveDead);

            //NavigationService.NavigateAsync($"TreeCountEdit?{NavParams.UNIT}={UnitCode}&{NavParams.STRATUM}={obj.StratumCode}&{NavParams.SAMPLE_GROUP}={obj.SampleGroupCode}&{NavParams.SPECIES}={obj.SpeciesCode}&{NavParams.LIVE_DEAD}={obj.LiveDead}",
            //    useModalNavigation: true);
        }

        public void EditTree(string treeID)
        {
            NavigationService.ShowTreeEdit(treeID);

            //NavigationService.NavigateAsync("Tree", new NavigationParameters() { { NavParams.TreeID, treeID } });
        }

        public async Task TallyAsync(TallyPopulation pop)
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
                    DialogService.ShowMessageAsync("Tree #" + entry.TreeNumber.ToString(), sampleType).FireAndForget();
                }

                //if (tree.CountOrMeasure == "M" && await AskEnterMeasureTreeDataAsync(tallySettings, dialogService))
                //{
                //    var task = dialogService.ShowEditTreeAsync(tree, dataService);//allow method to contiue from show edit tree we will allow tally history action to be added in the background
                //}
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
            var tallyPopulation = Tallies.First(x => x.StratumCode == tallyEntry.StratumCode
            && x.SampleGroupCode == tallyEntry.SampleGroupCode
            && x.SpeciesCode == tallyEntry.SpeciesCode
            && x.LiveDead == tallyEntry.LiveDead);

            tallyPopulation.TreeCount -= tallyEntry.TreeCount;
            TallyFeed.Remove(TallyFeed.First(x => x.TallyLedgerID == tallyLedgerID));
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }
    }

    class TallyEntryComparer : EqualityComparer<TallyEntry>
    {
        public override bool Equals(TallyEntry x, TallyEntry y)
        {
            return x.TallyLedgerID == y.TallyLedgerID;
        }

        public override int GetHashCode(TallyEntry obj)
        {
            return obj.TallyLedgerID.GetHashCode();
        }
    }
}