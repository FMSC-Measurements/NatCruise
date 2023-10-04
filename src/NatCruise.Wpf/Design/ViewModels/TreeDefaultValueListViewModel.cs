using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Wpf.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class TreeDefaultValueListViewModel : ViewModelBase
    {
        private ObservableCollection<TreeDefaultValue> _treeDefaultValues;
        private TreeDefaultValue _newTreeDefaultValue;
        private IEnumerable<string> _speciesCodeOptions;
        private IEnumerable<Product> _productOptions;
        private ICommand _addNewTreeDefaultValueCommand;
        private ICommand _deleteTreeDefaultValueCommand;
        private IEnumerable<string> _productCodeOptions;

        public TreeDefaultValueListViewModel(ITemplateDataservice templateDataservice, ISpeciesDataservice speciesDataservice, ISetupInfoDataservice setupInfoDataservice, INatCruiseDialogService dialogService)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(speciesDataservice));
            SetupDataservice = setupInfoDataservice ?? throw new ArgumentNullException(nameof(setupInfoDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NewTreeDefaultValue = MakeNewTreeDefaultValue();
        }

        public ITemplateDataservice TemplateDataservice { get; }
        public ISpeciesDataservice SpeciesDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

        public ICommand AddNewTreeDefaultValueCommand => _addNewTreeDefaultValueCommand ??= new DelegateCommand(AddNewTreeDefaultValue);
        public ICommand DeleteTreeDefaultValueCommand => _deleteTreeDefaultValueCommand ??= new DelegateCommand<TreeDefaultValue>(DeleteTreeDefaultValue);

        public ObservableCollection<TreeDefaultValue> TreeDefaultValues
        {
            get => _treeDefaultValues;
            set
            {
                if (_treeDefaultValues != null)
                {
                    foreach (var i in _treeDefaultValues)
                    {
                        i.PropertyChanged -= TreeDefaultValue_PropertyChanged;
                    }
                }
                SetProperty(ref _treeDefaultValues, value);
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        i.PropertyChanged += TreeDefaultValue_PropertyChanged;
                    }
                }
            }
        }

        private void TreeDefaultValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tdv = (TreeDefaultValue)sender;
            TemplateDataservice.UpsertTreeDefaultValue(tdv);
        }

        public TreeDefaultValue NewTreeDefaultValue
        {
            get => _newTreeDefaultValue;
            set => SetProperty(ref _newTreeDefaultValue, value);
        }

        public IEnumerable<string> SpeciesCodeOptions
        {
            get => _speciesCodeOptions;
            set => SetProperty(ref _speciesCodeOptions, value);
        }

        public IEnumerable<Product> ProductOptions
        {
            get => _productOptions;
            set
            {
                SetProperty(ref _productOptions, value);
                ProductCodeOptions = (value != null) ? value.Select(x => x.ProductCode).ToArray() : Enumerable.Empty<string>();
            }
        }

        public IEnumerable<string> ProductCodeOptions
        {
            get => _productCodeOptions;
            protected set => SetProperty(ref _productCodeOptions, value);
        }

        public void AddNewTreeDefaultValue()
        {
            var newTDV = NewTreeDefaultValue;
            if (newTDV == null) { return; }

            try
            {
                TemplateDataservice.AddTreeDefaultValue(newTDV);
                TreeDefaultValues.Add(newTDV);
                newTDV.PropertyChanged += TreeDefaultValue_PropertyChanged;
                NewTreeDefaultValue = MakeNewTreeDefaultValue();
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Tree Default Already Exists");
            }
        }

        protected TreeDefaultValue MakeNewTreeDefaultValue()
        {
            return new TreeDefaultValue();
        }

        public void DeleteTreeDefaultValue(TreeDefaultValue tdv)
        {
            if (tdv == null) { return; }
            TemplateDataservice.DeleteTreeDefaultValue(tdv);
            tdv.PropertyChanged -= TreeDefaultValue_PropertyChanged;
            TreeDefaultValues.Remove(tdv);
        }

        public override void Load()
        {
            base.Load();

            SpeciesCodeOptions = SpeciesDataservice.GetSpeciesCodes();
            ProductOptions = SetupDataservice.GetProducts();
            var tdvs = TemplateDataservice.GetTreeDefaultValues();
            TreeDefaultValues = new ObservableCollection<TreeDefaultValue>(tdvs);
        }
    }
}