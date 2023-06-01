using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using NatCruise.MVVM;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateLogFieldSetupViewModel : ViewModelBase
    {
        private ObservableCollection<StratumTemplateLogFieldSetup> _fieldSetup;
        private IEnumerable<LogField> _logFields;
        private StratumTemplateLogFieldSetup _selectedLogFieldSetup;
        private StratumTemplate _stratumTemplate;

        public IStratumTemplateDataservice StratumTemplateDataservice { get; }

        public StratumTemplateLogFieldSetupViewModel(IStratumTemplateDataservice stratumTemplateDataservice, ILogFieldDataservice logFieldDataservice)
        {
            StratumTemplateDataservice = stratumTemplateDataservice ?? throw new ArgumentNullException(nameof(stratumTemplateDataservice));
            LogFields = logFieldDataservice.GetLogFields();
        }

        public ICommand AddCommand => new DelegateCommand<LogField>(AddLogField);
        public ICommand RemoveCommand => new DelegateCommand<StratumTemplateLogFieldSetup>(RemoveLogField);
        public ICommand MoveUpCommand => new DelegateCommand<StratumTemplateLogFieldSetup>(MoveUp);
        public ICommand MoveDownCommand => new DelegateCommand<StratumTemplateLogFieldSetup>(MoveDown);

        public StratumTemplate StratumTemplate
        {
            get => _stratumTemplate;
            set
            {
                SetProperty(ref _stratumTemplate, value);
                LoadFieldSetups();
            }
        }

        

        public ObservableCollection<StratumTemplateLogFieldSetup> FieldSetups
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
                if (FieldSetups is null || LogFields is null) { return Enumerable.Empty<LogField>(); }

                var usedFields = FieldSetups.Select(x => x.Field).ToArray();
                return LogFields.Where(x => usedFields.Contains(x.Field) == false).ToArray();
            }
        }

        public StratumTemplateLogFieldSetup SelectedLogFieldSetup
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
            var stlfs = (StratumTemplateLogFieldSetup)sender;
            if(stlfs is null) { return; }
            StratumTemplateDataservice.UpsertStratumTemplateLogFieldSetup(stlfs);
        }

        

        private void AddLogField(LogField lf)
        {
            if(lf is null) { return; }
            var stratumTemplateName = StratumTemplate?.StratumTemplateName;
            var newstlfs = new StratumTemplateLogFieldSetup
            {
                Field = lf.Field,
                FieldOrder = FieldSetups.Count + 1,
                StratumTemplateName = stratumTemplateName,
            };

            StratumTemplateDataservice.UpsertStratumTemplateLogFieldSetup(newstlfs);
            FieldSetups.Add(newstlfs);
            RaisePropertyChanged(nameof(AvalibleLogFields));

        }

        private void MoveDown(StratumTemplateLogFieldSetup selectedLf)
        {
            if (selectedLf is null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedLf);
            if (selectedIndex == FieldSetups.Count - 1) { return; }
            var newIndex = selectedIndex + 1;
            FieldSetups.Move(selectedIndex, newIndex);
            SaveFieldOrder();
        }

        private void MoveUp(StratumTemplateLogFieldSetup selectedLf)
        {
            if (selectedLf is null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedLf);
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
                StratumTemplateDataservice.UpsertStratumTemplateLogFieldSetup(field);
            }
        }

        private void RemoveLogField(StratumTemplateLogFieldSetup lfs)
        {
            if(lfs is null) { return; }

            StratumTemplateDataservice.DeleteStratumTemplateLogFieldSetup(lfs);
            LoadFieldSetups();
        }

        protected void LoadFieldSetups()
        {
            var stratumTemplateName = StratumTemplate?.StratumTemplateName;
            if (stratumTemplateName != null)
            {
                var fieldSetups = StratumTemplateDataservice.GetStratumTemplateLogFieldSetups(stratumTemplateName);
                FieldSetups = new ObservableCollection<StratumTemplateLogFieldSetup>(fieldSetups);
            }
            else
            {
                FieldSetups = new ObservableCollection<StratumTemplateLogFieldSetup>();
            }
            RaisePropertyChanged(nameof(AvalibleLogFields));
        }
    }
}