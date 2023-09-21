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
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateListViewModel : ViewModelBase
    {
        public readonly string[] YieldComponent_Options = new string[] { "CL", "CD", "NL", "ND", };

        private ICommand _addStratumTemplateCommand;
        private ObservableCollection<StratumTemplate> _stratumTemplates;
        private StratumTemplate _seletedStratumTemplate;
        private IEnumerable<string> _methods;
        private IEnumerable<TreeField> _treefieldOptions;

        public event EventHandler StratumTemplateAdded;

        protected IStratumTemplateDataservice StratumTemplateDataservice { get; }

        public StratumTemplateListViewModel(IStratumTemplateDataservice stratumTemplateDataservice, ITreeFieldDataservice treeFieldDataservice, ISaleDataservice saleDataservice, ISetupInfoDataservice setupDataservice, INatCruiseDialogService dialogService)
        {
            StratumTemplateDataservice = stratumTemplateDataservice ?? throw new ArgumentNullException(nameof(stratumTemplateDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));

            TreeFieldOptions = treeFieldDataservice.GetTreeFields();
            Methods = SetupDataservice.GetCruiseMethods().Select(x => x.Method).ToArray();
        }

        public string Region { get; protected set; }
        public string Forest { get; protected set; }

        public ICommand AddStratumTemplateCommand => _addStratumTemplateCommand ??= new DelegateCommand<string>(AddStratumTemplate);

        public ObservableCollection<StratumTemplate> StratumTemplates
        {
            get => _stratumTemplates;
            protected set
            {
                if (_stratumTemplates != null)
                {
                    foreach (var i in _stratumTemplates)
                    {
                        i.PropertyChanged -= StratumTemplate_PropertyChanged;
                    }
                }

                SetProperty(ref _stratumTemplates, value);
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        i.PropertyChanged += StratumTemplate_PropertyChanged;
                    }
                }
            }
        }

        private void StratumTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var st = sender as StratumTemplate;
            if (st != null)
            {
                StratumTemplateDataservice.UpsertStratumTemplate(st);
            }
        }

        public StratumTemplate SelectedStratumTemplate
        {
            get => _seletedStratumTemplate;
            set => SetProperty(ref _seletedStratumTemplate, value);
        }

        public IEnumerable<string> Methods
        {
            get => _methods;
            protected set => SetProperty(ref _methods, value);
        }

        public IEnumerable<string> YieldComponentOptions => YieldComponent_Options;

        public IEnumerable<TreeField> TreeFieldOptions
        {
            get => _treefieldOptions;
            protected set => SetProperty(ref _treefieldOptions, value);
        }

        public INatCruiseDialogService DialogService { get; }
        public ISaleDataservice SaleDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public override void Load()
        {
            base.Load();

            var sale = SaleDataservice.GetSale();
            Region = sale.Region;
            Forest = sale.Forest;

            var stratumTemplate = StratumTemplateDataservice.GetStratumTemplates();
            StratumTemplates = new ObservableCollection<StratumTemplate>(stratumTemplate);
        }

        public void AddStratumTemplate(string name)
        {
            if (!StratumTemplates.Any(x => name.Equals(x.StratumTemplateName, StringComparison.OrdinalIgnoreCase)))
            {
                var newStratumTemplate = new StratumTemplate
                {
                    StratumTemplateName = name
                };

                StratumTemplateDataservice.UpsertStratumTemplate(newStratumTemplate);
                StratumTemplates.Add(newStratumTemplate);
                SelectedStratumTemplate = newStratumTemplate;
                StratumTemplateAdded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                DialogService.ShowNotification("Template Already Exists");
            }
        }
    }
}