using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumListViewModel : ViewModelBase
    {
        private ICommand _addStratumCommand;
        private ICommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;
        private Stratum _selectedStratum;
        private IEnumerable<StratumTemplate> _stratumTemplateOptions;
        private StratumTemplate _selectedStratumTemplate;

        public StratumListViewModel(IStratumDataservice stratumDataservice, ITemplateDataservice templateDataservice, IFieldSetupDataservice fieldSetupDataservice)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
        }

        protected IStratumDataservice StratumDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }
        public ITemplateDataservice TemplateDataservice { get; }

        public ObservableCollection<Stratum> Strata
        {
            get => _strata;
            protected set => SetProperty(ref _strata, value);
        }

        public Stratum SelectedStratum
        {
            get => _selectedStratum;
            set => SetProperty(ref _selectedStratum, value);
        }

        public IEnumerable<StratumTemplate> StratumTemplateOptions
        {
            get => _stratumTemplateOptions;
            protected set => SetProperty(ref _stratumTemplateOptions, value);
        }

        public StratumTemplate SelectedStratumTemplate
        {
            get => _selectedStratumTemplate;
            set => SetProperty(ref _selectedStratumTemplate, value);
        }

        public override void Load()
        {
            StratumTemplateOptions = TemplateDataservice.GetStratumTemplates();
            var strata = StratumDataservice.GetStrata();
            Strata = new ObservableCollection<Stratum>(strata);
        }

        public ICommand AddStratumCommand => _addStratumCommand ?? (_addStratumCommand = new DelegateCommand<string>(AddStratum));

        public ICommand RemoveStratumCommand => _removeStratumCommand ?? (_removeStratumCommand = new DelegateCommand<Stratum>(RemoveStratum));

        public void AddStratum(string code)
        {
            var newStratum = new Stratum
            {
                StratumCode = code,
                YieldComponent = "CL",
            };

            var stratumTemplate = SelectedStratumTemplate;
            if (stratumTemplate != null)
            {
                newStratum.BasalAreaFactor = stratumTemplate.BasalAreaFactor;
                newStratum.Description = stratumTemplate.StratumTemplateName;
                newStratum.FBSCode = stratumTemplate.FBSCode;
                newStratum.FixCNTField = stratumTemplate.FixCNTField;
                newStratum.FixedPlotSize = stratumTemplate.FixedPlotSize;
                newStratum.HotKey = stratumTemplate.Hotkey;
                newStratum.KZ3PPNT = stratumTemplate.KZ3PPNT;
                newStratum.Method = stratumTemplate.Method;
                newStratum.SamplingFrequency = stratumTemplate.SamplingFrequency;
                newStratum.YieldComponent = stratumTemplate.YieldComponent;
            }

            StratumDataservice.AddStratum(newStratum);

            if (stratumTemplate != null)
            {
                FieldSetupDataservice.SetTreeFieldsFromStratumTemplate(code, stratumTemplate.StratumTemplateName);
            }

            Strata.Add(newStratum);
            SelectedStratum = newStratum;
        }

        public void RemoveStratum(Stratum stratum)
        {
            if (stratum is null) { throw new System.ArgumentNullException(nameof(stratum)); }
            var strata = Strata;

            StratumDataservice.DeleteStratum(stratum);
            var index = strata.IndexOf(stratum);
            if (index < 0) { return; }
            strata.RemoveAt(index);

            if (index <= strata.Count - 1)
            {
                var newSelectedStratum = strata[index];
                SelectedStratum = newSelectedStratum;
            }
            else
            {
                SelectedStratum = strata.LastOrDefault();
            }
        }
    }
}