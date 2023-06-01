using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
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
        private TreeAuditRule _newTreeAuditRule;
        private DelegateCommand _addNewTreeAuditRuleCommand;
        private DelegateCommand<TreeAuditRule> _deleteTreeAuditRuleCommand;
        private ICommand _showEditTreeAuditRule;

        public TreeAuditRuleListViewModel(ITreeAuditRuleDataservice treeAuditRuleDataservice, ITreeFieldDataservice treeFieldDataservice, INatCruiseNavigationService navigationService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            TreeAuditRuleDataservice = treeAuditRuleDataservice ?? throw new ArgumentNullException(nameof(treeAuditRuleDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));

            NewTreeAuditRule = new TreeAuditRule();
            TreeFieldOptions = TreeFieldDataservice.GetTreeFields();
        }

        public INatCruiseNavigationService NavigationService { get; }
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

        public ICommand ShowEditTreeAuditRule => _showEditTreeAuditRule ??= new DelegateCommand<TreeAuditRule>(x => NavigationService.ShowTreeAuditRuleEdit(x.TreeAuditRuleID));

        public void AddNewTreeAuditRule()
        {
            var newTreeAuditRule = NewTreeAuditRule;
            if (newTreeAuditRule == null) {
                NewTreeAuditRule = new TreeAuditRule();
                return;
            }

            if (newTreeAuditRule.Min == null || newTreeAuditRule.Max == null)
            { return; }
            if(newTreeAuditRule.Min.HasValue && newTreeAuditRule.Max.HasValue
                && Math.Round(newTreeAuditRule.Min.Value, 2) >= Math.Round(newTreeAuditRule.Max.Value, 2))
            { return; }
                

            newTreeAuditRule.TreeAuditRuleID = Guid.NewGuid().ToString();
            TreeAuditRuleDataservice.AddTreeAuditRule(newTreeAuditRule);
            NewTreeAuditRule = new TreeAuditRule();
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
