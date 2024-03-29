﻿using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.XF.ViewModels
{
    public class StratumListViewModel : ViewModelBase
    {
        public const string ALL_UNITS_OPTION = "All Units";
        private IEnumerable<Stratum> _strata;
        private ICommand _showFieldSetupCommand;
        private ICommand _showStratumDetailsCommand;
        private string _cuttingUnitFilter;
        private ICommand _showSampleGroupsCommand;

        public StratumListViewModel(IStratumDataservice stratumDataservice, ICuttingUnitDataservice cuttingUnitDataservice, INatCruiseNavigationService natCruiseNavigationService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            NavigationService = natCruiseNavigationService ?? throw new ArgumentNullException(nameof(natCruiseNavigationService));

            CuttingUnitFilter = ALL_UNITS_OPTION;
            CuttingUnitCodes = cuttingUnitDataservice.GetCuttingUnitCodes().Prepend(ALL_UNITS_OPTION).ToArray();
        }

        public ICommand ShowFieldSetupCommand => _showFieldSetupCommand ??= new DelegateCommand<Stratum>(st => NavigationService.ShowFieldSetup(st.StratumCode).FireAndForget());
        public ICommand ShowStratumInfoCommand => _showStratumDetailsCommand ??= new DelegateCommand<Stratum>(st => NavigationService.ShowStratumInfo(st.StratumCode).FireAndForget());
        public ICommand ShowSampleGroupsCommand => _showSampleGroupsCommand ??= new DelegateCommand<Stratum>(st => NavigationService.ShowSampleGroups(st.StratumCode).FireAndForget());

        public IStratumDataservice StratumDataservice { get; }
        public INatCruiseNavigationService NavigationService { get; }
        public IEnumerable<Stratum> Strata
        {
            get => _strata;
            private set => SetProperty(ref _strata, value);
        }

        public IEnumerable<string> CuttingUnitCodes { get; set; }

        public string CuttingUnitFilter
        {
            get => _cuttingUnitFilter;
            set
            {
                SetProperty(ref _cuttingUnitFilter, value);
                Load();
            }
        }

        public override void Load()
        {
            base.Load();

            var unitFilter = CuttingUnitFilter;
            if (unitFilter == ALL_UNITS_OPTION) unitFilter = null;
            Strata = StratumDataservice.GetStrata(unitFilter);
        }
    }
}
