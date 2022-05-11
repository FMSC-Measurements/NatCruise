using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
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
    public class StratumListViewModel : ViewModelBase
    {
        private ICommand _addStratumCommand;
        private ICommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;
        private Stratum _selectedStratum;
        private IEnumerable<StratumTemplate> _stratumTemplateOptions;
        private StratumTemplate _selectedStratumTemplate;

        public StratumListViewModel(IStratumDataservice stratumDataservice, ITemplateDataservice templateDataservice, IFieldSetupDataservice fieldSetupDataservice, INatCruiseDialogService dialogService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            StratumValidator = new StratumValidator();
        }

        protected IStratumDataservice StratumDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }
        public ITemplateDataservice TemplateDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public StratumValidator StratumValidator { get; }
        public ObservableCollection<Stratum> Strata
        {
            get => _strata;
            protected set
            {
                if(_strata != null)
                {
                    foreach(var st in _strata)
                    {
                        st.PropertyChanged -= stratum_PropertyChanged;
                    }
                }
                SetProperty(ref _strata, value);
                if(value != null)
                {
                    foreach(var st in value)
                    {
                        st.PropertyChanged += stratum_PropertyChanged;
                    }
                }
            }
        }

        private void stratum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Stratum.Errors)) { return; }
            if(sender is Stratum st && st != null)
            {
                ValidateStratum(st);
            }
            
        }

        public Stratum SelectedStratum
        {
            get => _selectedStratum;
            set
            {
                SetProperty(ref _selectedStratum, value);
                if(value != null)
                {
                    ValidateStratum(value);
                }
            }
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
            var strata = StratumDataservice.GetStrata().ToArray();

            foreach (var st in strata)
            {
                ValidateStratum(st);
            }

            Strata = new ObservableCollection<Stratum>(strata);

            
        }

        public ICommand AddStratumCommand => _addStratumCommand ?? (_addStratumCommand = new DelegateCommand<string>(AddStratum));

        public ICommand RemoveStratumCommand => _removeStratumCommand ?? (_removeStratumCommand = new DelegateCommand<Stratum>(RemoveStratum));

        public void AddStratum(string code)
        {
            code = code.Trim();
            if (Regex.IsMatch(code, "^[a-zA-Z0-9]+$") is false) { return; }

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

            try
            {
                StratumDataservice.AddStratum(newStratum);

                if (stratumTemplate != null)
                {
                    FieldSetupDataservice.SetTreeFieldsFromStratumTemplate(code, stratumTemplate.StratumTemplateName);
                    FieldSetupDataservice.SetLogFieldsFromStratumTemplate(code, stratumTemplate.StratumTemplateName);
                }

                newStratum.PropertyChanged += stratum_PropertyChanged;
                Strata.Add(newStratum);
                SelectedStratum = newStratum;
            }
            catch(FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Stratum Code Already Exists");
            }
        }

        public void RemoveStratum(Stratum stratum)
        {
            if (stratum is null) { throw new System.ArgumentNullException(nameof(stratum)); }
            var strata = Strata;

            StratumDataservice.DeleteStratum(stratum);
            var index = strata.IndexOf(stratum);
            if (index < 0) { return; }
            strata.RemoveAt(index);
            stratum.PropertyChanged -= stratum_PropertyChanged;

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

        public void ValidateStratum(Stratum stratum)
        {
            if (stratum is null) { throw new ArgumentNullException(nameof(stratum)); }

            var errors = StratumValidator.Validate(stratum).Errors
                .Where(x => x.Severity == FluentValidation.Severity.Error)
                .Select(x => x.ErrorMessage).ToArray();

            if (errors.Length > 0)
            { stratum.Errors = errors; }
            else
            { stratum.Errors = Enumerable.Empty<string>(); }
        }
    }
}