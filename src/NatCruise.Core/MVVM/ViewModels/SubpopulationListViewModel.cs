using CommunityToolkit.Mvvm.Input;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace NatCruise.MVVM.ViewModels
{
    public class SubpopulationListViewModel : ViewModelBase
    {
        public const int SPECIES_CODE_MAX_LENGTH = 6;

        private RelayCommand<string> _addSubpopulationCommand;
        private SampleGroup _sampleGroup;
        private RelayCommand<Subpopulation> _removeSubpopulationCommand;
        private ObservableCollection<Subpopulation> _subPopulations;
        private IEnumerable<string> _speciesOptions;
        private IApplicationSettingService _appSettings;
        private bool _isSuperuserModeEnabled;
        private bool _isLocked;
        private Subpopulation _selectedSubpopulation;

        public SubpopulationListViewModel(ISpeciesDataservice speciesDataservice,
            ISampleGroupDataservice sampleGroupDataservice,
            ISubpopulationDataservice subpopulationDataservice,
            IApplicationSettingService applicationSettingService,
            INatCruiseDialogService dialogService)
        {
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SubpopulationDataservice = subpopulationDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(subpopulationDataservice));
            AppSettings = applicationSettingService ?? throw new ArgumentNullException(nameof(applicationSettingService));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public event EventHandler SubpopulationAdded;

        public ISpeciesDataservice SpeciesDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        protected ISubpopulationDataservice SubpopulationDataservice { get; }

        protected INatCruiseDialogService DialogService { get; }

        public IApplicationSettingService AppSettings
        {
            get => _appSettings;
            private set
            {
                if (_appSettings != null) { _appSettings.PropertyChanged -= AppSettings_PropertyChanged; }
                _appSettings = value;
                if (value != null)
                {
                    IsSuperuserModeEnabled = value.IsSuperuserMode;
                    _appSettings.PropertyChanged += AppSettings_PropertyChanged;
                }
                OnPropertyChanged(nameof(AppSettings));
            }
        }

        private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IApplicationSettingService.IsSuperuserMode))
            {
                var appSettings = (IApplicationSettingService)sender;
                IsSuperuserModeEnabled = appSettings.IsSuperuserMode;
            }
        }

        public bool IsSuperuserModeEnabled
        {
            get => _isSuperuserModeEnabled;
            set
            {
                SetProperty(ref _isSuperuserModeEnabled, value);
                RemoveSubpopulationCommand.NotifyCanExecuteChanged();
            }
        }

        public IEnumerable<string> LiveDeadOptions { get; } = new[] { "Default", "L", "D" };

        public IRelayCommand<string> AddSubpopulationCommand => _addSubpopulationCommand ??= new RelayCommand<string>(AddSubpopulation);
        public IRelayCommand<Subpopulation> RemoveSubpopulationCommand => _removeSubpopulationCommand ??= new RelayCommand<Subpopulation>(RemoveSubpopulation, CanRemoveSubpopulation);



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

        public Subpopulation SelectedSubpopulation
        {
            get => _selectedSubpopulation;
            set
            {
                SetProperty(ref _selectedSubpopulation, value);
                RemoveSubpopulationCommand.NotifyCanExecuteChanged();
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
            { sp.PropertyChanged += OnSubpopulationPropertyChanged; }
        }

        private void OnSubpopulationsChanging(ObservableCollection<Subpopulation> subPopulations)
        {
            if (subPopulations == null) { return; }

            foreach (var sp in subPopulations)
            { sp.PropertyChanged -= OnSubpopulationPropertyChanged; }
        }

        private void OnSubpopulationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var subpopulation = (Subpopulation)sender;

            if (subpopulation.HasTrees && !IsSuperuserModeEnabled)
            {
                throw new InvalidOperationException("Can Not Edit Subpopulation That Has Tree Data");
            }

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

        public override void Load()
        {
            var parameters = Parameters;
            if (parameters != null)
            {
                var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
                var sampleGroupCode = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);

                SampleGroup = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);
            }
        }

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
            if (species.IsNullOrEmpty()) { return; }
            species = species.Trim();
            if (Regex.IsMatch(species, "^[a-zA-Z0-9]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)) is false) { return; }

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
            newSubpopulation.PropertyChanged += OnSubpopulationPropertyChanged;

            RefreshSpeciesOptions();
        }

        private bool CanRemoveSubpopulation(Subpopulation sp)
        {
            return sp != null
                && (!sp.HasTrees || IsSuperuserModeEnabled);
        }

        public void RemoveSubpopulation(Subpopulation subpopulation)
        {
            if (subpopulation.HasTrees)
            {
                DialogService.ShowNotification($"Subpopulation: {subpopulation.SpeciesCode}|{subpopulation.LiveDead} has field data can can't be removed");
                //NotificationRequest.Raise(new Notification { Content = $"Subpopulation: {subpopulation.Species}|{subpopulation.LiveDead} has tally data can can't be removed", Title = "!" });
                return;
            }

            Subpopulations.Remove(subpopulation);
            subpopulation.PropertyChanged -= OnSubpopulationPropertyChanged;
            SubpopulationDataservice.DeleteSubpopulation(subpopulation);
        }

        //public void UpdateSubpopulation(Subpopulation subpopulation)
        //{
        //    SubpopulationDataservice.UpdateSubpopulation(subpopulation);
        //}
    }
}