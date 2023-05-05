using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class TreeAuditRuleListViewModel : ViewModelBase
    {
        private ObservableCollection<TreeAuditRule> _treeAuditRules;
        private TreeAuditRule _newTreeAuditRule = new TreeAuditRule();
        private DelegateCommand _addNewTreeAuditRuleCommand;
        private DelegateCommand<TreeAuditRule> _deleteTreeAuditRuleCommand;

        public TreeAuditRuleListViewModel(ITreeAuditRuleDataservice treeAuditRuleDataservice, ITreeFieldDataservice treeFieldDataservice)
        {
            TreeAuditRuleDataservice = treeAuditRuleDataservice ?? throw new ArgumentNullException(nameof(treeAuditRuleDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));

            TreeFieldOptions = TreeFieldDataservice.GetTreeFields();
        }

        public ITreeAuditRuleDataservice TreeAuditRuleDataservice { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }

        public IEnumerable<TreeField> TreeFieldOptions { get; }

        public ObservableCollection<TreeAuditRule> TreeAuditRules
        {
            get => _treeAuditRules;
            set => SetProperty(ref _treeAuditRules, value);
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
            TreeAuditRuleDataservice.AddTreeAuditRule(newTreeAuditRule);
            TreeAuditRules.Add(newTreeAuditRule);
        }

        public void DeleteTreeAuditRule(TreeAuditRule tar)
        {
            TreeAuditRuleDataservice.DeleteTreeAuditRule(tar);
            TreeAuditRules.Remove(tar);
        }

        public override void Load()
        {
            base.Load();

            var treeAuditRules = TreeAuditRuleDataservice.GetTreeAuditRules();
            TreeAuditRules = new ObservableCollection<TreeAuditRule>(treeAuditRules);

            
        }
    }
}
