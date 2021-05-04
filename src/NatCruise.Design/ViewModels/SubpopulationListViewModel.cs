using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class SubpopulationListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSubpopulationCommand;
        private SampleGroup _sampleGroup;
        private DelegateCommand<Subpopulation> _removeSubpopulationCommand;
        private ObservableCollection<Subpopulation> _subPopulations;
        private IEnumerable<string> _speciesOptions;

        public SubpopulationListViewModel(IDataserviceProvider dataserviceProvider, IDialogService dialogService)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            SubpopulationDataservice = dataserviceProvider.GetDataservice<ISubpopulationDataservice>() ?? throw new ArgumentNullException(nameof(SubpopulationDataservice));
            TemplateDataservice = dataserviceProvider.GetDataservice<ITemplateDataservice>() ?? throw new ArgumentNullException(nameof(TemplateDataservice));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        protected ITemplateDataservice TemplateDataservice { get; }

        protected ISubpopulationDataservice SubpopulationDataservice { get; }

        protected IDialogService DialogService { get; }

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
            var speciesCodes = TemplateDataservice.GetSpeciesCodes();

            SpeciesOptions = speciesCodes;
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
                    DialogService.ShowNotification($"Subpopulation: {species} already exists");
                    //NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {species} already exists", Title = "!" });
                    return;
                }
            }

            var newSubpopulation = new Subpopulation
            {
                StratumCode = SampleGroup.StratumCode,
                SampleGroupCode = SampleGroup.SampleGroupCode,
                LiveDead = SampleGroup.DefaultLiveDead ?? "L",
                SpeciesCode = species,
            };

            SubpopulationDataservice.AddSubpopulation(newSubpopulation);
            Subpopulations.Add(newSubpopulation);
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