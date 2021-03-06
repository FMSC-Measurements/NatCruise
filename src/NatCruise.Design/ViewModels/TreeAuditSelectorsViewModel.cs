﻿using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class TreeAuditSelectorsViewModel : ViewModelBase
    {
        private TreeAuditRule _treeAuditRule;
        private ObservableCollection<TreeAuditRuleSelector> _selectors;
        private TreeAuditRuleSelector _newRuleSelector = new TreeAuditRuleSelector();
        private IEnumerable<string> _speciesOptions;
        private IEnumerable<Product> _productOptions;
        private DelegateCommand _addNewRuleSelectorCommand;
        private DelegateCommand<TreeAuditRuleSelector> _deleteRuleSelectorCommand;

        public TreeAuditSelectorsViewModel(ITemplateDataservice templateDataservice, ISetupInfoDataservice setupDataservice)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
        }

        public ICommand AddNewRuleSelectorCommand => _addNewRuleSelectorCommand ??= new DelegateCommand(AddNewRuleSelector);
        public ICommand DeleteRuleSelectorCommand => _deleteRuleSelectorCommand ??= new DelegateCommand<TreeAuditRuleSelector>(DeleteRuleSelector);

        public ITemplateDataservice TemplateDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public TreeAuditRule TreeAuditRule
        {
            get => _treeAuditRule;
            set
            {
                SetProperty(ref _treeAuditRule, value);
                if (value != null)
                {
                    var selectors = TemplateDataservice.GetRuleSelectors(value.TreeAuditRuleID);
                    Selectors = new ObservableCollection<TreeAuditRuleSelector>(selectors);
                }
                else
                { Selectors = null; }
            }
        }

        public ObservableCollection<TreeAuditRuleSelector> Selectors
        {
            get => _selectors;
            set
            {
                SetProperty(ref _selectors, value);
            }
        }

        public TreeAuditRuleSelector NewRuleSelector
        {
            get => _newRuleSelector;
            set => SetProperty(ref _newRuleSelector, value);
        }

        public IEnumerable<string> SpeciesOptions
        {
            get => _speciesOptions;
            set => SetProperty(ref _speciesOptions, value);
        }

        public IEnumerable<string> LiveDeadOptions { get; } = new[] { "L", "D" };

        public IEnumerable<Product> ProductOptions
        {
            get => _productOptions;
            set => SetProperty(ref _productOptions, value);
        }

        public override void Load()
        {
            base.Load();

            SpeciesOptions = TemplateDataservice.GetSpeciesCodes();
            ProductOptions = SetupDataservice.GetProducts();
        }

        public void AddNewRuleSelector()
        {
            var newRuleSelector = NewRuleSelector;
            if (newRuleSelector == null) { return; }
            var auditRule = TreeAuditRule;
            if (auditRule == null) { return; }
            newRuleSelector.TreeAuditRuleID = auditRule.TreeAuditRuleID;

            TemplateDataservice.AddRuleSelector(newRuleSelector);
            Selectors.Add(newRuleSelector);

            NewRuleSelector = new TreeAuditRuleSelector();
        }

        public void DeleteRuleSelector(TreeAuditRuleSelector tars)
        {
            if (tars == null) { return; }
            TemplateDataservice.DeleteRuleSelector(tars);
            Selectors.Remove(tars);
        }
    }
}