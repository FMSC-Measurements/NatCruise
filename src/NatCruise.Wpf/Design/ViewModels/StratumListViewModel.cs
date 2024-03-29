﻿using CommunityToolkit.Mvvm.Input;
using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace NatCruise.Design.ViewModels
{
    public class StratumListViewModel : ViewModelBase
    {
        private static readonly string[] RECON_CRUISEMETHODS = new[] { CruiseMethods.FIX, CruiseMethods.PNT, CruiseMethods.FIXCNT };

        private IRelayCommand _addStratumCommand;
        private IRelayCommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;
        private Stratum _selectedStratum;
        private IEnumerable<StratumTemplate> _stratumTemplateOptions;
        private StratumTemplate _selectedStratumTemplate;
        private IApplicationSettingService _appSettings;
        private bool _isSuperuserModeEnabled;

        public StratumListViewModel(IStratumDataservice stratumDataservice,
            IStratumTemplateDataservice stratumTemplateDataservice,
            IFieldSetupDataservice fieldSetupDataservice,
            ISaleDataservice saleDataservice,
            IApplicationSettingService applicationSettingService,
            INatCruiseDialogService dialogService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            StratumTemplateDataservice = stratumTemplateDataservice ?? throw new ArgumentNullException(nameof(stratumTemplateDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            SaleDataservice = saleDataservice ?? throw new ArgumentNullException(nameof(saleDataservice));
            AppSettings = applicationSettingService ?? throw new ArgumentNullException(nameof(applicationSettingService));

            StratumValidator = new StratumValidator();
        }

        public event EventHandler StratumAdded;

        protected IStratumDataservice StratumDataservice { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }
        public IStratumTemplateDataservice StratumTemplateDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public ISaleDataservice SaleDataservice { get; }
        public StratumValidator StratumValidator { get; }

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
            if (e.PropertyName == nameof(IWpfApplicationSettingService.IsSuperuserMode))
            {
                var appSettings = (IWpfApplicationSettingService)sender;
                IsSuperuserModeEnabled = appSettings.IsSuperuserMode;
            }
        }

        public bool IsSuperuserModeEnabled
        {
            get => _isSuperuserModeEnabled;
            set
            {
                SetProperty(ref _isSuperuserModeEnabled, value);
                RemoveStratumCommand.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<Stratum> Strata
        {
            get => _strata;
            protected set
            {
                if (_strata != null)
                {
                    foreach (var st in _strata)
                    {
                        st.PropertyChanged -= stratum_PropertyChanged;
                    }
                }
                SetProperty(ref _strata, value);
                if (value != null)
                {
                    foreach (var st in value)
                    {
                        st.PropertyChanged += stratum_PropertyChanged;
                    }
                }
            }
        }

        private void stratum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Stratum.Errors)) { return; }
            if (sender is Stratum st)
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
                RemoveStratumCommand.NotifyCanExecuteChanged();
                if (value != null)
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
            var stratumTemplates = StratumTemplateDataservice.GetStratumTemplates();
            var cruise = SaleDataservice.GetCruise();
            if (cruise.Purpose.Equals("Recon", StringComparison.OrdinalIgnoreCase))
            {
                stratumTemplates = stratumTemplates
                    .Where(x => RECON_CRUISEMETHODS.Contains(x.Method) || x.Method == null)
                    .ToArray();
            }

            StratumTemplateOptions = stratumTemplates;
            var strata = StratumDataservice.GetStrata().ToArray();

            foreach (var st in strata)
            {
                ValidateStratum(st);
            }

            Strata = new ObservableCollection<Stratum>(strata);
        }

        public IRelayCommand AddStratumCommand => _addStratumCommand ??= new RelayCommand<string>(AddStratum);

        public IRelayCommand RemoveStratumCommand => _removeStratumCommand ??= new RelayCommand<Stratum>(RemoveStratum, CanRemoveStratum);

        public void AddStratum(string code)
        {
            code = code.Trim();
            if (Regex.IsMatch(code, "^[a-zA-Z0-9]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)) is false) { return; }

            var newStratum = new Stratum
            {
                StratumCode = code,
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

            newStratum.YieldComponent ??= "CL";

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
                StratumAdded?.Invoke(this, EventArgs.Empty);
                SelectedStratum = newStratum;
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Stratum Code Already Exists");
            }
        }

        private bool CanRemoveStratum(Stratum st)
        {
            return st != null
                && (!st.HasTrees || IsSuperuserModeEnabled);
        }

        public void RemoveStratum(Stratum stratum)
        {
            if (stratum is null) { throw new System.ArgumentNullException(nameof(stratum)); }
            if (stratum.HasTrees && !IsSuperuserModeEnabled) { return; }

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