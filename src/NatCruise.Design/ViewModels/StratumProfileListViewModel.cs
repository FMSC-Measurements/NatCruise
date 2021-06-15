using NatCruise.Data.Abstractions;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumProfileListViewModel : ViewModelBase
    {
        public readonly string[] YealdComponent_Options = new string[] { "CL", "CD", "NL", "ND", };

        private ICommand _addStratumDefaultCommand;
        private ObservableCollection<StratumDefault> _stratumDefaults;
        private StratumDefault _seletedStratumDefault;
        private IEnumerable<string> _methods;
        private IEnumerable<TreeField> _treefieldOptions;

        protected ITemplateDataservice TemplateDataservice { get; }

        public StratumProfileListViewModel(ITemplateDataservice templateDataservice, ISaleDataservice saleDataservice, ISetupInfoDataservice setupDataservice, IDialogService dialogService)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));

            TreeFieldOptions = TemplateDataservice.GetTreeFields();
            Methods = SetupDataservice.GetCruiseMethods().Select(x => x.Method).ToArray();
        }

        public string Region { get; protected set; }
        public string Forest { get; protected set; }

        public ICommand AddStratumDefaultCommand => _addStratumDefaultCommand ??= new DelegateCommand<string>(AddStratumDefault);

        public ObservableCollection<StratumDefault> StratumDefaults
        {
            get => _stratumDefaults;
            protected set
            {
                if (_stratumDefaults != null)
                {
                    foreach (var i in _stratumDefaults)
                    {
                        i.PropertyChanged -= StratumDefault_PropertyChanged;
                    }
                }

                SetProperty(ref _stratumDefaults, value);
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        i.PropertyChanged += StratumDefault_PropertyChanged;
                    }
                }
            }
        }

        private void StratumDefault_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var std = sender as StratumDefault;
            if (std != null)
            {
                TemplateDataservice.UpdateStratumDefault(std);
            }
        }

        public StratumDefault SelectedStratumDefault
        {
            get => _seletedStratumDefault;
            set => SetProperty(ref _seletedStratumDefault, value);
        }

        public IEnumerable<string> Methods
        {
            get => _methods;
            protected set => SetProperty(ref _methods, value);
        }

        public IEnumerable<string> YieldComponentOptions => YealdComponent_Options;

        public IEnumerable<TreeField> TreeFieldOptions
        {
            get => _treefieldOptions;
            protected set => SetProperty(ref _treefieldOptions, value);
        }

        public IDialogService DialogService { get; }
        public ISaleDataservice SaleDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public override void Load()
        {
            base.Load();

            var sale = SaleDataservice.GetSale();
            Region = sale.Region;
            Forest = sale.Forest;

            

            var stratumDefaults = TemplateDataservice.GetStratumDefaults();
            StratumDefaults = new ObservableCollection<StratumDefault>(stratumDefaults);
        }

        public void AddStratumDefault(string description)
        {
            if (!StratumDefaults.Any(x => description.Equals(x.Description, StringComparison.OrdinalIgnoreCase)))
            {
                var newStratumDefault = new StratumDefault
                {
                    Description = description
                };

                TemplateDataservice.AddStratumDefault(newStratumDefault);
                StratumDefaults.Add(newStratumDefault);
            }
            else
            {
                DialogService.ShowNotification("Profile Already Exists");
            }
        }
    }
}