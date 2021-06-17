using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NatCruise.Util;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateFieldsViewModel : ViewModelBase
    {
        private StratumDefault _stratumDefault;
        private ObservableCollection<TreeFieldSetupDefault> _fieldSetups;
        private IEnumerable<TreeField> _treeFields;
        private TreeFieldSetupDefault _selectedTreeFieldSetup;
        private ICommand _moveUpCommand;
        private ICommand _moveDownCommand;
        private ICommand _addCommand;
        private ICommand _removeCommand;

        public StratumTemplateFieldsViewModel(ITemplateDataservice templateDataservice)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));

            TreeFields = TemplateDataservice.GetTreeFields();
        }

        public ICommand AddCommand => _addCommand ??= new DelegateCommand<TreeField>(AddTreeFieldSetup);
        public ICommand RemoveCommand => _removeCommand ??= new DelegateCommand<TreeFieldSetupDefault>(RemoveTreeFieldSetup);
        public ICommand MoveUpCommand => _moveUpCommand ??= new DelegateCommand<TreeFieldSetupDefault>(MoveUp);
        public ICommand MoveDownCommand => _moveDownCommand ??= new DelegateCommand<TreeFieldSetupDefault>(MoveDown);

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            set
            {
                SetProperty(ref _treeFields, value);
                RaisePropertyChanged(nameof(AvalibleTreeFields));
            }
        }

        public IEnumerable<TreeField> AvalibleTreeFields
        {
            get
            {
                if(TreeFields == null) { return null; }
                if (TreeFieldSetups == null || TreeFieldSetups.Count == 0)
                { return TreeFields; }
                else
                {
                    var takenTreeFields = TreeFieldSetups.Select(x => x.Field).ToArray();
                    return TreeFields.Where(x => takenTreeFields.Contains(x.Field) == false)
                        .ToArray();
                }
            }
        }

        public ObservableCollection<TreeFieldSetupDefault> TreeFieldSetups
        {
            get => _fieldSetups;
            set => SetProperty(ref _fieldSetups, value);
        }

        public ITemplateDataservice TemplateDataservice { get; }

        public TreeFieldSetupDefault SelectedTreeFieldSetup
        {
            get => _selectedTreeFieldSetup;
            set
            {
                if (_selectedTreeFieldSetup != null)
                { _selectedTreeFieldSetup.PropertyChanged += SelectedTreeFieldSetup_PropertyChanged; }
                SetProperty(ref _selectedTreeFieldSetup, value);
                if (value != null)
                {
                    value.PropertyChanged -= SelectedTreeFieldSetup_PropertyChanged;

                    var dbType = TreeFields.Where(x => x.Field == value.Field).SingleOrDefault()?.DbType;
                    IsDefaultReal = string.Compare(dbType, "REAL", true) == 0;
                    IsDefaultInt = string.Compare(dbType, "INTEGER", true) == 0;
                    IsDefaultBoolean = string.Compare(dbType, "BOOLEAN", true) == 0;
                    IsDefaultText = string.Compare(dbType, "TEXT", true) == 0;
                }
                else
                {
                    IsDefaultReal = false;
                    IsDefaultInt = false;
                    IsDefaultBoolean = false;
                    IsDefaultText = false;
                }
                RaisePropertyChanged(nameof(IsDefaultBoolean));
                RaisePropertyChanged(nameof(IsDefaultInt));
                RaisePropertyChanged(nameof(IsDefaultReal));
                RaisePropertyChanged(nameof(IsDefaultText));

            }
        }

        public bool IsDefaultReal { get; set; }
        public bool IsDefaultInt { get; set; }
        public bool IsDefaultBoolean { get; set; }
        public bool IsDefaultText { get; set; }

        public StratumDefault StratumDefault
        {
            get => _stratumDefault;
            set
            {
                SetProperty(ref _stratumDefault, value);
                LoadFieldSetups();
            }
        }

        protected void LoadFieldSetups()
        {
            var stratumDefaultID = StratumDefault?.StratumDefaultID;
            if (stratumDefaultID != null)
            {
                var fieldSetups = TemplateDataservice.GetTreeFieldSetupDefaults(stratumDefaultID);
                TreeFieldSetups = new ObservableCollection<TreeFieldSetupDefault>(fieldSetups);
            }
            else
            {
                TreeFieldSetups = new ObservableCollection<TreeFieldSetupDefault>();
            }
            RaisePropertyChanged(nameof(AvalibleTreeFields));
        }

        private void SelectedTreeFieldSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tfsd = (TreeFieldSetupDefault)sender;
            TemplateDataservice.UpsertTreeFieldSetupDefault(tfsd);
        }

        public void AddTreeFieldSetup(TreeField treeField)
        {
            if (treeField == null) { return; }
            var stratumDefaultID = StratumDefault?.StratumDefaultID;
            var newtfsd = new TreeFieldSetupDefault()
            {
                Field = treeField.Field,
                FieldOrder = TreeFieldSetups.Count,
                StratumDefaultID = stratumDefaultID,
            };

            TemplateDataservice.AddTreeFieldSetupDefault(newtfsd);
            TreeFieldSetups.Add(newtfsd);
            RaisePropertyChanged(nameof(AvalibleTreeFields));
        }

        public void RemoveTreeFieldSetup(TreeFieldSetupDefault tfsd)
        {
            if (tfsd is null) { throw new ArgumentNullException(nameof(tfsd)); }

            TemplateDataservice.DeleteTreeFieldSetupDefault(tfsd);
            TreeFieldSetups.Remove(tfsd);
        }

        public void MoveUp(TreeFieldSetupDefault tfsd)
        {
            if (tfsd == null) { return; }
            var selectedIndex = TreeFieldSetups.IndexOf(tfsd);
            if (selectedIndex == TreeFieldSetups.Count - 1) { return; }
            var newIndex = selectedIndex + 1;
            var otherTfsd = TreeFieldSetups[newIndex];

            tfsd.FieldOrder = newIndex;
            otherTfsd.FieldOrder = selectedIndex;

            TemplateDataservice.UpsertTreeFieldSetupDefault(tfsd);
            TemplateDataservice.UpsertTreeFieldSetupDefault(otherTfsd);

            TreeFieldSetups.Move(selectedIndex, newIndex);
        }

        public void MoveDown(TreeFieldSetupDefault tfsd)
        {
            if (tfsd == null) { return; }
            var selectedIndex = TreeFieldSetups.IndexOf(tfsd);
            if (selectedIndex < 1) { return; }
            var newIndex = selectedIndex - 1;
            var otherTfsd = TreeFieldSetups[newIndex];

            tfsd.FieldOrder = newIndex;
            otherTfsd.FieldOrder = selectedIndex;

            TemplateDataservice.UpsertTreeFieldSetupDefault(tfsd);
            TemplateDataservice.UpsertTreeFieldSetupDefault(otherTfsd);

            TreeFieldSetups.Move(selectedIndex, newIndex);
        }
    }
}