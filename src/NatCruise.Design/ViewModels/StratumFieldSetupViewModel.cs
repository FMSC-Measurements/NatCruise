using NatCruise.Data;
using NatCruise.Design.Data;
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
    public class StratumFieldSetupViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private ObservableCollection<TreeFieldSetup> _fieldSetups;
        private IEnumerable<TreeField> _treeFields;
        private TreeFieldSetup _selectedTreeFieldSetup;
        private IEnumerable<StratumTemplate> _stratumTemplates;

        public StratumFieldSetupViewModel(IDataserviceProvider dataserviceProvider)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }
            TemplateDataservice = dataserviceProvider.GetDataservice<ITemplateDataservice>() ?? throw new ArgumentNullException(nameof(TemplateDataservice));
            FieldSetupDataservice = dataserviceProvider.GetDataservice<IFieldSetupDataservice>() ?? throw new ArgumentNullException(nameof(FieldSetupDataservice));
        }

        protected IFieldSetupDataservice FieldSetupDataservice { get; }
        protected ITemplateDataservice TemplateDataservice { get; }

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

        private void OnStratumChanged(Stratum stratum)
        {
            if (stratum == null) { return; }

            LoadFieldSetups();
        }

        public IEnumerable<StratumTemplate> StratumTemplates
        {
            get => _stratumTemplates;
            set => SetProperty(ref _stratumTemplates, value);
        }

        public ICommand AddTreeFieldCommand => new DelegateCommand<TreeField>(AddTreeField);

        public ICommand RemoveTreeFieldCommand => new DelegateCommand(RemoveTreeField);

        public ICommand MoveUpCommand => new DelegateCommand(MoveUp);

        public ICommand MoveDownCommand => new DelegateCommand(MoveDown);

        public ICommand ApplyStratumTemplateCommand => new DelegateCommand<StratumTemplate>(ApplyStratumTemplate);

        public ObservableCollection<TreeFieldSetup> FieldSetups
        {
            get => _fieldSetups;
            set
            {
                SetProperty(ref _fieldSetups, value);
                RaisePropertyChanged(nameof(AvalibleTreeFields));
            }
        }

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            set => SetProperty(ref _treeFields, value);
        }

        private class TreeFieldComparer : IEqualityComparer<TreeField>
        {
            private static TreeFieldComparer _instance;
            public static TreeFieldComparer Instance => _instance ??= new TreeFieldComparer();

            public bool Equals(TreeField x, TreeField y)
            {
                return x.Field == y.Field;
            }

            public int GetHashCode(TreeField obj)
            {
                return obj.Field.GetHashCode();
            }
        }

        // todo
        public IEnumerable<TreeField> AvalibleTreeFields
        {
            get => (TreeFields != null && FieldSetups != null) ? TreeFields.Except(FieldSetups.Select(x => x.Field), TreeFieldComparer.Instance).ToArray()
                : Enumerable.Empty<TreeField>();
        }

        public TreeFieldSetup SelectedTreeFieldSetup
        {
            get => _selectedTreeFieldSetup;
            set
            {
                if (_selectedTreeFieldSetup != null) { _selectedTreeFieldSetup.PropertyChanged -= SelectedTreeFieldSetup_PropertyChanged; }
                SetProperty(ref _selectedTreeFieldSetup, value);
                if (value != null) { value.PropertyChanged += SelectedTreeFieldSetup_PropertyChanged; }
                RaisePropertyChanged(nameof(IsDefaultBoolean));
                RaisePropertyChanged(nameof(IsDefaultInt));
                RaisePropertyChanged(nameof(IsDefaultReal));
                RaisePropertyChanged(nameof(IsDefaultText));
            }
        }

        private void SelectedTreeFieldSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var fieldSetup = sender as TreeFieldSetup;
            FieldSetupDataservice.UpsertTreeFieldSetup(fieldSetup);
        }

        public bool IsDefaultReal => string.Compare(SelectedTreeFieldSetup?.Field?.DbType, "REAL", true) == 0;
        public bool IsDefaultInt => string.Compare(SelectedTreeFieldSetup?.Field?.DbType, "INTEGER", true) == 0;
        public bool IsDefaultBoolean => string.Compare(SelectedTreeFieldSetup?.Field?.DbType, "BOOLEAN", true) == 0;
        public bool IsDefaultText => string.Compare(SelectedTreeFieldSetup?.Field?.DbType, "TEXT", true) == 0;

        public void ApplyStratumTemplate(StratumTemplate st)
        {
            if (st == null) { return; }
            var stratumCode = Stratum.StratumCode;

            FieldSetupDataservice.SetTreeFieldsFromStratumTemplate(stratumCode, st.StratumTemplateName);
            LoadFieldSetups();
        }

        public void MoveDown()
        {
            var selectedTf = SelectedTreeFieldSetup;
            if (selectedTf == null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedTf);
            if (selectedIndex == FieldSetups.Count - 1) { return; }
            var newIndex = selectedIndex + 1;
            var otherTf = FieldSetups[newIndex];

            selectedTf.FieldOrder = newIndex;
            otherTf.FieldOrder = selectedIndex;

            FieldSetupDataservice.UpsertTreeFieldSetup(selectedTf);
            FieldSetupDataservice.UpsertTreeFieldSetup(otherTf);

            FieldSetups.Move(selectedIndex, newIndex);
        }

        public void MoveUp()
        {
            var selectedTf = SelectedTreeFieldSetup;
            if (selectedTf == null) { return; }
            var selectedIndex = FieldSetups.IndexOf(selectedTf);
            if (selectedIndex < 1) { return; }
            var newIndex = selectedIndex - 1;
            var otherTf = FieldSetups[newIndex];

            selectedTf.FieldOrder = newIndex;
            otherTf.FieldOrder = selectedIndex;

            FieldSetupDataservice.UpsertTreeFieldSetup(selectedTf);
            FieldSetupDataservice.UpsertTreeFieldSetup(otherTf);

            FieldSetups.Move(selectedIndex, newIndex);
        }

        private void RemoveTreeField()
        {
            var selectedTreeField = SelectedTreeFieldSetup;
            if (selectedTreeField == null) { return; }

            FieldSetupDataservice.DeleteTreeFieldSetup(selectedTreeField);
            OnStratumChanged(Stratum);
        }

        public void AddTreeField(TreeField tf)
        {
            if (tf == null) { return; }
            var newtfs = new TreeFieldSetup()
            {
                Field = tf,
                FieldOrder = FieldSetups.Count,
                StratumCode = Stratum.StratumCode,
            };
            FieldSetupDataservice.UpsertTreeFieldSetup(newtfs);
            FieldSetups.Add(newtfs);
            RaisePropertyChanged(nameof(AvalibleTreeFields));
        }

        public override void Load()
        {
            base.Load();
            TreeFields = TemplateDataservice.GetTreeFields();
            StratumTemplates = TemplateDataservice.GetStratumTemplates();
        }

        protected void LoadFieldSetups()
        {
            var stratumCode = Stratum.StratumCode;
            var fieldSetups = FieldSetupDataservice.GetTreeFieldSetups(stratumCode);
            FieldSetups = new ObservableCollection<TreeFieldSetup>(fieldSetups);
        }
    }
}