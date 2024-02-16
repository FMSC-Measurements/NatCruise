using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NatCruise.Util;

namespace NatCruise.MVVM.ViewModels
{
    public class TreeAuditRuleEditViewModel : ViewModelBase
    {
        private TreeAuditRule _treeAuditRule;
        private ObservableCollection<TreeAuditRuleSelector> _selectors;
        private TreeAuditRuleSelector _newRuleSelector = new TreeAuditRuleSelector();
        private DelegateCommand _addNewRuleSelectorCommand;
        private DelegateCommand<TreeAuditRuleSelector> _deleteRuleSelectorCommand;

        public TreeAuditRuleEditViewModel(
            ITreeAuditRuleDataservice treeAuditRuleDataservice,
                                          ITreeFieldDataservice treeFieldDataservice,
                                          ISpeciesDataservice speciesDataservice,
                                          ISetupInfoDataservice setupDataservice,
                                          INatCruiseDialogService dialogService
            )
        {
            TreeAuditRuleDataservice = treeAuditRuleDataservice ?? throw new ArgumentNullException(nameof(treeAuditRuleDataservice));
            SpeciesDataservice = speciesDataservice ?? throw new ArgumentNullException(nameof(speciesDataservice));
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SpeciesOptions = SpeciesDataservice.GetSpeciesCodes();
            var prodOptions = ProductOptions = setupDataservice.GetProducts();
            ProductCodeOptions = prodOptions.Select(x => x.ProductCode).ToArray();
            TreeFieldOptions = treeFieldDataservice.GetTreeFields();
        }

        public ITreeAuditRuleDataservice TreeAuditRuleDataservice { get; }

        public ICommand AddNewRuleSelectorCommand => _addNewRuleSelectorCommand ??= new DelegateCommand(AddNewRuleSelector);
        public ICommand DeleteRuleSelectorCommand => _deleteRuleSelectorCommand ??= new DelegateCommand<TreeAuditRuleSelector>(DeleteRuleSelector);

        public ISpeciesDataservice SpeciesDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

        public TreeAuditRule TreeAuditRule
        {
            get => _treeAuditRule;
            set
            {
                if (object.ReferenceEquals(_treeAuditRule, value)) { return; }
                if (_treeAuditRule != null)
                {
                    _treeAuditRule.PropertyChanged -= OnTreeAuditRule_PropertyChanged;
                }

                SetProperty(ref _treeAuditRule, value);
                if (value != null)
                {
                    value.PropertyChanged += OnTreeAuditRule_PropertyChanged;


                    var selectors = TreeAuditRuleDataservice.GetRuleSelectors(value.TreeAuditRuleID);
                    Selectors = new ObservableCollection<TreeAuditRuleSelector>(selectors);
                }
                else
                { Selectors = null; }
            }
        }

        private void OnTreeAuditRule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tar = (TreeAuditRule)sender;

            if (tar.Min == null || tar.Max == null)
            { return; }
            if (tar.Min.HasValue && tar.Max.HasValue
                && Math.Round(tar.Min.Value, 2) >= Math.Round(tar.Max.Value, 2))
            { return; }

            TreeAuditRuleDataservice.UpsertTreeAuditRule(tar);
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

        public IEnumerable<string> SpeciesOptions { get; }
        public IEnumerable<string> LiveDeadOptions { get; } = new[] { "L", "D" };
        public IEnumerable<Product> ProductOptions { get; }
        public IEnumerable<string> ProductCodeOptions { get; }
        public IEnumerable<TreeField> TreeFieldOptions { get; }

        public override void Load()
        {
            base.Load();

            // on WPF parameters are not set
            var parameters = Parameters;
            if (parameters != null)
            {
                var tarID = Parameters.GetValueOrDefault<string>(NavParams.TreeAuditRuleID);
                Load(tarID);
            }
        }

        public void Load(string treeAuditRuleID)
        {
            var tar = TreeAuditRuleDataservice.GetTreeAuditRule(treeAuditRuleID);
            TreeAuditRule = tar;
        }

        public void AddNewRuleSelector()
        {
            var newRuleSelector = NewRuleSelector ?? throw new InvalidOperationException(nameof(NewRuleSelector) + " Is Null");
            var auditRule = TreeAuditRule ?? throw new InvalidOperationException(nameof(TreeAuditRule) + " Is Null");
            newRuleSelector.TreeAuditRuleID = auditRule.TreeAuditRuleID;

            try
            {
                TreeAuditRuleDataservice.AddRuleSelector(newRuleSelector);
                Selectors.Add(newRuleSelector);
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                //DialogService.ShowNotification("Audit Rule Selector Already Exists");
            }

            NewRuleSelector = new TreeAuditRuleSelector();
        }

        public void DeleteRuleSelector(TreeAuditRuleSelector tars)
        {
            if (tars == null) { return; }
            TreeAuditRuleDataservice.DeleteRuleSelector(tars);
            Selectors.Remove(tars);
        }
    }
}