using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace NatCruise.Design.ViewModels
{
    public class SubpopulationListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSubpopulationCommand;
        private SampleGroup _sampleGroup;
        private DelegateCommand<Subpopulation> _removeSubpopulationCommand;
        private ObservableCollection<Subpopulation> _subPopulations;
        private IEnumerable<string> _speciesOptions;
        private bool _isFixCNT;

        public SubpopulationListViewModel(ISpeciesDataservice speciesDataservice, ISubpopulationDataservice subpopulationDataservice, INatCruiseDialogService dialogService)
        {
            SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public event EventHandler SubpopulationAdded;

        public ISpeciesDataservice SpeciesDataservice { get; }
        protected ISubpopulationDataservice SubpopulationDataservice { get; }

        protected INatCruiseDialogService DialogService { get; }

        //public InteractionRequest<INotification> NotificationRequest { get; set; }

        public IEnumerable<string> LiveDeadOptions { get; } = new[] { "Default", "L", "D" };

        public DelegateCommand<string> AddSubpopulationCommand => _addSubpopulationCommand ?? (_addSubpopulationCommand = new DelegateCommand<string>(AddSubpopulation));
        public DelegateCommand<Subpopulation> RemoveSubpopulationCommand => _removeSubpopulationCommand ?? (_removeSubpopulationCommand = new DelegateCommand<Subpopulation>(RemoveSubpopulation));

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            set
            {
                SetProperty(ref _sampleGroup, value);
                OnSampleGroupChanged(value);
            }
        }

        public ObservableCollection<Subpopulation> Subpopulations
        {
            get => _subPopulations;
            protected set
            {
                OnSubpopulationsChanging(_subPopulations);
                SetProperty(ref _subPopulations, value);
                OnSubPopulationsChanged(value);
            }
        }

        public IEnumerable<string> SpeciesOptions
        {
            get => _speciesOptions;
            set => SetProperty(ref _speciesOptions, value);
        }

        private void OnSubPopulationsChanged(ObservableCollection<Subpopulation> oldSubPopulations)
        {
            if (oldSubPopulations == null) { return; }

            foreach (var sp in oldSubPopulations)
            { sp.PropertyChanged += Sp_PropertyChanged; }
        }

        private void OnSubpopulationsChanging(ObservableCollection<Subpopulation> subPopulations)
        {
            if (subPopulations == null) { return; }

            foreach (var sp in subPopulations)
            { sp.PropertyChanged -= Sp_PropertyChanged; }
        }

        private void Sp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var subpopulation = (Subpopulation)sender;

            var propertyName = e.PropertyName;
            if (propertyName == nameof(Subpopulation.LiveDead))
            {
                var liveDead = subpopulation.LiveDead;
                if (liveDead != "L" && liveDead != "D")
                {
                    //setting live dead will cause property changed to fire again;
                    subpopulation.LiveDead = SampleGroup.DefaultLiveDead;
                }
                else
                {
                    if (SubpopulationDataservice.Exists(subpopulation.StratumCode, subpopulation.SampleGroupCode, subpopulation.SpeciesCode, liveDead))
                    {
                        DialogService.ShowNotification($"Subpopulation already exists");
                        //NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {species} already exists", Title = "!" });
                        return;
                    }
                    SubpopulationDataservice.UpdateSubpopulation(subpopulation);
                }
            }
            if (propertyName == nameof(Subpopulation.IntervalSize)
                || propertyName == nameof(Subpopulation.Min)
                || propertyName == nameof(Subpopulation.Max))
            {
                SubpopulationDataservice.UpsertFixCNTTallyPopulation(subpopulation);
            }
        }

        //protected override void Load()
        //{
        //    //if (navParams != null)
        //    //{
        //    //    var stratumCode = navParams.StratumCode;
        //    //    var sampleGroupCode = navParams.SampleGroupCode;

        //    //    SampleGroup = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);
        //    //}
        //}

        private void OnSampleGroupChanged(SampleGroup value)
        {
            if (value == null)
            {
                Subpopulations = new ObservableCollection<Subpopulation>();
            }
            else
            {
                var subpopulations = SubpopulationDataservice.GetSubpopulations(value.StratumCode, value.SampleGroupCode);
                Subpopulations = new ObservableCollection<Subpopulation>(subpopulations);
            }

            RefreshSpeciesOptions();
        }

        protected void RefreshSpeciesOptions()
        {
            var speciesCodes = SpeciesDataservice.GetSpeciesCodes();

            SpeciesOptions = speciesCodes;
        }

        public void AddSubpopulation(string species)
        {
            species = species.Trim();
            if (Regex.IsMatch(species, "^[a-zA-Z0-9]+$") is false) { return; }

            //var speciesList = SpeciesOptions;
            //var alreadyExists = speciesList.Any(x => x.Equals(species, StringComparison.OrdinalIgnoreCase));

            //if (alreadyExists == false)
            //{
            //    var newSpecies = new Species()
            //    {
            //        SpeciesCode = species,
            //    };
            //    TemplateDataservice.UpsertSpecies(newSpecies);
            //    SpeciesOptions.Add(species);
            //}

            var sampleGroup = SampleGroup;
            var liveDead = sampleGroup.DefaultLiveDead ?? "L";
            if (SubpopulationDataservice.Exists(sampleGroup.StratumCode, sampleGroup.SampleGroupCode, species, liveDead))
            {
                liveDead = (liveDead.Equals("L", StringComparison.InvariantCultureIgnoreCase)) ? "D" : "L";
                if (SubpopulationDataservice.Exists(sampleGroup.StratumCode, sampleGroup.SampleGroupCode, species, liveDead))
                {
                    DialogService.ShowNotification($"Subpopulation: {species} already exists");
                    //NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {species} already exists", Title = "!" });
                    return;
                }
            }

            var newSubpopulation = new Subpopulation
            {
                StratumCode = SampleGroup.StratumCode,
                SampleGroupCode = SampleGroup.SampleGroupCode,
                LiveDead = liveDead,
                SpeciesCode = species,
            };

            SubpopulationDataservice.AddSubpopulation(newSubpopulation);
            Subpopulations.Add(newSubpopulation);
            SubpopulationAdded?.Invoke(this, EventArgs.Empty);
            newSubpopulation.PropertyChanged += Sp_PropertyChanged;

            RefreshSpeciesOptions();
        }

        public void RemoveSubpopulation(Subpopulation subpopulation)
        {
            if (SubpopulationDataservice.HasTreeCounts(subpopulation.StratumCode, subpopulation.SampleGroupCode, subpopulation.SpeciesCode, subpopulation.LiveDead))
            {
                DialogService.ShowNotification($"Subpopulation: {subpopulation.SpeciesCode}|{subpopulation.LiveDead} has tally data can can't be removed");
                //NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {subpopulation.Species}|{subpopulation.LiveDead} has tally data can can't be removed", Title = "!" });
                return;
            }

            Subpopulations.Remove(subpopulation);
            subpopulation.PropertyChanged -= Sp_PropertyChanged;
            SubpopulationDataservice.DeleteSubpopulation(subpopulation);
        }

        public void UpdateSubpopulation(Subpopulation subpopulation)
        {
            SubpopulationDataservice.UpdateSubpopulation(subpopulation);
        }
    }
}