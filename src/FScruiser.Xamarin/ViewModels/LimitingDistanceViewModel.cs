using NatCruise.Cruise.Data;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class LimitingDistanceViewModel : XamarinViewModelBase//, INavigatedAware
    {
        public const String MEASURE_TO_FACE = "Face";
        public const String MEASURE_TO_CENTER = "Center";
        public static readonly String[] MEASURE_TO_OPTIONS = new String[] { MEASURE_TO_FACE, MEASURE_TO_CENTER };

        private bool? _isTreeIn = null;
        private double _bafOrFps;
        private double _dbh;
        private int _slopePCT;
        private double? _slopeDistance;
        private double _limitingDistance;
        private double _azimuth;
        private bool _isToFace = true;
        private bool _isVariableRadius;
        private Plot_Stratum _plot;

        protected IDataserviceProvider DataserviceProvider { get; }
        public IPlotStratumDataservice PlotStratumDataservice { get; protected set; }
        public IPlotDataservice PlotDataservice { get; protected set; }
        public INatCruiseDialogService DialogService { get; }

        public Plot_Stratum PlotStratum
        {
            get => _plot;
            set => SetProperty(ref _plot, value);
        }

        public double BafOrFps
        {
            get { return _bafOrFps; }
            set
            {
                _bafOrFps = value;
                Calculate();
            }
        }

        public double DBH
        {
            get { return _dbh; }
            set
            {
                _dbh = value;
                Calculate();
            }
        }

        public int SlopePCT
        {
            get { return _slopePCT; }
            set
            {
                _slopePCT = value;
                Calculate();
            }
        }

        // HACK xamarin really doesn't like binding to nullible types
        // so instead we will bind the text box to a property that exposes SlopeDistance as a string.
        // see: https://forums.xamarin.com/discussion/144704/binding-to-nullable-int
        public string SlopeDistanceStr
        {
            get => SlopeDistance?.ToString() ?? "";
            set
            {
                if (value != null && double.TryParse(value, out var d))
                {
                    SlopeDistance = d;
                }
                else
                {
                    SlopeDistance = null;
                }
            }
        }

        public double? SlopeDistance
        {
            get { return _slopeDistance; }
            set
            {
                _slopeDistance = value;
                Calculate();
            }
        }

        public double Azimuth
        {
            get { return _azimuth; }
            set
            {
                _azimuth = value;
            }
        }

        public bool IsToFace
        {
            get { return _isToFace; }
            set
            {
                SetProperty(ref _isToFace, value);
                Calculate();
                RaisePropertyChanged(nameof(MeasureToSelection));
            }
        }

        public bool IsVariableRadius
        {
            get { return _isVariableRadius; }
            set
            {
                SetProperty(ref _isVariableRadius, value);
                Calculate();
            }
        }

        public IEnumerable<string> MeasureToOptions => MEASURE_TO_OPTIONS;

        public string MeasureToSelection
        {
            get { return (IsToFace) ? MEASURE_TO_FACE : MEASURE_TO_CENTER; }
            set
            {
                switch (value)
                {
                    case MEASURE_TO_FACE: { IsToFace = true; break; }
                    case MEASURE_TO_CENTER: { IsToFace = false; break; }
                    default: { break; }
                }
            }
        }

        public double LimitingDistance
        {
            get { return _limitingDistance; }
            set { SetProperty(ref _limitingDistance, value); }
        }

        public bool? IsTreeIn
        {
            get { return _isTreeIn; }
            set
            {
                SetProperty(ref _isTreeIn, value);
                RaisePropertyChanged(nameof(TreeStatus));
            }
        }

        public string TreeStatus
        {
            get
            {
                if (IsTreeIn.HasValue == false) { return ""; }
                else if (IsTreeIn == true) { return "IN"; }
                else { return "OUT"; }
            }
        }

        public ICommand SaveReportToPlotCommand => new Command(SaveReport);

        public LimitingDistanceViewModel(IDataserviceProvider dataserviceProvider, INatCruiseDialogService dialogService)
        {
            // we need DataserviceProvider because we might not know if a cruise is selected until Load gets called
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        //void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        //{
        //    // do nothing
        //}

        //public void OnNavigatedFrom(INavigationParameters parameters)
        //{
        //    if (TreeStatus != null)
        //    {
        //        SaveReport();
        //    }
        //}

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            if (unitCode != null && stratumCode != null && plotNumber != null)
            {
                var ds = PlotStratumDataservice = DataserviceProvider.GetDataservice<IPlotStratumDataservice>();
                var pDs = PlotDataservice = DataserviceProvider.GetDataservice<IPlotDataservice>();
                var plotSt = ds.GetPlot_Stratum(unitCode, stratumCode, plotNumber);

                if (plotSt != null)
                {
                    var isVariableRadious = IsVariableRadius = CruiseDAL.Schema.CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(plotSt.CruiseMethod);

                    BafOrFps = (isVariableRadious) ? plotSt.BAF : plotSt.FPS;

                    PlotStratum = plotSt;

                    RaisePropertyChanged(nameof(BafOrFps));
                }
            }
        }

        public bool CanCalculate()
        {
            return BafOrFps > 0.0 && DBH > 0.0;
        }

        public void Calculate()
        {
            if (!CanCalculate())
            {
                IsTreeIn = null;
                LimitingDistance = 0.0;
                return;
            }

            var limitingDistance = LimitingDistance = CalculateLimitingDistance.Calculate(BafOrFps, DBH, SlopePCT, IsVariableRadius, IsToFace);
            var slopeDistance = SlopeDistance;

            if (slopeDistance.HasValue)
            {
                IsTreeIn = CalculateLimitingDistance.DeterminTreeInOrOut(slopeDistance.Value, limitingDistance);
            }
            else
            {
                IsTreeIn = null;
            }
        }

        protected string GenerateReport()
        {
            Calculate();

            if (IsTreeIn.HasValue)
            {
                return CalculateLimitingDistance.GenerateReport(TreeStatus, LimitingDistance, SlopeDistance.Value,
                    SlopePCT, Azimuth, BafOrFps, DBH, IsVariableRadius, IsToFace, PlotStratum?.StratumCode);
            }
            else { return null; }
        }

        public void SaveReport()
        {
            var plot = PlotStratum;
            if (plot != null)
            {
                var report = GenerateReport();
                if (!string.IsNullOrEmpty(report))
                {
                    PlotDataservice.AddPlotRemark(plot.CuttingUnitCode, plot.PlotNumber, report);
                    DialogService.ShowNotification(report, "Report Saved");
                }
            }
        }
    }
}