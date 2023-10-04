using NatCruise.Data;
using NatCruise.Wpf.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class SpeciesListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSpeciesCommand;
        private DelegateCommand<Species> _deleteSpeciesCommand;
        private ObservableCollection<Species> _species;
        private IEnumerable<FIASpecies> _fiaOptions;

        public ISetupInfoDataservice SetupDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public SpeciesDetailViewModel SpeciesDetailViewModel { get; }
        public ISpeciesDataservice SpeciesDataservice { get; }
        public ILoggingService LoggingService { get; }

        public SpeciesListViewModel(ITemplateDataservice templateDataservice,
                                    ISpeciesDataservice speciesDataservice,
                                    ISetupInfoDataservice setupDataservice,
                                    INatCruiseDialogService dialogService,
                                    SpeciesDetailViewModel speciesDetailViewModel,
                                    ILoggingService loggingService)
        {
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            SpeciesDetailViewModel = speciesDetailViewModel ?? throw new ArgumentNullException(nameof(speciesDetailViewModel));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(speciesDataservice));
            LoggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
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
            get => SpeciesDetailViewModel.Species;
            set
            {
                SpeciesDetailViewModel.Species = value;
                OnPropertyChanged();
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

            Species = new ObservableCollection<Species>(SpeciesDataservice.GetSpecies());

            var speciesFiaCodes = Species.Select(x => x.FIACode).Where(x => !string.IsNullOrEmpty(x)).ToHashSet();
            var fiaOptions = SetupDataservice.GetFIASpecies().ToList();
            var optionFiaCode = fiaOptions.Select(x => x.FIACode).Where(x => !string.IsNullOrEmpty(x)).ToHashSet();

            var missingFiaCode = speciesFiaCodes.Except(optionFiaCode);

            foreach (var fia in missingFiaCode)
            {
                fiaOptions.Add(new FIASpecies{ FIACode = fia });
                LoggingService.LogEvent(nameof(SpeciesListViewModel) + ":Unrecognized FIAcode:" + fia);
            }
            fiaOptions = fiaOptions.OrderBy(x => int.TryParse(x.FIACode, out var i)? i : 0).ToList();

            FIAOptions = fiaOptions;
        }

        public void AddSpecies(string speciesCode)
        {
            speciesCode = speciesCode.Trim();
            if (Regex.IsMatch(speciesCode, "^[a-zA-Z0-9]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)) is false) { return; }

            var speciesList = Species;
            var alreadyExists = speciesList.Any(x => x.SpeciesCode.Equals(speciesCode, StringComparison.OrdinalIgnoreCase));

            if (!alreadyExists)
            {
                var newSpecies = new Species()
                {
                    SpeciesCode = speciesCode,
                };
                SpeciesDataservice.UpsertSpecies(newSpecies);
                Species.Add(newSpecies);
                SpeciesAdded?.Invoke(this, EventArgs.Empty);
                SpeciesDetailViewModel.Species = newSpecies;
            }
            else
            { DialogService.ShowNotification("Species Code Already Exists"); }
        }

        public void DeleteSpecies(Species species)
        {
            if (species is null) { throw new ArgumentNullException(nameof(species)); }

            try
            {
                SpeciesDataservice.DeleteSpecies(species.SpeciesCode);
            }
            catch (FMSC.ORM.ConstraintException)
            {
                DialogService.ShowNotification("Can Not Delete Species With Data");
            }

            
        }
    }
}