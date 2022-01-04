using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class TreeDefaultValueListViewModel : ViewModelBase
    {
        private ObservableCollection<TreeDefaultValue> _treeDefaultValues;
        private TreeDefaultValue _newTreeDefaultValue = new TreeDefaultValue();
        private IEnumerable<string> _speciesCodeOptions;
        private IEnumerable<Product> _productOptions;
        private ICommand _addNewTreeDefaultValueCommand;
        private ICommand _deleteTreeDefaultValueCommand;

        public TreeDefaultValueListViewModel(ITemplateDataservice templateDataservice, ISetupInfoDataservice setupInfoDataservice, IDialogService dialogService)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            SetupDataservice = setupInfoDataservice ?? throw new ArgumentNullException(nameof(setupInfoDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public ITemplateDataservice TemplateDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public IDialogService DialogService { get; }

        public ICommand AddNewTreeDefaultValueCommand => _addNewTreeDefaultValueCommand ?? new DelegateCommand(AddNewTreeDefaultValue);
        public ICommand DeleteTreeDefaultValueCommand => _deleteTreeDefaultValueCommand ?? new DelegateCommand<TreeDefaultValue>(DeleteTreeDefaultValue);

        public ObservableCollection<TreeDefaultValue> TreeDefaultValues
        {
            get => _treeDefaultValues;
            set
            {
                if(_treeDefaultValues != null)
                {
                    foreach(var i in _treeDefaultValues)
                    {

                    }
                }
                SetProperty(ref _treeDefaultValues, value);
                if(value != null)
                {
                    foreach(var i in value)
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
                RaisePropertyChanged(nameof(ProductCodeOptions));
            }
        }

        public IEnumerable<string> ProductCodeOptions => ProductOptions?.Select(x => x.ProductCode).ToArray();

        public void AddNewTreeDefaultValue()
        {
            var newTDV = NewTreeDefaultValue;
            if(newTDV == null) { return; }

            try
            {
                TemplateDataservice.AddTreeDefaultValue(newTDV);
                TreeDefaultValues.Add(newTDV);
                newTDV.PropertyChanged += TreeDefaultValue_PropertyChanged;
                NewTreeDefaultValue = new TreeDefaultValue();
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Tree Default Already Exists");
            }
        }

        public void DeleteTreeDefaultValue(TreeDefaultValue tdv)
        {
            if(tdv == null) { return; }
            TemplateDataservice.DeleteTreeDefaultValue(tdv);
            tdv.PropertyChanged -= TreeDefaultValue_PropertyChanged;
            TreeDefaultValues.Remove(tdv);
        }

        public override void Load()
        {
            base.Load();

            SpeciesCodeOptions = TemplateDataservice.GetSpeciesCodes();
            ProductOptions = SetupDataservice.GetProducts();
            var tdvs = TemplateDataservice.GetTreeDefaultValues();
            TreeDefaultValues = new ObservableCollection<TreeDefaultValue>(tdvs);
        }
    }
}
