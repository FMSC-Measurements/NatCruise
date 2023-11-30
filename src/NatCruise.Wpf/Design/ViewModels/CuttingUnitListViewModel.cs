using CommunityToolkit.Mvvm.Input;
using NatCruise.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using NatCruise.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace NatCruise.Design.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private IRelayCommand<CuttingUnit> _removeCuttingUnitCommand;
        private IRelayCommand<string> _addCuttingUnitCommand;
        private ObservableCollection<CuttingUnit> _cuttingUnits;
        private CuttingUnit _selectedUnit;
        private IApplicationSettingService _appSettings;
        private bool _isSuperuserModeEnabled;

        public CuttingUnitListViewModel(ICuttingUnitDataservice unitDataservice,
            INatCruiseDialogService dialogService,
            IApplicationSettingService applicationSettingService)
        {
            UnitDataservice = unitDataservice ?? throw new ArgumentNullException(nameof(unitDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            AppSettings = applicationSettingService ?? throw new ArgumentNullException(nameof(applicationSettingService));

            CuttingUnitValidator = new CuttingUnitValidator();
        }

        public event EventHandler CuttingUnitAdded;

        private ICuttingUnitDataservice UnitDataservice { get; }
        public INatCruiseDialogService DialogService { get; }

        public IApplicationSettingService AppSettings
        {
            get => _appSettings;
            private set
            {
                if (_appSettings != null) { _appSettings.PropertyChanged -= AppSettings_PropertyChanged; }
                _appSettings = value;
                if (value != null)
                {
                    IsSuperuserModeEnabled = value.IsSuperuserMode;
                    _appSettings.PropertyChanged += AppSettings_PropertyChanged;
                }
                OnPropertyChanged(nameof(AppSettings));
            }
        }

        private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IWpfApplicationSettingService.IsSuperuserMode))
            {
                var appSettings = (IWpfApplicationSettingService)sender;
                IsSuperuserModeEnabled = appSettings.IsSuperuserMode;
            }
        }

        public bool IsSuperuserModeEnabled
        {
            get => _isSuperuserModeEnabled;
            set
            {
                SetProperty(ref _isSuperuserModeEnabled, value);
                RemoveCuttingUnitCommand.NotifyCanExecuteChanged();
            }
        }

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
            if (e.PropertyName == nameof(CuttingUnit.Errors)) { return; }
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
                RemoveCuttingUnitCommand.NotifyCanExecuteChanged();
                if (value != null)
                {
                    ValidateUnit(value);
                }
            }
        }

        public IRelayCommand AddCuttingUnitCommand => _addCuttingUnitCommand ?? (_addCuttingUnitCommand = new RelayCommand<string>(AddCuttingUnit));

        public IRelayCommand RemoveCuttingUnitCommand => _removeCuttingUnitCommand ?? (_removeCuttingUnitCommand = new RelayCommand<CuttingUnit>(RemoveCuttingUnit, CanRemoveCuttingUnit));

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
            if (Regex.IsMatch(unitCode, "^[a-zA-Z0-9]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)) is false) { return; }

            var newUnit = new CuttingUnit()
            {
                CuttingUnitCode = unitCode
            };

            try
            {
                UnitDataservice.AddCuttingUnit(newUnit);
                newUnit.PropertyChanged += unit_PropertyChanged;
                CuttingUnits.Add(newUnit);
                CuttingUnitAdded?.Invoke(this, EventArgs.Empty);
                SelectedUnit = newUnit;
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Unit Code Already Exists");
            }
        }

        public bool CanRemoveCuttingUnit(CuttingUnit unit)
        {
            var result = unit != null
                && (!unit.HasTrees || IsSuperuserModeEnabled);
            Debug.WriteLine("CanRemoveCuttingunit: " + result.ToString());
            return result;
        }

        public void RemoveCuttingUnit(CuttingUnit unit)
        {
            if (unit == null) { return; }
            var cuttingUnits = CuttingUnits;

            if (unit.HasTrees && !IsSuperuserModeEnabled) { return; }

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
            var errors = CuttingUnitValidator.Validate(unit).Errors
                .Where(x => x.Severity == FluentValidation.Severity.Error)
                .Select(x => x.ErrorMessage).ToArray();
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