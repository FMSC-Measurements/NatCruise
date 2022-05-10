using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumLogFieldSetupViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private IEnumerable<StratumTemplate> _stratumTemplates;

        protected IFieldSetupDataservice FieldSetupDataservice { get; }
        public ITemplateDataservice TemplateDataservice { get; }

        public StratumLogFieldSetupViewModel(IFieldSetupDataservice fieldSetupDataservice, ITemplateDataservice templateDataservice)
        {
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
        }

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                _stratum = value;
                OnStratumChanged(value);
                RaisePropertyChanged();
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

        public ObservableCollection<LogFieldSetup> FieldSetups
        {
            get => _fieldSetup;
            set
            {
                SetProperty(ref _fieldSetup, value);
                RaisePropertyChanged(nameof(AvalibleLogFields));
            }
        }

        public IEnumerable<LogField> LogFields
        {
            get => _logFields;
            set
            {
                SetProperty(ref _logFields, value);
                RaisePropertyChanged(nameof(AvalibleLogFields));
            }
        }

        public IEnumerable<LogField> AvalibleLogFields
        {
            get
            {
                if(FieldSetups is null || LogFields is null) { return Enumerable.Empty<LogField>(); }

                var usedFields = FieldSetups.Select(x => x.Field).ToArray();
                return LogFields.Where(x => usedFields.Contains(x.Field) == false).ToArray();
            }
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
            LogFields = TemplateDataservice.GetLogFields();
            StratumTemplates = TemplateDataservice.GetStratumTemplates();
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
                FieldOrder = FieldSetups.Count,
                StratumCode = Stratum.StratumCode,
            };
            FieldSetupDataservice.UpsertLogFieldSetup(newLfs);
            FieldSetups.Add(newLfs);
            RaisePropertyChanged(nameof(AvalibleLogFields));
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
            var otherLf = FieldSetups[newIndex];

            selectedLf.FieldOrder = newIndex;
            otherLf.FieldOrder = selectedIndex;

            FieldSetupDataservice.UpsertLogFieldSetup(selectedLf);
            FieldSetupDataservice.UpsertLogFieldSetup(otherLf);

            FieldSetups.Move(selectedIndex, newIndex);
        }

        protected void MoveUp(LogFieldSetup selectedLfs)
        {
            if (selectedLfs is null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedLfs);
            if (selectedIndex < 1) { return; }
            var newIndex = selectedIndex - 1;
            var otherLf = FieldSetups[newIndex];

            selectedLfs.FieldOrder = newIndex;
            otherLf.FieldOrder = selectedIndex;

            FieldSetupDataservice.UpsertLogFieldSetup(selectedLfs);
            FieldSetupDataservice.UpsertLogFieldSetup(otherLf);

            FieldSetups.Move(selectedIndex, newIndex);
        }
    }
}