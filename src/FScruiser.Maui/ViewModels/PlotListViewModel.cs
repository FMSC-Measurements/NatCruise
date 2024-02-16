using CommunityToolkit.Mvvm.Input;
using FScruiser.Maui.Services;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Common;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class PlotListViewModel : ViewModelBase
{
    private ICommand? _addPlotCommand;
    private ICommand? _editPlotCommand;
    private ICommand? _deletePlotCommand;
    private ICommand? _showTallyPlotCommand;
    private CuttingUnit? _cuttingUnit;

    public string? UnitCode => CuttingUnit?.CuttingUnitCode;

    public CuttingUnit? CuttingUnit
    {
        get => _cuttingUnit;
        protected set
        {
            SetProperty(ref _cuttingUnit, value);
            OnPropertyChanged(nameof(UnitCode));
            OnPropertyChanged(nameof(Title));
        }
    }

    public string Title => $"Unit {UnitCode} - {CuttingUnit?.Description} Plots";

    public bool HasFixCNTStrata { get; set; }

    public IEnumerable<Plot>? Plots { get; protected set; }
    public INatCruiseDialogService DialogService { get; }

    protected IPlotDataservice PlotDataservice { get; }
    public ICuttingUnitDataservice CuttingUnitDataservice { get; }
    public IStratumDataservice StratumDataservice { get; }
    protected ICruiseNavigationService NavigationService { get; }

    public ICommand AddPlotCommand => _addPlotCommand ??= new RelayCommand(() => AddPlot().FireAndForget());
    public ICommand DeletePlotCommand => _deletePlotCommand ??= new RelayCommand<Plot>(DeletePlot);
    public ICommand EditPlotCommand => _editPlotCommand ??= new RelayCommand<Plot>((p) => ShowEditPlot(p).FireAndForget());
    public ICommand ShowTallyPlotCommand => _showTallyPlotCommand ??= new RelayCommand<Plot>((p) => ShowTallyPlot(p).FireAndForget());

    public PlotListViewModel(ICruiseNavigationService navigationService,
        INatCruiseDialogService dialogService,
        IPlotDataservice plotDataservice,
        ICuttingUnitDataservice cuttingUnitDataservice,
        IStratumDataservice stratumDataservice)
    {
        PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
        CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
        StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    protected override void Load(IDictionary<string, object> parameters)
    {
        if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        var unitCode = parameters.GetValue<string>(NavParams.UNIT);
        if (string.IsNullOrEmpty(unitCode) is false)
        {
            CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);
        }
        //else
        //{
        //    // when Plot Tally Page is navigated we want to load Plot List behind it in the nav stack
        //    // because we just have the plot ID, so we need to read the plot and get the unit code.
        //    var plotID = parameters.GetValue<string>(NavParams.PlotID);
        //    if (string.IsNullOrEmpty(plotID) == false)
        //    {
        //        var plot = PlotDataservice.GetPlot(plotID);
        //        CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(plot.CuttingUnitCode);
        //    }
        //}

        RefreshPlots();
    }

    private void RefreshPlots()
    {
        var unitCode = UnitCode;

        Plots = PlotDataservice.GetPlotsByUnitCode(UnitCode).ToArray();
        OnPropertyChanged(nameof(Plots));

        HasFixCNTStrata = CuttingUnitDataservice.GetCuttingUnitStrataSummary(UnitCode)
            .Methods
            .Any(x => x == CruiseDAL.Schema.CruiseMethods.FIXCNT);
    }

    public Task AddPlot()
    {
        var plotID = PlotDataservice.AddNewPlot(UnitCode);
        return NavigationService.ShowPlotEdit(plotID);
    }

    private void DeletePlot(Plot? plot)
    {
        if (plot is null) return;

        var unitCode = UnitCode;
        var plotNumber = plot.PlotNumber;

        PlotDataservice.DeletePlot(unitCode, plotNumber);

        RefreshPlots();
    }

    public Task ShowEditPlot(Plot? plot)
    {
        if (plot is null) return Task.CompletedTask;
        return NavigationService.ShowPlotEdit(plot.PlotID);
    }

    public async Task ShowTallyPlot(Plot? plot)
    {
        if (plot is null) return;

        var fixCNTstrata = StratumDataservice.GetPlotStrata(UnitCode)
            .Where(x => x.Method == CruiseDAL.Schema.CruiseMethods.FIXCNT)
            .ToArray();

        if (fixCNTstrata.Any()
            && await DialogService.AskValueAsync("Show FixCNT Tally Page?", "FixCNT", "Standard") == "FixCNT")
        {
            string stratum = null;
            if (fixCNTstrata.Count() == 1)
            {
                stratum = fixCNTstrata.Single().StratumCode;
            }
            else
            {
                stratum = await DialogService.AskValueAsync("Select Stratum", fixCNTstrata.Select(x => x.StratumCode).ToArray());
            }

            if (stratum == null || stratum == "Cancel") { return; }
            await NavigationService.ShowFixCNT(UnitCode, plot.PlotNumber, stratum);
        }
        else
        {
            await NavigationService.ShowPlotTally(plot.PlotID);
        }
    }
}