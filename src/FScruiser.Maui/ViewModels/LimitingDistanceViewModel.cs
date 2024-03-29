﻿using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Logic;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Common;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels
{
    public class LimitingDistanceViewModel : ViewModelBase//, INavigatedAware
    {
        public const string TREE_STATUS_IN = "IN";
        public const string TREE_STATUS_OUT = "OUT";

        public const String MEASURE_TO_FACE = "Face";
        public const String MEASURE_TO_CENTER = "Center";
        public static readonly String[] MEASURE_TO_OPTIONS = new String[] { MEASURE_TO_FACE, MEASURE_TO_CENTER };

        public const String MODE_FPS = "fps";
        public const String MODE_BAF = "bar";
        public const String MODE_STRATUM = "stratum";

        private bool? _isTreeIn = null;
        private decimal _dbh;
        private int _slopePCT;
        private decimal? _slopeDistance;
        private decimal _limitingDistance;
        private decimal _azimuth;
        private bool _isToFace = true;
        private Plot? _plot;
        private decimal _baf;
        private decimal _fps;
        private string? _stratumMode;
        private IEnumerable<Stratum>? _stratumOptions;
        private Stratum? _stratum;
        private bool _useBigBAF;
        private IEnumerable<SampleGroup>? _bigBAFSampleGroupOptions;
        private SampleGroup? _bigBAFSampleGroup;
        private string? _treeNumber;
        private ICommand? _saveReportToPlotCommand;
        private ICommand? _copyReportToClipboardCommand;

        public INatCruiseDialogService DialogService { get; }
        public IPlotDataservice PlotDataservice { get; protected set; }
        public ISampleGroupDataservice SampleGroupDataservice { get; protected set; }
        public IStratumDataservice StratumDataservice { get; protected set; }

        public ILimitingDistanceCalculator LimitingDistanceCalculator { get; }

        public IClipboard Clipboard { get; }

        public bool UseNewLimitingDistanceCalculator { get; }

        #region stratum settings

        public IEnumerable<Stratum>? StratumOptions
        {
            get => _stratumOptions;
            set => SetProperty(ref _stratumOptions, value);
        }

        public Stratum? Stratum
        {
            get => _stratum;
            set
            {
                if (_stratum == value) { return; }
                SetProperty(ref _stratum, value);
                BigBAFSampleGroup = null;
                UseBigBAF = false;

                if (value != null)
                {
                    var isVariableRadious = CruiseDAL.Schema.CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(value.Method);

                    // BAF and FPS should be whole numbers but for some reason they are REALs in the data structure
                    // note that FPS value is the denominator in fractions of acres
                    if (isVariableRadious)
                    {
                        BAF = (decimal)value.BasalAreaFactor;
                        FPS = 0.0m;

                        BigBAFSampleGroupOptions = SampleGroupDataservice.GetSampleGroups(value.StratumCode)
                            .Where(x => x.BigBAF > 0)
                            .ToArray();
                    }
                    else
                    {
                        FPS = (decimal)value.FixedPlotSize;
                        BAF = 0.0m;
                    }
                    OnPropertyChanged(nameof(IsVariableRadius));
                    OnPropertyChanged(nameof(StratumSettingsSummary));
                    Calculate();
                }
                else
                {
                    BigBAFSampleGroupOptions = null;
                }
            }
        }

        public IEnumerable<SampleGroup>? BigBAFSampleGroupOptions
        {
            get => _bigBAFSampleGroupOptions;
            set => SetProperty(ref _bigBAFSampleGroupOptions, value);
        }

        public SampleGroup? BigBAFSampleGroup
        {
            get => _bigBAFSampleGroup;
            set
            {
                SetProperty(ref _bigBAFSampleGroup, value);
                OnPropertyChanged(nameof(StratumSettingsSummary));
                Calculate();
            }
        }

        public bool UseBigBAF
        {
            get => _useBigBAF;
            set
            {
                SetProperty(ref _useBigBAF, value);
                OnPropertyChanged(nameof(StratumSettingsSummary));
                Calculate();
            }
        }

        public Plot? Plot
        {
            get => _plot;
            set
            {
                SetProperty(ref _plot, value);
            }
        }

        public decimal BAF
        {
            get
            {
                if (Mode == MODE_STRATUM && Stratum != null)
                {
                    return (UseBigBAF && BigBAFSampleGroup != null) ? (decimal)BigBAFSampleGroup.BigBAF : (decimal)Stratum.BasalAreaFactor;
                }
                return _baf;
            }
            set
            {
                SetProperty(ref _baf, value);
                OnPropertyChanged(nameof(StratumSettingsSummary));
                Calculate();
            }
        }

        public decimal FPS
        {
            get => _fps;
            set
            {
                SetProperty(ref _fps, value);
                OnPropertyChanged(nameof(StratumSettingsSummary));
                Calculate();
            }
        }

        public string? Mode
        {
            get => _stratumMode;
            set
            {
                SetProperty(ref _stratumMode, value);
                OnPropertyChanged(nameof(IsVariableRadius));
                OnPropertyChanged(nameof(StratumSettingsSummary));
                Calculate();
            }
        }

        public bool IsVariableRadius
        {
            get
            {
                var stMode = Mode;
                if (stMode == MODE_BAF) { return true; }
                if (stMode == MODE_FPS) { return false; }
                if (stMode == MODE_STRATUM && Stratum != null)
                {
                    return CruiseDAL.Schema.CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Stratum.Method);
                }
                return false;
            }
        }

        public string? StratumSettingsSummary
        {
            get
            {
                var mode = Mode;
                if (mode == MODE_BAF)
                { return "BAF: " + BAF; }
                if (mode == MODE_FPS)
                { return "FPS: " + FPS; }

                var st = Stratum;
                if (mode == MODE_STRATUM && st != null)
                {
                    var stuff = (IsVariableRadius) ? "BAF: " + BAF : "FPS: " + FPS;
                    return "Stratum: " + st.StratumCode + " " + stuff;
                }
                return "";
            }
        }

        #endregion stratum settings

        public decimal DBH
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

        // HACK xamarin really doesn't like binding to nullable types
        // so instead we will bind the text box to a property that exposes SlopeDistance as a string.
        // see: https://forums.xamarin.com/discussion/144704/binding-to-nullable-int
        public string? SlopeDistanceStr
        {
            get => SlopeDistance?.ToString() ?? "";
            set
            {
                if (value != null && decimal.TryParse(value, out var d))
                {
                    SlopeDistance = d;
                }
                else
                {
                    SlopeDistance = null;
                }
            }
        }

        public decimal? SlopeDistance
        {
            get { return _slopeDistance; }
            set
            {
                _slopeDistance = value;
                Calculate();
            }
        }

        public bool IsToFace
        {
            get { return _isToFace; }
            set
            {
                SetProperty(ref _isToFace, value);
                Calculate();
                OnPropertyChanged(nameof(MeasureToSelection));
            }
        }

        public IEnumerable<string> MeasureToOptions => MEASURE_TO_OPTIONS;

        public string? MeasureToSelection
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

        public decimal LimitingDistance
        {
            get { return _limitingDistance; }
            set { SetProperty(ref _limitingDistance, value); }
        }

        #region result properties

        public bool? IsTreeIn
        {
            get { return _isTreeIn; }
            set
            {
                SetProperty(ref _isTreeIn, value);
                OnPropertyChanged(nameof(TreeStatus));
            }
        }

        public string? TreeStatus
        {
            get
            {
                if (IsTreeIn.HasValue == false) { return ""; }
                else if (IsTreeIn == true) { return TREE_STATUS_IN; }
                else { return TREE_STATUS_OUT; }
            }
        }

        #endregion result properties

        #region report properties

        /// <summary>
        /// Azimuth is recorded for the purpose of helping check cruisers
        /// identify which tree was LDed
        /// </summary>
        public decimal Azimuth
        {
            get { return _azimuth; }
            set
            {
                _azimuth = value;
            }
        }

        public string? TreeNumber
        {
            get => _treeNumber;
            set => SetProperty(ref _treeNumber, value);
        }

        #endregion report properties

        #region commands

        public ICommand SaveReportToPlotCommand => _saveReportToPlotCommand ??= new Command(SaveReport);

        public ICommand CopyReportToClipboardCommand => _copyReportToClipboardCommand ??= new Command(x => CopyReportToClipBoard().FireAndForget());

        #endregion commands

        public LimitingDistanceViewModel(ISampleGroupDataservice sampleGroupDataservice, IPlotDataservice plotDataService, IStratumDataservice stratumDataservice, INatCruiseDialogService dialogService, IApplicationSettingService appSettings)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            UseNewLimitingDistanceCalculator = appSettings.UseNewLimitingDistanceCalculator;

            SampleGroupDataservice = sampleGroupDataservice;
            PlotDataservice = plotDataService;
            StratumDataservice = stratumDataservice;

            Clipboard = Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default;

            if (UseNewLimitingDistanceCalculator)
            { LimitingDistanceCalculator = new CalculateLimitingDistance2(); }
            else
            { LimitingDistanceCalculator = new CalculateLimitingDistance(); }
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

        protected override void Load(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var plotNumber = parameters.GetValue<string>(NavParams.PLOT_NUMBER);

            if (unitCode != null)
            {
                var stDs = StratumDataservice;

                var strata = stDs.GetStrata(unitCode);
                StratumOptions = strata;

                if (stratumCode != null)
                {
                    var ds = StratumDataservice;
                    _stratumMode = MODE_STRATUM;
                    Stratum = ds.GetStratum(stratumCode);
                }

                if (plotNumber != null
                && int.TryParse(plotNumber, out var iplotNumber))
                {
                    var pDs = PlotDataservice;
                    var plot = Plot = pDs.GetPlot(unitCode, iplotNumber);
                }
                else
                { Plot = null; }
            }
            else // if no unit selected load all strata
            {
                var stDs = StratumDataservice;
                if (stDs != null)
                {
                    var strata = stDs.GetStrata();
                    StratumOptions = strata;
                }
            }
        }

        public bool CanCalculate()
        {
            if (DBH <= 0.0m) { return false; }

            if (IsVariableRadius)
            {
                return BAF > 0.0m;
            }
            else
            {
                return FPS > 0.0m;
            }
        }

        public void Calculate()
        {
            if (!CanCalculate())
            {
                IsTreeIn = null;
                LimitingDistance = 0.0m;
                return;
            }

            var limitingDistance = LimitingDistance = LimitingDistanceCalculator.Calculate(BAF, FPS, DBH, SlopePCT, IsVariableRadius, IsToFace);
            var slopeDistance = SlopeDistance;

            if (slopeDistance.HasValue)
            {
                IsTreeIn = LimitingDistanceCalculator.DeterminTreeInOrOut(slopeDistance.Value, limitingDistance);
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
                var treeNumber = (IsTreeIn.Value) ? TreeNumber : "";

                return LimitingDistanceCalculator.GenerateReport(TreeStatus, LimitingDistance, SlopeDistance.Value,
                    SlopePCT, Azimuth, BAF, FPS, DBH, IsVariableRadius, IsToFace, Stratum?.StratumCode, treeNumber);
            }
            else { return null; }
        }

        public void SaveReport()
        {
            var plot = Plot;
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

        public async Task CopyReportToClipBoard()
        {
            var report = GenerateReport();
            if (!string.IsNullOrEmpty(report))
            {
                await Clipboard.SetTextAsync(report);
                DialogService.ShowNotification(report, "Report Copied");
            }
        }
    }
}