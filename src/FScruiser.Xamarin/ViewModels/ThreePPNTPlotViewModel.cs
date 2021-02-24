using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using NatCruise.Data;

namespace FScruiser.XF.ViewModels
{
    public class ThreePPNTPlotViewModel : ViewModelBase
    {
        private const double DEFAULT_VOLUME_FACTOR = 0.333;

        private int _averageHeight;
        private int _treeCount;
        private double _volumeFactor = 0.333;
        private Command _addPlotCommand;
        private Command _cancelCommand;

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
        }

        public ICommand AddPlotCommand => _addPlotCommand ?? (_addPlotCommand = new Command(async () => await AddPlotAsync()));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new Command(Cancel));

        protected ICruiseNavigationService NavigationService { get; }
        protected ICuttingUnitDatastore Datastore { get; }
        protected ICruiseDialogService DialogService { get; }

        public Plot_Stratum StratumPlot { get; protected set; }

        public ThreePPNTPlotViewModel(ICruiseNavigationService navigationService,
            IDataserviceProvider datastoreProvider,
            ICruiseDialogService dialogService)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            Datastore = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var plotNum = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            var plotStratum = Datastore.GetPlot_Stratum(unit, stratumCode, plotNum);
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

        private void Cancel()
        {
            NavigationService.GoBackAsync();
        }

        private async System.Threading.Tasks.Task AddPlotAsync()
        {
            var kpi = CalculateKPI();
            if (kpi <= 0)
            {
                await DialogService.ShowMessageAsync("Invalid Input");
                return;
            }

            var datastore = Datastore;

            var plotStratum = StratumPlot;
            var unit = plotStratum.CuttingUnitCode;
            var stratumCode = plotStratum.StratumCode;
            var plotNumber = plotStratum.PlotNumber;

            plotStratum.InCruise = true;
            datastore.InsertPlot_Stratum(plotStratum);

            var random = new Random();
            var randomValue = random.Next(1, StratumPlot.KZ3PPNT);

            if (kpi > randomValue)
            {
                for (var i = 0; i < TreeCount; i++)
                {
                    datastore.CreatePlotTree(unit, plotNumber, stratumCode, treeCount: 1, countMeasure: "M");
                }

                await DialogService.ShowMessageAsync("Measure Plot");

                //await NavigationService.NavigateAsync($"/Main/Navigation/Plots/PlotTally?UnitCode={stratumPlot.UnitCode}&PlotNumber={stratumPlot.PlotNumber}");
            }
            else
            {
                datastore.CreatePlotTree(unit, plotNumber, stratumCode, treeCount: TreeCount, countMeasure: "C");

                await DialogService.ShowMessageAsync("Count Plot");
            }

            await NavigationService.GoBackAsync();
        }
    }
}