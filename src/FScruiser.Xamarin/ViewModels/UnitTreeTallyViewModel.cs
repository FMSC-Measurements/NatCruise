using CruiseDAL.Schema;
using FScruiser.Data;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class UnitTreeTallyViewModel : ViewModelBase
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
            set => SetValue(ref _title, value);
        }
        public IList<TallyEntry> TallyFeed
        {
            get { return _tallyFeed; }
            set { SetValue(ref _tallyFeed, value); }
        }

        public IEnumerable<TallyPopulation> Tallies
        {
            get { return _tallies; }
            protected set
            {
                SetValue(ref _tallies, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
                RaisePropertyChanged(nameof(StrataFilterOptions));
            }
        }

        public IEnumerable<string> StrataFilterOptions
        {
            get => _stratumCodes;
            set => SetValue(ref _stratumCodes, value);
        }

        public string SelectedStratumCode
        {
            get { return _selectedStratumCode; }
            set
            {
                SetValue(ref _selectedStratumCode, value);
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

        public string UnitCode
        {
            get => _unitCode;
            set
            {
                _unitCode = value;
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

        public ICommand ShowTallyMenuCommand => _showTallyMenuCommand
            ?? (_showTallyMenuCommand = new Command<TallyPopulation>(ShowTallyMenu));

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation>(async (x) => await this.TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<string>(EditTree));

        public ICommand UntallyCommand => _untallyCommand
            ?? (_untallyCommand = new Command<string>(Untally));

        #endregion Commands

        public ITallyDataservice TallyDataservice { get; }
        public ITallyPopulationDataservice TallyPopulationDataservice { get; }

        public IDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorService { get; }
        public ICruisersDataservice CruisersDataService { get; }
        public ISoundService SoundService { get; }

        public UnitTreeTallyViewModel(INavigationService navigationService,
            IDataserviceProvider dataserviceProvider,
            IDialogService dialogService,
            ITallySettingsDataService tallySettings,
            ISoundService soundService) : base(navigationService)
        {
            TallyDataservice = dataserviceProvider.Get<ITallyDataservice>();
            TallyPopulationDataservice = dataserviceProvider.Get<ITallyPopulationDataservice>();
            SampleSelectorService = dataserviceProvider.Get<ISampleSelectorDataService>();
            DialogService = dialogService;
            CruisersDataService = dataserviceProvider.Get<ICruisersDataservice>();
            SoundService = soundService;
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unitCode = UnitCode = parameters.GetValue<string>("UnitCode");

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

            Tallies = tallyPopulations;

            TallyFeed = TallyDataservice.GetTallyEntriesByUnitCode(UnitCode).Reverse().ToObservableCollection();
        }

        private void ShowTallyMenu(TallyPopulation obj)
        {
            NavigationService.NavigateAsync($"TreeCountEdit?{NavParams.UNIT}={UnitCode}&{NavParams.STRATUM}={obj.StratumCode}&{NavParams.SAMPLE_GROUP}={obj.SampleGroupCode}&{NavParams.SPECIES}={obj.Species}&{NavParams.LIVE_DEAD}={obj.LiveDead}",
                useModalNavigation: true);
        }

        public void EditTree(string treeID)
        {
            NavigationService.NavigateAsync("Tree", new NavigationParameters() { { NavParams.TreeID, treeID } });
        }

        public async Task TallyAsync(TallyPopulation pop)
        {
            // perform logic to determin if tally is a sample
            var action = await TreeBasedTallyLogic.TallyAsync(UnitCode, pop, SampleSelectorService, DialogService);

            // record action to the database,
            // database will assign tree a tree number if there is a tree
            // action might be null if user dosn't enter kpi or tree count for clicker entry
            if (action == null) { return; }
            var entry = await TallyDataservice.InsertTallyActionAsync(action);

            // trigger updates due to tally
            await HandleTally(pop, action, entry);
        }

        protected async Task HandleTally(TallyPopulation population,
            TallyAction action, TallyEntry entry)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            TallyFeed.Add(entry);
            RaiseTallyEntryAdded();

            population.TreeCount = population.TreeCount + action.TreeCount;
            population.SumKPI = population.SumKPI + action.KPI;

            await SoundService.SignalTallyAsync();
            if (action.IsSample)
            {
                var method = population.Method;

                if (action.IsInsuranceSample)
                {
                    await SoundService.SignalInsuranceTreeAsync();
                }
                else
                {
                    await SoundService.SignalMeasureTreeAsync();
                }

                //if (CruisersDataService.PromptCruiserOnSample)
                //{
                //    var cruiser = await DialogService.AskCruiserAsync();
                //    if (cruiser != null)
                //    {
                //        Datastore.UpdateTreeInitials(entry.TreeID, cruiser);
                //    }
                //}
                //else
                if (method != CruiseMethods.H_PCT)
                {
                    var sampleType = (action.IsInsuranceSample) ? "Insurance Tree" : "Measure Tree";
                    await DialogService.ShowMessageAsync("Tree #" + entry.TreeNumber.ToString(), sampleType);
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
            && x.Species == tallyEntry.Species
            && x.LiveDead == tallyEntry.LiveDead);

            tallyPopulation.TreeCount -= tallyEntry.TreeCount;
            TallyFeed.Remove(TallyFeed.First(x => x.TallyLedgerID == tallyLedgerID));
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }
    }
}