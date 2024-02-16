using FScruiser.XF.Services;
using Microsoft.Extensions.DependencyInjection;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public class NavOption
        {
            public string Heading { get; set; }
            public ICommand Command { get; set; }
        }

        private IEnumerable<CuttingUnit> _cuttingUnits;
        private CuttingUnit _selectedCuttingUnit;
        private CuttingUnitStrataSummary _selectedUnitStratumSummary;
        private IEnumerable<NavOption> _navOptions;
        private Command _showAuditRulesCommand;
        private Command _showUtilitiesCommand;
        private Command _showCruisersCommand;
        private Command _showSampleStateManagmentCommand;
        private Command _showFeedbackCommand;
        private List<NavOption> _moreNavOptions;
        private ICommand _showAbout;

        public ICommand ShowSelectSale => new Command(() => NavigationService.ShowSaleSelect().FireAndForget());

        public ICommand ShowSaleCommand => new Command(() => NavigationService.ShowSale(DataContext?.CruiseID).FireAndForget());

        //public ICommand ShowUnitsCommand => new Command(() => NavigationService.ShowCuttingUnitList().FireAndForget());

        public ICommand ShowStrataCommand => new Command(() => NavigationService.ShowStrata().FireAndForget());

        public ICommand ShowCuttingUnitCommand => new Command(() => NavigationService.ShowCuttingUnitInfo(SelectedCuttingUnit?.CuttingUnitCode).FireAndForget());

        public ICommand ShowTreesCommand => new Command(() => ShowTrees().FireAndForget());

        private Task ShowTrees()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowTreeList(selectedUnit.CuttingUnitCode);
        }

        public ICommand ShowPlotsCommand => new Command(() => ShowPlots().FireAndForget());

        private Task ShowPlots()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowPlotList(selectedUnit.CuttingUnitCode);
        }

        public ICommand ShowPlotTreesCommand => new Command(() => NavigationService.ShowPlotTreeList(SelectedCuttingUnit?.CuttingUnitCode).FireAndForget());

        public ICommand ShowTallyCommand => new Command(() => ShowTally().FireAndForget());

        private Task ShowTally()
        {
            var selectedUnit = SelectedCuttingUnit;
            if (selectedUnit == null) { return Task.CompletedTask; }

            return NavigationService.ShowTally(selectedUnit.CuttingUnitCode);
        }

        public ICommand ShowSettingsCommand => new Command(() => NavigationService.ShowSettings().FireAndForget());

        public ICommand ShowFeedbackCommand => _showFeedbackCommand ??= new Command(() => NavigationService.ShowFeedback().FireAndForget());

        public ICommand ShowSampleStateManagmentCommand => _showSampleStateManagmentCommand ??= new Command(() => NavigationService.ShowSampleStateManagment().FireAndForget());

        public ICommand ShowCruisersCommand => _showCruisersCommand ??= new Command(() => NavigationService.ShowManageCruisers().FireAndForget());
        public ICommand ShowAuditRulesCommand => _showAuditRulesCommand ??= new Command(() => NavigationService.ShowTreeAuditRules().FireAndForget());
        public ICommand ShowUtilitiesCommand => _showUtilitiesCommand ??= new Command(() => NavigationService.ShowUtilities().FireAndForget());

        public ICommand ShowAboutCommand => _showAbout ??= new Command(() => NavigationService.ShowAbout().FireAndForget());

        public CuttingUnit SelectedCuttingUnit
        {
            get => _selectedCuttingUnit;
            set
            {
                SetProperty(ref _selectedCuttingUnit, value);
                if (value != null)
                {
                    var summary = CuttingUnitDataservice.GetCuttingUnitStrataSummary(value.CuttingUnitCode);
                    SelectedUnitStrataSummary = summary;
                }
                else { SelectedUnitStrataSummary = null; }

                OnPropertyChanged(nameof(IsCuttingUnitSelected));
                RefreshNavOptions();
                NavigationService.ShowBlank();
            }
        }

        public CuttingUnitStrataSummary SelectedUnitStrataSummary
        {
            get => _selectedUnitStratumSummary;
            set
            {
                SetProperty(ref _selectedUnitStratumSummary, value);
                OnPropertyChanged(nameof(HasPlotStrata));
                OnPropertyChanged(nameof(HasTreeStrata));
            }
        }

        public IEnumerable<CuttingUnit> CuttingUnits
        {
            get => _cuttingUnits;
            protected set => SetProperty(ref _cuttingUnits, value);
        }

        public string CurrentCruiseName
        {
            get
            {
                var cruiseID = DataContext?.CruiseID;
                if (cruiseID != null)
                {
                    var cruise = SaleDataservice.GetCruise(cruiseID);
                    return cruise.ToString();
                }
                else
                {
                    return "Select Cruise";
                }
            }
        }

        public bool IsCruiseSelected => DataContext?.CruiseID != null;

        public bool IsCuttingUnitSelected
        {
            get { return IsCruiseSelected && SelectedCuttingUnit != null; }
        }

        public bool HasPlotStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasPlotStrata;

        public bool HasTreeStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasTreeStrata;

        public IEnumerable<NavOption> NavOptions
        {
            get => _navOptions;
            protected set => SetProperty(ref _navOptions, value);
        }

        public List<NavOption> MoreNavOptions
        {
            get => _moreNavOptions;
            protected set => SetProperty(ref _moreNavOptions, value);
        }

        public ICruiseNavigationService NavigationService { get; }
        public IDataContextService DataContext { get; }
        public IAppInfoService AppInfo { get; }
        public INatCruiseDialogService DialogService { get; }
        public IDeviceInfoService DeviceInfo { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public ISaleDataservice SaleDataservice { get; }

        public MainViewModel(
                ICruiseNavigationService navigationService,
                INatCruiseDialogService dialogService,
                IDataContextService dataContext,
                IServiceProvider serviceProvider,
                IDeviceInfoService deviceInfoService,
                IAppInfoService appInfo)
        {
            AppInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DialogService = dialogService;
            DataContext = dataContext;
            DeviceInfo = deviceInfoService ?? throw new ArgumentNullException(nameof(deviceInfoService));

            if (dataContext.CruiseID != null)
            {
                var cuttingUnitDataservice = CuttingUnitDataservice = serviceProvider.GetRequiredService<ICuttingUnitDataservice>();
                SaleDataservice = serviceProvider.GetRequiredService<ISaleDataservice>();
                if (cuttingUnitDataservice != null)
                {
                    CuttingUnits = cuttingUnitDataservice.GetCuttingUnits();
                }
            }

            RefreshNavOptions();
        }

        public void RefreshNavOptions()
        {
            var navOptions = new List<NavOption>();
            //var moreNavOptions = new List<NavOption>();
            var isCruiseSelected = DataContext.CruiseID != null;
            if (isCruiseSelected)
            {
                if (IsCuttingUnitSelected)
                {
                    if (HasTreeStrata)
                    {
                        navOptions.Add(new NavOption { Heading = "Trees", Command = ShowTreesCommand });
                        navOptions.Add(new NavOption { Heading = "Tally", Command = ShowTallyCommand });
                    }

                    if (HasPlotStrata)
                    {
                        navOptions.Add(new NavOption { Heading = "Plot Trees", Command = ShowPlotTreesCommand });
                        navOptions.Add(new NavOption { Heading = "Plots", Command = ShowPlotsCommand });
                    }

                    navOptions.Add(new NavOption { Heading = "Cutting Unit", Command = ShowCuttingUnitCommand });
                }

                navOptions.Add(new NavOption { Heading = "Sale", Command = ShowSaleCommand });

                navOptions.Add(new NavOption { Heading = "Strata", Command = ShowStrataCommand });

                navOptions.Add(new NavOption { Heading = "Audit Rules", Command = ShowAuditRulesCommand });
            }

            navOptions.Add(new NavOption { Heading = "Cruisers", Command = ShowCruisersCommand });
            navOptions.Add(new NavOption { Heading = "Utilities", Command = ShowUtilitiesCommand });

            NavOptions = navOptions;
            //MoreNavOptions = moreNavOptions;
        }
    }
}