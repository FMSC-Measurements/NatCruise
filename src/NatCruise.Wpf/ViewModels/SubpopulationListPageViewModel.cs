using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NatCruise.Wpf.ViewModels
{
    public class SubpopulationListPageViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSubpopulationCommand;
        private SampleGroup _sampleGroup;
        private DelegateCommand<Subpopulation> _removeSubpopulationCommand;
        private ObservableCollection<Subpopulation> _subPopulations;
        private IEnumerable<SpeciesListItem> _speciesOptions;

        public SubpopulationListPageViewModel(IDataserviceProvider dataserviceProvider)
        {
            SubpopulationDataservice = dataserviceProvider.GetDataservice<ISubpopulationDataservice>() ?? throw new ArgumentNullException(nameof(SubpopulationDataservice));
            SpeciesCodeDataservice = dataserviceProvider.GetDataservice<ISpeciesCodeDataservice>() ?? throw new ArgumentNullException(nameof(SpeciesCodeDataservice));
        }

        private ISubpopulationDataservice SubpopulationDataservice { get; }

        private ISpeciesCodeDataservice SpeciesCodeDataservice { get; }

        //private ISampleGroupDataservice SampleGroupDataservice { get; }

        public InteractionRequest<INotification> NotificationRequest { get; set; }

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

        public IEnumerable<SpeciesListItem> SpeciesOptions
        {
            get => _speciesOptions;
            set => SetProperty(ref _speciesOptions, value);
        }

        private void OnSubPopulationsChanged(ObservableCollection<Subpopulation> oldSubPopulations)
        {
            if (oldSubPopulations == null) { return; }

            foreach (var sp in oldSubPopulations)
            { sp.PropertyChanged -= Sp_PropertyChanged; }
        }

        private void OnSubpopulationsChanging(ObservableCollection<Subpopulation> subPopulations)
        {
            if (subPopulations == null) { return; }

            foreach (var sp in subPopulations)
            { sp.PropertyChanged += Sp_PropertyChanged; }
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
                    UpdateSubpopulation(subpopulation);
                }
            }
        }

        protected override void Load()
        {
            //if (navParams != null)
            //{
            //    var stratumCode = navParams.StratumCode;
            //    var sampleGroupCode = navParams.SampleGroupCode;

            //    SampleGroup = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);
            //}
        }

        private void OnSampleGroupChanged(SampleGroup value)
        {
            if (value == null) { return; }

            Subpopulations = new ObservableCollection<Subpopulation>(SubpopulationDataservice.GetSubpopulations(value.StratumCode, value.SampleGroupCode));

            RefreshSpeciesOptions();
        }

        protected void RefreshSpeciesOptions()
        {
            var sampleGroup = SampleGroup;
            var productSpecies = SpeciesCodeDataservice.GetSpeciesCodes(sampleGroup.PrimaryProduct).ToArray();

            var speciesCodes = SpeciesCodeDataservice.GetSpeciesCodes();

            var species = SpeciesCodeDataservice.GetSpeciesCodes()
                .Select(x => new SpeciesListItem { Species = x, HasTreeDefaultMatch = productSpecies.Contains(x) })
                .OrderBy(x => x.HasTreeDefaultMatch)
                .ToArray();

            SpeciesOptions = species;
        }

        public void AddSubpopulation(string species)
        {
            var sampleGroup = SampleGroup;
            var liveDead = sampleGroup.DefaultLiveDead;
            if (SubpopulationDataservice.Exists(sampleGroup.StratumCode, sampleGroup.SampleGroupCode, species, liveDead))
            {
                liveDead = (liveDead.Equals("L", StringComparison.InvariantCultureIgnoreCase)) ? "D" : "L";
                if (SubpopulationDataservice.Exists(sampleGroup.StratumCode, sampleGroup.SampleGroupCode, species, liveDead))
                {
                    NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {species} already exists", Title = "!" });
                    return;
                }
            }

            var newSubpopulation = new Subpopulation
            {
                StratumCode = SampleGroup.StratumCode,
                SampleGroupCode = SampleGroup.SampleGroupCode,
                LiveDead = SampleGroup.DefaultLiveDead ?? "L",
                Species = species,
            };

            SubpopulationDataservice.AddSubpopulation(newSubpopulation);
            Subpopulations.Add(newSubpopulation);
            newSubpopulation.PropertyChanged += Sp_PropertyChanged;

            RefreshSpeciesOptions();
        }

        public void RemoveSubpopulation(Subpopulation subpopulation)
        {
            if (SubpopulationDataservice.HasTreeCounts(subpopulation.StratumCode, subpopulation.SampleGroupCode, subpopulation.Species, subpopulation.LiveDead))
            {
                NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {subpopulation.Species}|{subpopulation.LiveDead} has tally data can can't be removed", Title = "!" });
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

        public class SpeciesListItem
        {
            public string Species { get; set; }

            public bool HasTreeDefaultMatch { get; set; }

            public override string ToString()
            {
                return "* " + Species;
            }
        }
    }
}