using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Common;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotListViewModel : XamarinViewModelBase
    {
        private ICommand _addPlotCommand;
        private Command<Plot> _editPlotCommand;
        private ICommand _deletePlotCommand;
        private ICommand _showTallyPlotCommand;
        private CuttingUnit _cuttingUnit;

        public string UnitCode => CuttingUnit?.CuttingUnitCode;

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            protected set
            {
                SetProperty(ref _cuttingUnit, value);
                RaisePropertyChanged(nameof(UnitCode));
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Title => $"Unit {UnitCode} - {CuttingUnit?.Description} Plots";

        public bool HasFixCNTStrata { get; set; }

        public IEnumerable<Plot> Plots { get; protected set; }
        public IPageDialogService DialogService { get; }

        protected IPlotDataservice PlotDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public IStratumDataservice StratumDataservice { get; }
        protected ICruiseNavigationService NavigationService { get; }

        public ICommand AddPlotCommand => _addPlotCommand ??= new Command(() => AddPlot().FireAndForget());
        public ICommand DeletePlotCommand => _deletePlotCommand ??= new Command<Plot>(DeletePlot);
        public ICommand EditPlotCommand => _editPlotCommand ??= new Command<Plot>((p) => ShowEditPlot(p).FireAndForget());
        public ICommand ShowTallyPlotCommand => _showTallyPlotCommand ??= new Command<Plot>((p) => ShowTallyPlot(p).FireAndForget());

        public PlotListViewModel(ICruiseNavigationService navigationService,
            IPageDialogService dialogService,
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

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var cuttingUnit = CuttingUnit = CuttingUnitDataservice.GetCuttingUnit(unitCode);
            RefreshPlots();
        }

        private void RefreshPlots()
        {
            var unitCode = UnitCode;

            Plots = PlotDataservice.GetPlotsByUnitCode(UnitCode).ToArray();
            RaisePropertyChanged(nameof(Plots));

            HasFixCNTStrata = CuttingUnitDataservice.GetCuttingUnitStrataSummary(UnitCode)
                .Methods
                .Any(x => x == CruiseDAL.Schema.CruiseMethods.FIXCNT);
        }

        public Task AddPlot()
        {
            var plotID = PlotDataservice.AddNewPlot(UnitCode);
            return NavigationService.ShowPlotEdit(plotID);
        }

        private void DeletePlot(Plot plot)
        {
            var unitCode = UnitCode;
            var plotNumber = plot.PlotNumber;

            PlotDataservice.DeletePlot(unitCode, plotNumber);

            RefreshPlots();
        }

        public Task ShowEditPlot(Plot plot)
        {
            return NavigationService.ShowPlotEdit(plot.PlotID);
        }

        public async Task ShowTallyPlot(Plot plot)
        {
            var fixCNTstrata = StratumDataservice.GetPlotStrata(UnitCode)
                .Where(x => x.Method == CruiseDAL.Schema.CruiseMethods.FIXCNT)
                .ToArray();

            if (fixCNTstrata.Any()
                && await DialogService.DisplayAlertAsync("Show FixCNT Tally Page?", "", "FixCNT", "Standard"))
            {
                string stratum = null;
                if (fixCNTstrata.Count() == 1)
                {
                    stratum = fixCNTstrata.Single().StratumCode;
                }
                else
                {
                    stratum = await DialogService.DisplayActionSheetAsync("Select Stratum", "Cancel", "", fixCNTstrata.Select(x => x.StratumCode).ToArray());
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
}