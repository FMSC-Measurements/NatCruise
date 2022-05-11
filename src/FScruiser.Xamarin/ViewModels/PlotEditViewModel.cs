using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotEditViewModel : XamarinViewModelBase
    {
        private Plot _plot;
        private IEnumerable<Plot_Stratum> _stratumPlots;
        private Command<Plot_Stratum> _showLimitingDistanceCommand;
        private Command<Plot_Stratum> _toggleInCruiseCommand;
        private IEnumerable<PlotError> _errorsAndWarnings;
        private ICommand _updatePlotNumberCommand;

        protected ICruiseNavigationService NavigationService { get; }

        public IEnumerable<PlotError> ErrorsAndWarnings
        {
            get => _errorsAndWarnings;
            set => SetProperty(ref _errorsAndWarnings, value);
        }

        public Plot Plot
        {
            get => _plot;
            set
            {
                var oldValue = _plot;
                if (oldValue != null)
                {
                    oldValue.PropertyChanged -= Plot_PropertyChanged;
                }

                SetProperty(ref _plot, value);
                RaisePropertyChanged(nameof(PlotNumber));

                if (value != null)
                {
                    value.PropertyChanged += Plot_PropertyChanged;
                }
            }
        }

        public ICommand UpdatePlotNumberCommand => _updatePlotNumberCommand ?? (_updatePlotNumberCommand = new Command<string>(UpdatePlotNumber));

        public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ?? (_showLimitingDistanceCommand = new Command<Plot_Stratum>(async x => await ShowLimitingDistanceCalculatorAsync(x)));

        public ICommand ToggleInCruiseCommand => _toggleInCruiseCommand ?? (_toggleInCruiseCommand = new Command<Plot_Stratum>(async (x) => await ToggleInCruiseAsync(x)));

        #region PlotNumber

        public int PlotNumber
        {
            get { return Plot?.PlotNumber ?? 0; }
            protected set
            {
                var plot = Plot;
                if (plot != null)
                {
                    plot.PlotNumber = value;
                }
            }
        }

        private void OnPlotNumberChanged(int plotNumber)
        {
            //var stratumPlots = StratumPlots;
            //if (stratumPlots != null)
            //{
            //    foreach (var stratumPlot in stratumPlots)
            //    {
            //        stratumPlot.PlotNumber = plotNumber;
            //    }
            //}

            PlotDataservice.UpdatePlotNumber(Plot.PlotID, plotNumber);

            RaisePropertyChanged(nameof(PlotNumber));
        }

        private bool OnPlotNumberChanging(int oldValue, int newValue)
        {
            if (PlotDataservice.IsPlotNumberAvalible(UnitCode, newValue))
            {
                return true;
            }
            else
            {
                DialogService.ShowNotification("Plot Number Already Takend");
                return false;
            }
        }

        private void UpdatePlotNumber(string value)
        {
            if (int.TryParse(value, out var plotNumber))
            {
                UpdatePlotNumber(plotNumber);
            }
            else
            {
                // refresh displayed value
                RaisePropertyChanged(nameof(PlotNumber));
            }
        }

        private void UpdatePlotNumber(int newValue)
        {
            var oldValue = Plot.PlotNumber;
            // HACK because UpdatePlotNumber may be called twice when the value changes,
            // i.e. once when the control loses focus and once when the ReturnCommand is fired
            // we need to assure that the old and new values are different
            if (oldValue == newValue) { return; }

            if (PlotDataservice.IsPlotNumberAvalible(UnitCode, newValue))
            {
                PlotDataservice.UpdatePlotNumber(Plot.PlotID, newValue);

                PlotNumber = newValue;
            }
            else
            {
                DialogService.ShowNotification("Plot Number Already Takend");
            }

            // refresh displayed value
            RaisePropertyChanged(nameof(PlotNumber));
        }

        #endregion PlotNumber

        public string UnitCode => Plot?.CuttingUnitCode;

        public IEnumerable<Plot_Stratum> StratumPlots
        {
            get { return _stratumPlots; }
            set
            {
                var oldValue = _stratumPlots;
                if (oldValue != null)
                {
                    foreach (var sp in oldValue)
                    {
                        sp.PropertyChanged -= StratumPlot_PropertyChanged;
                    }
                }

                SetProperty(ref _stratumPlots, value);

                if (value != null)
                {
                    foreach (var sp in value)
                    {
                        sp.PropertyChanged += StratumPlot_PropertyChanged;
                    }
                }
            }
        }

        public IPlotDataservice PlotDataservice { get; }
        public INatCruiseDialogService DialogService { get; set; }

        public PlotEditViewModel(IPlotDataservice plotDataservice
            , INatCruiseDialogService dialogService
            , ICruiseNavigationService navigationService)
        {
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public async Task ToggleInCruiseAsync(Plot_Stratum stratumPlot)
        {
            var plot = Plot;
            var plotNumber = plot.PlotNumber;
            var stratumCode = stratumPlot.StratumCode;

            if (stratumPlot.InCruise)
            {
                var hasTreeData = PlotDataservice.GetNumTreeRecords(UnitCode, stratumCode, plotNumber) > 0;

                if (hasTreeData)
                {
                    if (await DialogService.AskYesNoAsync("Removing stratum will delete all tree data", "Continue?"))
                    {
                        PlotDataservice.DeletePlot_Stratum(stratumPlot.CuttingUnitCode, stratumCode, plotNumber);
                        stratumPlot.InCruise = false;
                    }
                }
                else
                {
                    PlotDataservice.DeletePlot_Stratum(stratumPlot.CuttingUnitCode, stratumCode, plotNumber);
                    stratumPlot.InCruise = false;
                }
            }
            else
            {
                if (stratumPlot.CruiseMethod == CruiseDAL.Schema.CruiseMethods.THREEPPNT)
                {
                    //var query = $"{NavParams.UNIT}={stratumPlot.CuttingUnitCode}&{NavParams.PLOT_NUMBER}={plotNumber}&{NavParams.STRATUM}={stratumCode}";

                    //await NavigationService.NavigateAsync("ThreePPNTPlot",
                    //    new NavigationParameters(query));

                    await NavigationService.ShowThreePPNTPlot(stratumPlot.CuttingUnitCode, stratumCode, plotNumber);
                }
                else
                {
                    PlotDataservice.InsertPlot_Stratum(stratumPlot);
                    stratumPlot.InCruise = true;
                }
            }

            RefreshErrorsAndWarnings(plot);
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var plotID = parameters.GetValue<string>(NavParams.PlotID);
            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            Plot plot = null;
            if (string.IsNullOrWhiteSpace(plotID) == false)
            {
                plot = PlotDataservice.GetPlot(plotID);
            }
            else
            {
                plot = PlotDataservice.GetPlot(unitCode, plotNumber);
            }

            var stratumPlots = PlotDataservice.GetPlot_Strata(plot.CuttingUnitCode, plot.PlotNumber);

            Plot = plot;
            StratumPlots = stratumPlots;

            RefreshErrorsAndWarnings(plot);
        }

        protected void RefreshErrorsAndWarnings(Plot plot)
        {
            var errorsAndWarnings = PlotDataservice.GetPlotErrors(plot.PlotID);
            ErrorsAndWarnings = errorsAndWarnings;
        }

        private void Plot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var plot = Plot;

            switch (e.PropertyName)
            {
                case nameof(Plot.Aspect):
                case nameof(Plot.Slope):
                case nameof(Plot.Remarks):
                    {
                        PlotDataservice.UpdatePlot(plot);
                        break;
                    }
            }
        }

        private void StratumPlot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Plot_Stratum stratumPlot && stratumPlot != null)
            {
                var propertyName = e.PropertyName;
                if (e.PropertyName == nameof(Plot_Stratum.InCruise)) { return; }

                if (stratumPlot.InCruise)
                {
                    PlotDataservice.UpdatePlot_Stratum(stratumPlot);
                }

                RefreshErrorsAndWarnings(Plot);
            }
        }

        public Task ShowLimitingDistanceCalculatorAsync(Plot_Stratum stratumPlot)
        {
            return NavigationService.ShowLimitingDistance(UnitCode, stratumPlot.StratumCode, stratumPlot.PlotNumber);
        }
    }
}