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
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Data;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateFieldsViewModel : ViewModelBase
    {
        private StratumTemplate _stratumTemplate;
        private ObservableCollection<StratumTemplateTreeFieldSetup> _fieldSetups;
        private IEnumerable<TreeField> _treeFields;
        private StratumTemplateTreeFieldSetup _selectedTreeFieldSetup;
        private ICommand _moveUpCommand;
        private ICommand _moveDownCommand;
        private ICommand _addCommand;
        private ICommand _removeCommand;

        public StratumTemplateFieldsViewModel(ITemplateDataservice templateDataservice, ITreeFieldDataservice treeFieldDataservice)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));

            TreeFields = treeFieldDataservice.GetTreeFields();
        }

        public ICommand AddCommand => _addCommand ??= new DelegateCommand<TreeField>(AddTreeFieldSetup);
        public ICommand RemoveCommand => _removeCommand ??= new DelegateCommand<StratumTemplateTreeFieldSetup>(RemoveTreeFieldSetup);
        public ICommand MoveUpCommand => _moveUpCommand ??= new DelegateCommand<StratumTemplateTreeFieldSetup>(MoveUp);
        public ICommand MoveDownCommand => _moveDownCommand ??= new DelegateCommand<StratumTemplateTreeFieldSetup>(MoveDown);

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

        public ObservableCollection<StratumTemplateTreeFieldSetup> TreeFieldSetups
        {
            get => _fieldSetups;
            set => SetProperty(ref _fieldSetups, value);
        }

        public ITemplateDataservice TemplateDataservice { get; }

        public StratumTemplateTreeFieldSetup SelectedTreeFieldSetup
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

        public StratumTemplate StratumTemplate
        {
            get => _stratumTemplate;
            set
            {
                SetProperty(ref _stratumTemplate, value);
                LoadFieldSetups();
            }
        }

        protected void LoadFieldSetups()
        {
            var stratumTemplateName = StratumTemplate?.StratumTemplateName;
            if (stratumTemplateName != null)
            {
                var fieldSetups = TemplateDataservice.GetStratumTemplateTreeFieldSetups(stratumTemplateName);
                TreeFieldSetups = new ObservableCollection<StratumTemplateTreeFieldSetup>(fieldSetups);
            }
            else
            {
                TreeFieldSetups = new ObservableCollection<StratumTemplateTreeFieldSetup>();
            }
            RaisePropertyChanged(nameof(AvalibleTreeFields));
        }

        private void SelectedTreeFieldSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var sttfs = (StratumTemplateTreeFieldSetup)sender;
            if(sttfs is null) { return; }
            TemplateDataservice.UpsertStratumTemplateTreeFieldSetup(sttfs);
        }

        public void AddTreeFieldSetup(TreeField treeField)
        {
            if (treeField == null) { return; }
            var stratumTemplateName = StratumTemplate?.StratumTemplateName;
            var newtfsd = new StratumTemplateTreeFieldSetup()
            {
                Field = treeField.Field,
                FieldOrder = TreeFieldSetups.Count + 1,
                StratumTemplateName = stratumTemplateName,
            };

            TemplateDataservice.UpsertStratumTemplateTreeFieldSetup(newtfsd);
            TreeFieldSetups.Add(newtfsd);
            RaisePropertyChanged(nameof(AvalibleTreeFields));
        }

        public void RemoveTreeFieldSetup(StratumTemplateTreeFieldSetup tfsd)
        {
            if (tfsd is null) { throw new ArgumentNullException(nameof(tfsd)); }

            TemplateDataservice.DeleteStratumTemplateTreeFieldSetup(tfsd);
            TreeFieldSetups.Remove(tfsd);
        }

        public void MoveUp(StratumTemplateTreeFieldSetup tfsd)
        {
            if (tfsd == null) { return; }
            var selectedIndex = TreeFieldSetups.IndexOf(tfsd);
            if (selectedIndex == TreeFieldSetups.Count - 1) { return; }
            var newIndex = selectedIndex + 1;
            TreeFieldSetups.Move(selectedIndex, newIndex);
            SaveFieldOrder();
        }

        public void MoveDown(StratumTemplateTreeFieldSetup tfsd)
        {
            if (tfsd == null) { return; }
            var selectedIndex = TreeFieldSetups.IndexOf(tfsd);
            if (selectedIndex < 1) { return; }
            var newIndex = selectedIndex - 1;
            TreeFieldSetups.Move(selectedIndex, newIndex);
            SaveFieldOrder();
        }

        protected void SaveFieldOrder()
        {
            var fieldSetups = TreeFieldSetups;
            foreach (var (field, i) in fieldSetups.Select((x, i) => (x, i + 1)))
            {
                field.FieldOrder = i;
                TemplateDataservice.UpsertStratumTemplateTreeFieldSetup(field);
            }
        }
    }
}