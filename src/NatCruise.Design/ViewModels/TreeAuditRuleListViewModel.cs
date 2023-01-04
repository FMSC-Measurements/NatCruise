using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using NatCruise.MVVM;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class TreeAuditRuleListViewModel : ViewModelBase
    {
        private ObservableCollection<TreeAuditRule> _treeAuditRules;
        private TreeAuditRule _newTreeAuditRule = new TreeAuditRule();
        private IEnumerable<TreeField> _treeFieldOptions;
        private DelegateCommand _addNewTreeAuditRuleCommand;
        private DelegateCommand<TreeAuditRule> _deleteTreeAuditRuleCommand;

        public TreeAuditRuleListViewModel(ITemplateDataservice templateDataservice)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
        }

        ITemplateDataservice TemplateDataservice { get; }

        public IEnumerable<TreeField> TreeFieldOptions
        {
            get => _treeFieldOptions;
            set => SetProperty(ref _treeFieldOptions, value);
        }

        public ObservableCollection<TreeAuditRule> TreeAuditRules
        {
            get => _treeAuditRules;
            set
            {
                if(_treeAuditRules != null)
                {
                    foreach (var i in _treeAuditRules)
                    {
                        i.PropertyChanged -= TreeAuditRule_PropertyChanged;
                    }
                }
                SetProperty(ref _treeAuditRules, value);
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        i.PropertyChanged += TreeAuditRule_PropertyChanged;
                    }
                }
            }
        }

        private void TreeAuditRule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tar = (TreeAuditRule)sender;
            TemplateDataservice.UpsertTreeAuditRule(tar);
        }

        public TreeAuditRule NewTreeAuditRule
        {
            get => _newTreeAuditRule;
            set => SetProperty(ref _newTreeAuditRule, value);
        }

        public ICommand AddNewTreeAuditRuleCommand => _addNewTreeAuditRuleCommand ??= new DelegateCommand(AddNewTreeAuditRule);
        public ICommand DeleteTreeAuditRuleCommand => _deleteTreeAuditRuleCommand ??= new DelegateCommand<TreeAuditRule>(DeleteTreeAuditRule);

        public void AddNewTreeAuditRule()
        {
            var newTreeAuditRule = NewTreeAuditRule;
            NewTreeAuditRule = new TreeAuditRule();
            if (newTreeAuditRule == null) { return; }

            newTreeAuditRule.TreeAuditRuleID = Guid.NewGuid().ToString();
            TemplateDataservice.AddTreeAuditRule(newTreeAuditRule);
            TreeAuditRules.Add(newTreeAuditRule);
            newTreeAuditRule.PropertyChanged += TreeAuditRule_PropertyChanged;
        }

        public void DeleteTreeAuditRule(TreeAuditRule tar)
        {
            TemplateDataservice.DeleteTreeAuditRule(tar);
            TreeAuditRules.Remove(tar);
            tar.PropertyChanged -= TreeAuditRule_PropertyChanged;
        }

        public override void Load()
        {
            base.Load();

            var treeAuditRules = TemplateDataservice.GetTreeAuditRules();
            TreeAuditRules = new ObservableCollection<TreeAuditRule>(treeAuditRules);

            TreeFieldOptions = TemplateDataservice.GetTreeFields();
        }
    }
}
