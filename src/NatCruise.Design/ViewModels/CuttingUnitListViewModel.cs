using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
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

        public CuttingUnitListViewModel(IDataserviceProvider datastoreProvider)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            var unitDataservice = datastoreProvider.GetDataservice<ICuttingUnitDataservice>();
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
        }

        private ICuttingUnitDataservice UnitDataservice { get; }

        public ObservableCollection<CuttingUnit> CuttingUnits
        {
            get => _cuttingUnits;
            protected set => SetProperty(ref _cuttingUnits, value);
        }

        public CuttingUnit SelectedUnit
        {
            get => _selectedUnit;
            set => SetProperty(ref _selectedUnit, value);
        }

        public ICommand AddCuttingUnitCommand => _addCuttingUnitCommand ?? (_addCuttingUnitCommand = new DelegateCommand<string>(AddCuttingUnit));

        public ICommand RemoveCuttingUnitCommand => _removeCuttingUnitCommand ?? (_removeCuttingUnitCommand = new DelegateCommand<CuttingUnit>(RemoveCuttingUnit));

        public override void Load()
        {
            var units = UnitDataservice.GetCuttingUnits();

            CuttingUnits = new ObservableCollection<CuttingUnit>(units);
        }

        public void AddCuttingUnit(string unitCode)
        {
            unitCode = unitCode.Trim();
            if(Regex.IsMatch(unitCode, "^[a-zA-Z0-9]+$") is false) { return; }

            var newUnit = new CuttingUnit()
            {
                CuttingUnitCode = unitCode
            };

            UnitDataservice.AddCuttingUnit(newUnit);
            CuttingUnits.Add(newUnit);
            SelectedUnit = newUnit;
        }

        public void RemoveCuttingUnit(CuttingUnit unit)
        {
            if (unit == null) { return; }
            var cuttingUnits = CuttingUnits;

            UnitDataservice.DeleteCuttingUnit(unit);
            var index = cuttingUnits.IndexOf(unit);
            cuttingUnits.Remove(unit);

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
    }
}