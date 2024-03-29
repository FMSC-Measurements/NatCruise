﻿using FScruiser.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class ShellViewModel : ViewModelBase
{
    public record class NavOption(string Heading, ICommand Command)
    {
    }

    private IEnumerable<CuttingUnit>? _cuttingUnits;
    private CuttingUnit? _selectedCuttingUnit;
    private CuttingUnitStrataSummary? _selectedUnitStratumSummary;
    private IEnumerable<NavOption>? _navOptions;
    private ICommand? _showAuditRulesCommand;
    private ICommand? _showUtilitiesCommand;
    private ICommand? _showCruisersCommand;
    private ICommand? _showSampleStateManagmentCommand;
    private ICommand? _showFeedbackCommand;
    private List<NavOption>? _moreNavOptions;
    private ICommand? _showAbout;
    private ICommand? _showSettingsCommand;
    private ICommand? _showPlotsCommand;
    private ICommand? _showTreeCommand;
    private string? _currentCruiseName;
    private bool _isCruiseSelected;

    public ICommand ShowSelectSale => new Command(() => NavigationService.ShowSaleSelect().FireAndForget());

    public ICommand ShowSaleCommand => new Command(() => NavigationService.ShowSale(DataContextService?.CruiseID).FireAndForget());

    //public ICommand ShowUnitsCommand => new Command(() => NavigationService.ShowCuttingUnitList().FireAndForget());

    public ICommand ShowStrataCommand => new Command(() => NavigationService.ShowStrata().FireAndForget());

    public ICommand ShowCuttingUnitCommand => new Command(() => NavigationService.ShowCuttingUnitInfo(SelectedCuttingUnit?.CuttingUnitCode).FireAndForget());

    public ICommand ShowTreesCommand => _showTreeCommand ??= new Command(() => ShowTrees().FireAndForget());

    private Task ShowTrees()
    {
        var selectedUnit = SelectedCuttingUnit;
        if (selectedUnit == null) { return Task.CompletedTask; }

        return NavigationService.ShowTreeList(selectedUnit.CuttingUnitCode);
    }

    public ICommand ShowPlotsCommand => _showPlotsCommand ??= new Command(() => ShowPlots().FireAndForget());

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

    public ICommand ShowSettingsCommand => _showSettingsCommand ??= new Command(() => NavigationService.ShowSettings().FireAndForget());

    public ICommand ShowFeedbackCommand => _showFeedbackCommand ??= new Command(() => NavigationService.ShowFeedback().FireAndForget());

    public ICommand ShowSampleStateManagmentCommand => _showSampleStateManagmentCommand ??= new Command(() => NavigationService.ShowSampleStateManagment().FireAndForget());

    public ICommand ShowCruisersCommand => _showCruisersCommand ??= new Command(() => NavigationService.ShowManageCruisers().FireAndForget());
    public ICommand ShowAuditRulesCommand => _showAuditRulesCommand ??= new Command(() => NavigationService.ShowTreeAuditRules().FireAndForget());
    public ICommand ShowUtilitiesCommand => _showUtilitiesCommand ??= new Command(() => NavigationService.ShowUtilities().FireAndForget());

    public ICommand ShowAboutCommand => _showAbout ??= new Command(() => NavigationService.ShowAbout().FireAndForget());

    public CuttingUnit? SelectedCuttingUnit
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

    public CuttingUnitStrataSummary? SelectedUnitStrataSummary
    {
        get => _selectedUnitStratumSummary;
        set
        {
            SetProperty(ref _selectedUnitStratumSummary, value);
            OnPropertyChanged(nameof(HasPlotStrata));
            OnPropertyChanged(nameof(HasTreeStrata));
        }
    }

    public IEnumerable<CuttingUnit>? CuttingUnits
    {
        get => _cuttingUnits;
        protected set => SetProperty(ref _cuttingUnits, value);
    }

    public string? CurrentCruiseName
    {
        get
        {
            return _currentCruiseName;
        }
        set
        {
            SetProperty(ref _currentCruiseName, value);
        }
    }

    public bool IsCruiseSelected
    {
        get => _isCruiseSelected;
        set => SetProperty(ref _isCruiseSelected, value);
    }

    public bool IsCuttingUnitSelected
    {
        get { return IsCruiseSelected && SelectedCuttingUnit != null; }
    }

    public bool HasPlotStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasPlotStrata;

    public bool HasTreeStrata => IsCuttingUnitSelected && SelectedUnitStrataSummary.HasTreeStrata;

    public IEnumerable<NavOption>? NavOptions
    {
        get => _navOptions;
        protected set => SetProperty(ref _navOptions, value);
    }

    public List<NavOption>? MoreNavOptions
    {
        get => _moreNavOptions;
        protected set => SetProperty(ref _moreNavOptions, value);
    }

    public ICruiseNavigationService NavigationService { get; }
    public IAppInfoService AppInfo { get; }
    public INatCruiseDialogService DialogService { get; }
    public IDataContextService DataContextService { get; }
    public IServiceProvider Services { get; }
    public IDeviceInfoService DeviceInfo { get; }
    public ICuttingUnitDataservice CuttingUnitDataservice { get; protected set; }

    public ShellViewModel(
            ICruiseNavigationService navigationService,
            INatCruiseDialogService dialogService,
            IDataContextService dataContextService,
            IServiceProvider serviceProvider,
            IAppInfoService appInfo)
    {
        AppInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        DialogService = dialogService;
        DataContextService = dataContextService;
        Services = serviceProvider;

        dataContextService.CruiseChanged += DataContextService_CruiseChanged;
    }

    private void DataContextService_CruiseChanged(object? sender, EventArgs e)
    {
        Load();
    }

    public override void Load()
    {
        base.Load();

        var cruiseID = DataContextService?.CruiseID;
        if (cruiseID != null)
        {
            var saleDataservice = Services.GetRequiredService<ISaleDataservice>();
            var cruise = saleDataservice.GetCruise(cruiseID);
            CurrentCruiseName = cruise.ToString();

            var cuttingUnitDataservice = CuttingUnitDataservice = Services.GetRequiredService<ICuttingUnitDataservice>();
            CuttingUnits = cuttingUnitDataservice.GetCuttingUnits();
            IsCruiseSelected = true;
        
        }
        else
        {
            IsCruiseSelected = false;
            CuttingUnits = null;
            CurrentCruiseName = "Select Cruise";
        }

        RefreshNavOptions();
    }

    public void RefreshNavOptions()
    {
        var navOptions = new List<NavOption>();
        //var moreNavOptions = new List<NavOption>();
        var isCruiseSelected = DataContextService.CruiseID != null;
        if (isCruiseSelected)
        {
            if (IsCuttingUnitSelected)
            {
                if (HasTreeStrata)
                {
                    navOptions.Add(new NavOption("Trees", ShowTreesCommand));
                    navOptions.Add(new NavOption("Tally", ShowTallyCommand));
                }

                if (HasPlotStrata)
                {
                    navOptions.Add(new NavOption("Plot Trees", ShowPlotTreesCommand));
                    navOptions.Add(new NavOption("Plots", ShowPlotsCommand));
                }

                navOptions.Add(new NavOption("Cutting Unit", ShowCuttingUnitCommand));
            }

            navOptions.Add(new NavOption("Sale", ShowSaleCommand));

            navOptions.Add(new NavOption("Strata", ShowStrataCommand));

            navOptions.Add(new NavOption("Audit Rules", ShowAuditRulesCommand));
        }

        navOptions.Add(new NavOption("Cruisers", ShowCruisersCommand));
        navOptions.Add(new NavOption("Utilities", ShowUtilitiesCommand));

        NavOptions = navOptions;
        //MoreNavOptions = moreNavOptions;
    }
}