using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Validation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class StratumTreeFieldSetupViewModel : ValidationViewModelBase
    {
        private Stratum _stratum;
        private ObservableCollection<TreeFieldSetup> _fieldSetups;
        private IEnumerable<TreeField> _treeFields;
        private TreeFieldSetup _selectedTreeFieldSetup;
        private IEnumerable<StratumTemplate> _stratumTemplates;
        private IEnumerable<TreeField> _avalibleTreeFields;

        public StratumTreeFieldSetupViewModel(
            IStratumTemplateDataservice templateDataservice,
            IFieldSetupDataservice fieldSetupDataservice,
            ITreeFieldDataservice treeFieldDataservice,
            TreeFieldSetupValidator treeFieldSetupValidator)
            : base(treeFieldSetupValidator)
        {
            StratumTemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
        }

        public event EventHandler TreeFieldAdded;

        protected IFieldSetupDataservice FieldSetupDataservice { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }
        protected IStratumTemplateDataservice StratumTemplateDataservice { get; }

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

        public ICommand MoveUpCommand => new DelegateCommand<TreeFieldSetup>(MoveUp);

        public ICommand MoveDownCommand => new DelegateCommand<TreeFieldSetup>(MoveDown);

        public ICommand MoveSelectedUpCommand => new DelegateCommand(MoveUp);

        public ICommand MoveSelectedDownCommand => new DelegateCommand(MoveDown);

        public ICommand ApplyStratumTemplateCommand => new DelegateCommand<StratumTemplate>(ApplyStratumTemplate);

        public ObservableCollection<TreeFieldSetup> FieldSetups
        {
            get => _fieldSetups;
            set
            {
                SetProperty(ref _fieldSetups, value);
                RefreshAvalableTreeFields();
            }
        }

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            set
            {
                SetProperty(ref _treeFields, value);
                RefreshAvalableTreeFields();
            }
        }



        public IEnumerable<TreeField> AvalibleTreeFields
        {
            get => _avalibleTreeFields;
            protected set => SetProperty(ref _avalibleTreeFields, value);
        }

        public TreeFieldSetup SelectedTreeFieldSetup
        {
            get => _selectedTreeFieldSetup;
            set
            {
                //if (_selectedTreeFieldSetup != null) { _selectedTreeFieldSetup.PropertyChanged -= SelectedTreeFieldSetup_PropertyChanged; }
                SetProperty(ref _selectedTreeFieldSetup, value);
                //if (value != null) { value.PropertyChanged += SelectedTreeFieldSetup_PropertyChanged; }
                ValidateAll(value);
                RaisePropertyChanged(nameof(IsDefaultBoolean));
                RaisePropertyChanged(nameof(IsDefaultInt));
                RaisePropertyChanged(nameof(IsDefaultReal));
                RaisePropertyChanged(nameof(IsDefaultText));
                RaisePropertyChanged(nameof(DefaultValueBool));
                RaisePropertyChanged(nameof(DefaultValueInt));
                RaisePropertyChanged(nameof(DefaultValueReal));
                RaisePropertyChanged(nameof(DefaultValueText));
                RaisePropertyChanged(nameof(IsLocked));
                RaisePropertyChanged(nameof(IsHidden));
                
            }
        }

        private void SelectedTreeFieldSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var fieldSetup = sender as TreeFieldSetup;

            FieldSetupDataservice.UpsertTreeFieldSetup(fieldSetup);
        }

        public bool IsLocked
        {
            get => SelectedTreeFieldSetup?.IsLocked ?? false;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if (tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.IsLocked = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
        }

        public bool IsHidden
        {
            get => SelectedTreeFieldSetup?.IsHidden ?? false;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if (tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.IsHidden = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
        }

        public bool? DefaultValueBool
        {
            get => SelectedTreeFieldSetup?.DefaultValueBool;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if (tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.DefaultValueBool = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
        }

        public int? DefaultValueInt
        {
            get => SelectedTreeFieldSetup?.DefaultValueInt;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if (tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.DefaultValueInt = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
        }

        public double? DefaultValueReal
        {
            get => SelectedTreeFieldSetup?.DefaultValueReal;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if (tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.DefaultValueReal = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
        }

        public string DefaultValueText
        {
            get => SelectedTreeFieldSetup?.DefaultValueText;
            set
            {
                var tfs = SelectedTreeFieldSetup;
                if(tfs != null)
                {
                    SetPropertyAndValidate(tfs, value, (m, x) => m.DefaultValueText = x);
                    FieldSetupDataservice.UpsertTreeFieldSetup(tfs);
                }
            }
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
            MoveDown(selectedTf);
        }

        public void MoveDown(TreeFieldSetup tfs)
        {
            var tfsIndex = FieldSetups.IndexOf(tfs);
            if (tfsIndex == FieldSetups.Count - 1) { return; }
            var newIndex = tfsIndex + 1;
            FieldSetups.Move(tfsIndex, newIndex);
            SaveFieldOrder();
        }

        public void MoveUp()
        {
            var selectedTf = SelectedTreeFieldSetup;
            if (selectedTf == null) { return; }
            MoveUp(selectedTf);
        }

        public void MoveUp(TreeFieldSetup tfs)
        {
            var tfsIndex = FieldSetups.IndexOf(tfs);
            if (tfsIndex < 1) { return; }
            var newIndex = tfsIndex - 1;
            FieldSetups.Move(tfsIndex, newIndex);
            SaveFieldOrder();
        }

        protected void SaveFieldOrder()
        {
            var fieldSetups = FieldSetups;
            

            foreach (var (field, i) in fieldSetups.Select((x, i) => (x, i + 1)))
            {
                field.FieldOrder = i;
                FieldSetupDataservice.UpsertTreeFieldSetup(field);
            }
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
            if(FieldSetups.Any(x => x.Field.Field == tf.Field)) { return; }
            var newtfs = new TreeFieldSetup()
            {
                Field = tf,
                FieldOrder = FieldSetups.Count + 1,
                StratumCode = Stratum.StratumCode,
            };
            FieldSetupDataservice.UpsertTreeFieldSetup(newtfs);
            FieldSetups.Add(newtfs);
            TreeFieldAdded?.Invoke(this, EventArgs.Empty);
            RefreshAvalableTreeFields();
        }

        public override void Load()
        {
            base.Load();

            TreeFields = TreeFieldDataservice.GetTreeFields();
            StratumTemplates = StratumTemplateDataservice.GetStratumTemplates();
        }

        protected void LoadFieldSetups()
        {
            var stratumCode = Stratum.StratumCode;
            var fieldSetups = FieldSetupDataservice.GetTreeFieldSetups(stratumCode);
            FieldSetups = new ObservableCollection<TreeFieldSetup>(fieldSetups);
        }

        protected void RefreshAvalableTreeFields()
        {
            AvalibleTreeFields = (TreeFields != null && FieldSetups != null) ? TreeFields.Except(FieldSetups.Select(x => x.Field), TreeFieldComparer.Instance).ToArray()
                : Enumerable.Empty<TreeField>();
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
    }
}