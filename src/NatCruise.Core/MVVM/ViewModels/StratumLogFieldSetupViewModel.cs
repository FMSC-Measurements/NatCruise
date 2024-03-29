﻿using NatCruise.Data;
using NatCruise.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class StratumLogFieldSetupViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private IEnumerable<StratumTemplate> _stratumTemplates;

        protected IFieldSetupDataservice FieldSetupDataservice { get; }
        public ILogFieldDataservice LogFieldDataservice { get; }
        public IStratumTemplateDataservice StratumTemplateDataservice { get; }

        public StratumLogFieldSetupViewModel(IFieldSetupDataservice fieldSetupDataservice, ILogFieldDataservice logFieldDataservice, IStratumTemplateDataservice stratumTemplateDataservice)
        {
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            LogFieldDataservice = logFieldDataservice ?? throw new ArgumentNullException(nameof(logFieldDataservice));
            StratumTemplateDataservice = stratumTemplateDataservice ?? throw new ArgumentNullException(nameof(stratumTemplateDataservice));
        }

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                _stratum = value;
                OnStratumChanged(value);
                OnPropertyChanged();
            }
        }

        private void OnStratumChanged(Stratum st)
        {
            if(st is null) { return; }
            LoadFieldSetups();
        }

        public IEnumerable<StratumTemplate> StratumTemplates
        {
            get => _stratumTemplates;
            set => SetProperty(ref _stratumTemplates, value);
        }

        public ICommand AddLogFieldCommand => new DelegateCommand<LogField>(AddLogField);

        public ICommand RemoveLogFieldCommand => new DelegateCommand<LogFieldSetup>(RemoveLogField);

        public ICommand MoveUpCommand => new DelegateCommand<LogFieldSetup>(MoveUp);

        public ICommand MoveDownCommand => new DelegateCommand<LogFieldSetup>(MoveDown);

        public ICommand ApplyTemplateCommand => new DelegateCommand<StratumTemplate>(ApplyStratumTemplate);

        private ObservableCollection<LogFieldSetup> _fieldSetup;
        private IEnumerable<LogField> _logFields;
        private LogFieldSetup _selectedLogFieldSetup;
        private IEnumerable<LogField> _avalibleLogFields;

        public ObservableCollection<LogFieldSetup> FieldSetups
        {
            get => _fieldSetup;
            set
            {
                SetProperty(ref _fieldSetup, value);
                RefreshAvalableLogFields();
            }
        }

        public IEnumerable<LogField> LogFields
        {
            get => _logFields;
            set
            {
                SetProperty(ref _logFields, value);
                RefreshAvalableLogFields();
            }
        }

        public IEnumerable<LogField> AvalibleLogFields
        {
            get => _avalibleLogFields;
            protected set => SetProperty(ref _avalibleLogFields, value);
        }

        public LogFieldSetup SelectedLogFieldSetup
        {
            get => _selectedLogFieldSetup;
            set
            {
                if (_selectedLogFieldSetup != null)
                { _selectedLogFieldSetup.PropertyChanged += SelectedLogFieldSetup_PropertyChanged; }
                SetProperty(ref _selectedLogFieldSetup, value);
                if (value != null) { value.PropertyChanged -= SelectedLogFieldSetup_PropertyChanged; }
            }
        }

        private void SelectedLogFieldSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is LogFieldSetup lfs && lfs != null)
            {
                FieldSetupDataservice.UpsertLogFieldSetup(lfs);
            }
        }

        protected void LoadFieldSetups()
        {
            var stratumCode = Stratum.StratumCode;
            var fieldSetups = FieldSetupDataservice.GetLogFieldSetups(stratumCode);
            FieldSetups = new ObservableCollection<LogFieldSetup>(fieldSetups);
        }

        public override void Load()
        {
            base.Load();
            LogFields = LogFieldDataservice.GetLogFields();
            StratumTemplates = StratumTemplateDataservice.GetStratumTemplates();
        }

        protected void ApplyStratumTemplate(StratumTemplate st)
        {
            if(st is null) { return; }
            var stratumCode = Stratum.StratumCode;

            FieldSetupDataservice.SetLogFieldsFromStratumTemplate(stratumCode, st.StratumTemplateName);
            LoadFieldSetups();
        }

        protected void AddLogField(LogField lf)
        {
            if (lf == null) { return; }
            var newLfs = new LogFieldSetup
            {
                Field = lf.Field,
                FieldOrder = FieldSetups.Count + 1,
                StratumCode = Stratum.StratumCode,
            };
            FieldSetupDataservice.UpsertLogFieldSetup(newLfs);
            FieldSetups.Add(newLfs);
            RefreshAvalableLogFields();
        }

        protected void RemoveLogField(LogFieldSetup lfs)
        {
            if (lfs is null) { return; }

            FieldSetupDataservice.DeleteLogFieldSetup(lfs);
            LoadFieldSetups();
        }

        protected void MoveDown(LogFieldSetup selectedLf)
        {
            if (selectedLf is null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedLf);
            if (selectedIndex == FieldSetups.Count - 1) { return; }
            var newIndex = selectedIndex + 1;
            FieldSetups.Move(selectedIndex, newIndex);
            SaveFieldOrder();
        }

        protected void MoveUp(LogFieldSetup selectedLfs)
        {
            if (selectedLfs is null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedLfs);
            if (selectedIndex < 1) { return; }
            var newIndex = selectedIndex - 1;
            FieldSetups.Move(selectedIndex, newIndex);
            SaveFieldOrder();
        }

        protected void SaveFieldOrder()
        {
            var fieldSetups = FieldSetups;


            foreach (var (field, i) in fieldSetups.Select((x, i) => (x, i + 1)))
            {
                field.FieldOrder = i;
                FieldSetupDataservice.UpsertLogFieldSetup(field);
            }
        }

        protected void RefreshAvalableLogFields()
        {
            if (FieldSetups is null || LogFields is null)
            {
                AvalibleLogFields = Enumerable.Empty<LogField>();
            }
            else
            {
                var usedFields = FieldSetups.Select(x => x.Field).ToArray();
                AvalibleLogFields = LogFields.Where(x => usedFields.Contains(x.Field) == false).ToArray();
            }
        }
    }
}