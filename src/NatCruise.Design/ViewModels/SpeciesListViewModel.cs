using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class SpeciesListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSpeciesCommand;
        private DelegateCommand<Species> _updateSpeciesCommand;
        private DelegateCommand<Species> _deleteSpeciesCommand;
        private ObservableCollection<Species> _species;
        private IEnumerable<FIASpecies> _fiaOptions;
        private Species _selectedSpecies;

        public ITemplateDataservice TemplateDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

        public SpeciesListViewModel(ITemplateDataservice templateDataservice, ISetupInfoDataservice setupDataservice, INatCruiseDialogService dialogService)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public event EventHandler SpeciesAdded;

        public ICommand AddSpeciesCommand => _addSpeciesCommand ??= new DelegateCommand<string>(AddSpecies);

        public ICommand DeleteSpeciesCommand => _deleteSpeciesCommand ??= new DelegateCommand<Species>(DeleteSpecies);

        public ObservableCollection<Species> Species
        {
            get => _species;
            protected set => SetProperty(ref _species, value);
        }

        public Species SelectedSpecies
        {
            get => _selectedSpecies;
            set
            {
                if (_selectedSpecies != null)
                { _selectedSpecies.PropertyChanged -= SelectedSpecies_PropertyChanged; }
                _selectedSpecies = value;
                if (value != null)
                { value.PropertyChanged += SelectedSpecies_PropertyChanged; }
                RaisePropertyChanged();

                void SelectedSpecies_PropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    var species = (Species)sender;
                    UpdateSpecies(species);
                }
            }
        }

        public IEnumerable<FIASpecies> FIAOptions
        {
            get => _fiaOptions;
            protected set => SetProperty(ref _fiaOptions, value);
        }

        public override void Load()
        {
            base.Load();

            Species = new ObservableCollection<Species>(TemplateDataservice.GetSpecies());
            FIAOptions = SetupDataservice.GetFIASpecies();
        }

        public void AddSpecies(string speciesCode)
        {
            speciesCode = speciesCode.Trim();
            if (Regex.IsMatch(speciesCode, "^[a-zA-Z0-9]+$") is false) { return; }

            var speciesList = Species;
            var alreadyExists = speciesList.Any(x => x.SpeciesCode.Equals(speciesCode, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists == false)
            {
                var newSpecies = new Species()
                {
                    SpeciesCode = speciesCode,
                };
                TemplateDataservice.UpsertSpecies(newSpecies);
                Species.Add(newSpecies);
                SpeciesAdded?.Invoke(this, EventArgs.Empty);
                SelectedSpecies = newSpecies;
            }
            else
            { DialogService.ShowNotification("Species Code Already Exists"); }
        }

        public void UpdateSpecies(Species species)
        {
            if (species is null) { throw new ArgumentNullException(nameof(species)); }

            TemplateDataservice.UpsertSpecies(species);
        }

        public void DeleteSpecies(Species species)
        {
            if (species is null) { throw new ArgumentNullException(nameof(species)); }

            TemplateDataservice.DeleteSpecies(species.SpeciesCode);
        }
    }
}