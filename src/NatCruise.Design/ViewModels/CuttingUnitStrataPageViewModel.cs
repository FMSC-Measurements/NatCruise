using NatCruise.Data;
using NatCruise.Services;
using NatCruise.Util;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitStrataPageViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private IEnumerable<string> _selectedUnitCodes;
        private ICommand _selectAllCommand;
        private DelegateCommand _clearAllcommand;

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                SetProperty(ref _stratum, value);
                OnStratumChanged(value);
            }
        }

        public CuttingUnitStrataPageViewModel(IDataserviceProvider dataserviceProvider, IDialogService dialogService)
        {
            StratumDataservice = dataserviceProvider.GetDataservice<IStratumDataservice>();
            CuttingUnitDataservice = dataserviceProvider.GetDataservice<ICuttingUnitDataservice>();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public IEnumerable<CuttingUnitItem> AllUnits
        {
            get;
            protected set;
        }

        public IStratumDataservice StratumDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public IDialogService DialogService { get; }

        public IEnumerable<string> SelectedUnitCodes
        {
            get => _selectedUnitCodes;
            protected set => SetProperty(ref _selectedUnitCodes, value);
        }

        public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new DelegateCommand(SelectAllUnits));

        public ICommand ClearAllCommand => _clearAllcommand ?? (_clearAllcommand = new DelegateCommand(ClearAllUnits));

        private void ClearAllUnits()
        {
            var stratumCode = Stratum.StratumCode;
            foreach (var unit in AllUnits)
            {
                var unitCode = unit.Unit.CuttingUnitCode;

                var hasTrees = StratumDataservice.HasTreeCounts(unitCode, stratumCode);
                if (hasTrees) { continue; }

                StratumDataservice.RemoveStratumFromCuttingUnit(unitCode, stratumCode);
            }

            RefreshSelectedUnits();
        }

        private void SelectAllUnits()
        {
            var stratumCode = Stratum.StratumCode;
            foreach (var unit in AllUnits)
            {
                var unitCode = unit.Unit.CuttingUnitCode;
                StratumDataservice.AddStratumToCuttingUnit(unitCode, stratumCode);
            }

            RefreshSelectedUnits();
        }

        private void OnStratumChanged(Stratum newStratum)
        {
            RefreshSelectedUnits();
        }

        private void RefreshSelectedUnits()
        {
            var stratum = Stratum;

            if (stratum != null)
            {
                SelectedUnitCodes = StratumDataservice.GetCuttingUnitCodesByStratum(stratum.StratumCode)
                    .ToHashSet();
            }
            else
            {
                SelectedUnitCodes = new HashSet<string>();
            }

            AllUnits = CuttingUnitDataservice
                .GetCuttingUnits()
                .Select(x => new CuttingUnitItem(x, this))
                .ToObservableCollection();

            RaisePropertyChanged(nameof(AllUnits));
        }

        public void AddUnit(string unitCode)
        {
            StratumDataservice.AddStratumToCuttingUnit(unitCode, Stratum.StratumCode);

            RefreshSelectedUnits();
        }

        public void RemoveUnit(string unitCode)
        {
            bool force = false;
            var stratumCode = Stratum.StratumCode;
            var hasTrees = StratumDataservice.HasTreeCounts(unitCode, stratumCode);

            if (hasTrees)
            {
                DialogService.ShowNotification("Unit Has Trees");
            }

            if (force || !hasTrees)
            {
                StratumDataservice.RemoveStratumFromCuttingUnit(unitCode, stratumCode);
            }

            RefreshSelectedUnits();
        }

        protected override void Load()
        {
        }

        public class CuttingUnitItem : BindableBase
        {
            public CuttingUnitItem(CuttingUnit unit, CuttingUnitStrataPageViewModel viewModel)
            {
                Unit = unit ?? throw new ArgumentNullException(nameof(unit));
                ParentViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }

            public CuttingUnitStrataPageViewModel ParentViewModel { get; }

            public CuttingUnit Unit { get; }

            public bool IsSelected
            {
                get => ParentViewModel.SelectedUnitCodes?.Contains(Unit.CuttingUnitCode) ?? false;
                set
                {
                    var unitCode = Unit.CuttingUnitCode;
                    if (value)
                    {
                        ParentViewModel.AddUnit(unitCode);
                    }
                    else
                    {
                        ParentViewModel.RemoveUnit(unitCode);
                    }
                }
            }
        }
    }
}