using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Design.Validation;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private DelegateCommand<CuttingUnit> _removeCuttingUnitCommand;
        private DelegateCommand<string> _addCuttingUnitCommand;
        private ObservableCollection<CuttingUnit> _cuttingUnits;
        private CuttingUnit _selectedUnit;
        private Dictionary<CuttingUnit, IEnumerable<string>> _unitErrorLookup;

        public CuttingUnitListViewModel(IDataserviceProvider datastoreProvider, IDialogService dialogService)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            var unitDataservice = datastoreProvider.GetDataservice<ICuttingUnitDataservice>();
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            CuttingUnitValidator = new CuttingUnitValidator();
        }

        private ICuttingUnitDataservice UnitDataservice { get; }
        public IDialogService DialogService { get; }
        public CuttingUnitValidator CuttingUnitValidator { get; }

        public ObservableCollection<CuttingUnit> CuttingUnits
        {
            get => _cuttingUnits;
            protected set
            {
                if (_cuttingUnits != null)
                {
                    foreach (var unit in _cuttingUnits)
                    {
                        unit.PropertyChanged -= unit_PropertyChanged;
                    }
                }
                SetProperty(ref _cuttingUnits, value);
                if (value != null)
                {
                    foreach (var unit in _cuttingUnits)
                    {
                        unit.PropertyChanged += unit_PropertyChanged;
                    }
                }
            }
        }

        private void unit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(CuttingUnit.Errors)) { return; }
            var unit = sender as CuttingUnit;
            if (unit != null)
            {
                ValidateUnit(unit);
            }
        }

        public CuttingUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                SetProperty(ref _selectedUnit, value);
                if(value != null)
                {
                    ValidateUnit(value);
                }
            }
        }

        public ICommand AddCuttingUnitCommand => _addCuttingUnitCommand ?? (_addCuttingUnitCommand = new DelegateCommand<string>(AddCuttingUnit));

        public ICommand RemoveCuttingUnitCommand => _removeCuttingUnitCommand ?? (_removeCuttingUnitCommand = new DelegateCommand<CuttingUnit>(RemoveCuttingUnit));

        public override void Load()
        {
            var units = UnitDataservice.GetCuttingUnits();

            CuttingUnits = new ObservableCollection<CuttingUnit>(units);

            foreach (var unit in units)
            {
                ValidateUnit(unit);
            }
        }

        public void AddCuttingUnit(string unitCode)
        {
            unitCode = unitCode.Trim();
            if (Regex.IsMatch(unitCode, "^[a-zA-Z0-9]+$") is false) { return; }

            var newUnit = new CuttingUnit()
            {
                CuttingUnitCode = unitCode
            };

            try
            {
                UnitDataservice.AddCuttingUnit(newUnit);
                newUnit.PropertyChanged += unit_PropertyChanged;
                CuttingUnits.Add(newUnit);
                SelectedUnit = newUnit;
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Unit Code Already Exists");
            }
        }

        public void RemoveCuttingUnit(CuttingUnit unit)
        {
            if (unit == null) { return; }
            var cuttingUnits = CuttingUnits;

            UnitDataservice.DeleteCuttingUnit(unit);
            var index = cuttingUnits.IndexOf(unit);
            cuttingUnits.Remove(unit);
            unit.PropertyChanged -= unit_PropertyChanged;

            if (index < 0) { return; }
            if (index <= cuttingUnits.Count - 1)
            {
                var newSelectedUnit = cuttingUnits[index];
                SelectedUnit = newSelectedUnit;
            }
            else
            {
                SelectedUnit = cuttingUnits.LastOrDefault();
            }
        }

        public void ValidateUnit(CuttingUnit unit)
        {
            var errors = CuttingUnitValidator.Validate(unit).Errors.Select(x => x.ErrorMessage).ToArray();
            if (errors.Length > 0)
            {
                unit.Errors = errors;
            }
            else
            {
                unit.Errors = Enumerable.Empty<string>();
            }
        }
    }
}