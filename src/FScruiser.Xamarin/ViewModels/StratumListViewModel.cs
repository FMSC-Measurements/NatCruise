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
        private IEnumerable<Stratum> _strata;
        private ICommand _showFieldSetupCommand;
        private ICommand _showStratumDetailsCommand;
        private string _cuttingUnitFilter;

        public StratumListViewModel(IStratumDataservice stratumDataservice, ICuttingUnitDataservice cuttingUnitDataservice, INatCruiseNavigationService natCruiseNavigationService)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            NavigationService = natCruiseNavigationService ?? throw new ArgumentNullException(nameof(natCruiseNavigationService));

            CuttingUnitCodes = cuttingUnitDataservice.GetCuttingUnitCodes();
        }

        public ICommand ShowFieldSetupCommand => _showFieldSetupCommand ??= new DelegateCommand<Stratum>(st => NavigationService.ShowFieldSetup(st.StratumCode));

        public ICommand ShowStratumDetailsCommand => _showStratumDetailsCommand ??= new DelegateCommand<Stratum>(st => NavigationService.ShowStratumDetail(st.StratumCode));

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
            set => SetProperty(ref _cuttingUnitFilter, value);
        }

        public override void Load()
        {
            base.Load();

            Strata = StratumDataservice.GetStrata(CuttingUnitFilter);
        }
    }
}
