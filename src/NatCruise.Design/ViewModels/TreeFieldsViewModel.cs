using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class TreeFieldsViewModel : ViewModelBase
    {
        private IEnumerable<TreeField> _treeFields;
        private TreeField _selectedTreeField;

        public TreeFieldsViewModel(ITreeFieldDataservice treeFieldDataservice)
        {
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
        }

        public ITreeFieldDataservice TreeFieldDataservice { get; }

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            set => SetProperty(ref _treeFields, value);
        }

        public TreeField SelectedTreeField
        {
            get => _selectedTreeField;
            set
            {
                if (_selectedTreeField != null) { _selectedTreeField.PropertyChanged -= SelectedTreeField_PropertyChanged; }
                SetProperty(ref _selectedTreeField, value);
                if(value != null) { value.PropertyChanged += SelectedTreeField_PropertyChanged; }
            }
        }
        private void SelectedTreeField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is TreeField tf && tf != null)
            {
                TreeFieldDataservice.UpdateTreeField(tf);
            }
        }

        public override void Load()
        {
            base.Load();
            TreeFields = TreeFieldDataservice.GetTreeFields();
        }
    }
}