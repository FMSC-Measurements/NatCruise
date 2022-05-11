using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.XF.ViewModels
{
    public class ThreePPNTPlotViewModel : XamarinViewModelBase
    {
        private const double DEFAULT_VOLUME_FACTOR = 0.333;

        private int _averageHeight;
        private int _treeCount;
        private double _volumeFactor = DEFAULT_VOLUME_FACTOR;
        private ICommand _addPlotCommand;
        private ICommand _cancelCommand;

        public int AverageHeight
        {
            get => _averageHeight;
            set => SetProperty(ref _averageHeight, value);
        }

        public int TreeCount
        {
            get => _treeCount;
            set => SetProperty(ref _treeCount, value);
        }

        public double VolumeFactor
        {
            get => _volumeFactor;
            set => SetProperty(ref _volumeFactor, value);
        }

        public ICommand AddPlotCommand => _addPlotCommand ??= new DelegateCommand(() => AddPlotAsync().FireAndForget());

        public ICommand CancelCommand => _cancelCommand ??= new DelegateCommand(() => Cancel().FireAndForget());

        protected ICruiseNavigationService NavigationService { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public Random Random { get; }
        protected IPlotDataservice PlotDataservice { get; }
        protected IPlotTreeDataservice PlotTallyDataservice { get; }
        protected INatCruiseDialogService DialogService { get; }

        public Plot_Stratum StratumPlot { get; protected set; }

        public ThreePPNTPlotViewModel(ICruiseNavigationService navigationService,
            IPlotDataservice plotDataservice,
            IPlotTreeDataservice plotTallyDataservice,
            INatCruiseDialogService dialogService,
            ISampleGroupDataservice sampleGroupDataservice,
            Random random)
        {
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            PlotTallyDataservice = plotTallyDataservice ?? throw new ArgumentNullException(nameof(plotTallyDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            Random = random ?? throw new ArgumentNullException(nameof(random));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var plotNum = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            var plotStratum = PlotDataservice.GetPlot_Stratum(unit, stratumCode, plotNum);
            StratumPlot = plotStratum;
        }

        public double CalculateKPI()
        {
            return ThreePPNTPlotViewModel.CalculateKPI(TreeCount, StratumPlot.BAF, AverageHeight, VolumeFactor);
        }

        public static int CalculateKPI(int treeCount, double baf, int averageHeight, double volumeFactor)
        {
            if (treeCount <= 0 || averageHeight <= 0)
            {
                return 0;
            }
            else
            {
                return (int)Math.Round((treeCount * averageHeight) * (baf * volumeFactor));
            }
        }

        private Task Cancel()
        {
            return NavigationService.GoBackAsync();
        }

        private async Task AddPlotAsync()
        {
            var kpi = CalculateKPI();
            if (kpi <= 0)
            {
                await DialogService.ShowMessageAsync("Invalid Input");
                return;
            }

            var datastore = PlotDataservice;

            var plotStratum = StratumPlot;
            var unit = plotStratum.CuttingUnitCode;
            var stratumCode = plotStratum.StratumCode;
            var plotNumber = plotStratum.PlotNumber;

            var sampleGroupCodes = SampleGroupDataservice.GetSampleGroupCodes(stratumCode);
            var sgCode = sampleGroupCodes.First();

            var randomValue = Random.Next(1, StratumPlot.KZ3PPNT);

            plotStratum.InCruise = true;
            plotStratum.TreeCount = TreeCount;
            plotStratum.AverageHeight = AverageHeight;
            plotStratum.CountOrMeasure = (kpi > randomValue) ? "M" : "C";
            datastore.Insert3PPNT_Plot_Stratum(plotStratum);

            if (kpi > randomValue)
            {
                for (var i = 0; i < TreeCount; i++)
                {
                    PlotTallyDataservice.CreatePlotTree(unit, plotNumber, stratumCode, sgCode, treeCount: 1, countMeasure: "M");
                }

                await DialogService.ShowMessageAsync("Measure Plot");

                //await NavigationService.NavigateAsync($"/Main/Navigation/Plots/PlotTally?UnitCode={stratumPlot.UnitCode}&PlotNumber={stratumPlot.PlotNumber}");
            }
            else
            {
                PlotTallyDataservice.CreatePlotTree(unit, plotNumber, stratumCode, sgCode, treeCount: TreeCount, countMeasure: "C");

                await DialogService.ShowMessageAsync("Count Plot");
            }

            await NavigationService.GoBackAsync();
        }
    }
}