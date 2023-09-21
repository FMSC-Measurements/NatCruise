﻿using NatCruise.Data;
using NatCruise.Design.Util;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Util;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitStrataViewModel : ViewModelBase
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
                RefreshSelectedUnits();
            }
        }

        public CuttingUnitStrataViewModel(ICuttingUnitDataservice cuttingUnitDataservice, IStratumDataservice stratumDataservice, INatCruiseDialogService dialogService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public IEnumerable<CuttingUnitItem> AllUnits
        {
            get;
            protected set;
        }

        public IStratumDataservice StratumDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

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

        private void RefreshSelectedUnits()
        {
            var stratum = Stratum;

            if (stratum != null)
            {
                SelectedUnitCodes = CuttingUnitDataservice.GetCuttingUnitCodesByStratum(stratum.StratumCode)
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

            OnPropertyChanged(nameof(AllUnits));
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

        public class CuttingUnitItem : BindableBase
        {
            public CuttingUnitItem(CuttingUnit unit, CuttingUnitStrataViewModel viewModel)
            {
                Unit = unit ?? throw new ArgumentNullException(nameof(unit));
                ParentViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }

            public CuttingUnitStrataViewModel ParentViewModel { get; }

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