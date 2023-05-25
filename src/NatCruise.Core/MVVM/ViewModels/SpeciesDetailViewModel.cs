using Microsoft.AppCenter.Crashes;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Util;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class SpeciesDetailViewModel : ViewModelBase
    {
        public Product DefaultProductOption { get; } = new Product
        {
            FriendlyName = "default",
            ProductCode = null,
        };

        private Species _species;
        private IEnumerable<FIASpecies> _fiaOptions;
        private ObservableCollection<SpeciesProduct> _contractSpecies;
        private ICommand _addContractSpeciesCommand;
        private DelegateCommand<SpeciesProduct> _removeContractSpeciesCommand;
        private IEnumerable<Product> _avalableProductOptions;

        public ISpeciesDataservice SpeciesDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public SpeciesDetailViewModel(ISpeciesDataservice speciesDataservice, ISetupInfoDataservice setupDataservice)
        {
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(speciesDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));

            FIAOptions = SetupDataservice.GetFIASpecies();
            ProductOptions = SetupDataservice.GetProducts().Prepend(DefaultProductOption);
        }

        public event EventHandler ContractSpeciesAdded;

        public Species Species
        {
            get => _species;
            set
            {
                if (_species != null)
                { _species.PropertyChanged -= _species_PropertyChanged; }
                _species = value;
                if (value != null)
                { value.PropertyChanged += _species_PropertyChanged; }

                RefreshContractSpecies();
                RefreshAvalableProductOptions();
                RaisePropertyChanged();

                void _species_PropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    var sp = (Species)sender;
                    SpeciesDataservice.UpsertSpecies(sp);
                }
            }
        }

        public IEnumerable<FIASpecies> FIAOptions
        {
            get => _fiaOptions;
            protected set => SetProperty(ref _fiaOptions, value);
        }

        public IEnumerable<Product> ProductOptions { get; }

        public IEnumerable<Product> AvalableProductOptions
        {
            get => _avalableProductOptions;
            protected set => SetProperty(ref _avalableProductOptions, value);
        }

        public ObservableCollection<SpeciesProduct> ContractSpecies
        {
            get => _contractSpecies;
            set => SetProperty(ref _contractSpecies, value);
        }

        public ICommand AddContractSpeciesCommand => _addContractSpeciesCommand ??= new DelegateCommand<string[]>(p => AddContractSpecies(p[0], p[1]));

        public ICommand RemoveContractSpeciesCommand => _removeContractSpeciesCommand ??= new DelegateCommand<SpeciesProduct>(spProd => RemoveContractSpecies(spProd));

        public void AddContractSpecies(string prod, string contrSp)
        {
            try
            {
                var spProd = SpeciesDataservice.AddSpeciesProduct(Species.SpeciesCode, prod, contrSp);
                //ContractSpecies.Add(spProd);
                ContractSpeciesAdded?.Invoke(this, EventArgs.Empty);
                RefreshContractSpecies();
                RefreshAvalableProductOptions();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void RemoveContractSpecies(SpeciesProduct spProd)
        {
            if (spProd == null) throw new ArgumentNullException(nameof(spProd));

            SpeciesDataservice.DeleteSpeciesProduct(spProd);
            RefreshContractSpecies();
            RefreshAvalableProductOptions();
        }

        protected void RefreshAvalableProductOptions()
        {
            var prodOptions = ProductOptions;
            var usedProds = ContractSpecies?.Select(x => x.PrimaryProduct);

            var availProdOptions = ProductOptions.Where(x => !usedProds.Contains(x.ProductCode))
                .ToArray();
            AvalableProductOptions = availProdOptions;
        }

        protected void RefreshContractSpecies()
        {
            var species = Species;
            if (species != null)
            {
                ContractSpecies = SpeciesDataservice.GetSpeciesProducts(species.SpeciesCode)
                        .ToObservableCollection();
            }
        }


    }
}
