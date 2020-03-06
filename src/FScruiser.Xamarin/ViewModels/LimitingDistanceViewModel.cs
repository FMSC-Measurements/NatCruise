using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.XF.ViewModels
{
    public class LimitingDistanceViewModel : ViewModelBase
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

        public ICuttingUnitDatastore Datastore { get; }

        public Plot_Stratum Plot { get; set; }

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
                if(value != null && double.TryParse(value, out var d))
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
                SetValue(ref _isToFace, value);
                Calculate();
                RaisePropertyChanged(nameof(MeasureToSelection));
            }
        }

        public bool IsVariableRadius
        {
            get { return _isVariableRadius; }
            set
            {
                SetValue(ref _isVariableRadius, value);
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
            set { SetValue(ref _limitingDistance, value); }
        }

        public bool? IsTreeIn
        {
            get { return _isTreeIn; }
            set
            {
                SetValue(ref _isTreeIn, value);
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

        public LimitingDistanceViewModel(IDataserviceProvider datastoreProvider)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            if(TreeStatus != null)
            {
                SaveReport();
            }
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unitCode = parameters.GetValue<string>("UnitCode");
            var stratumCode = parameters.GetValue<string>("StratumCode");
            var plotNumber = parameters.GetValue<int>("PlotNumber");

            var plot = Datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);

            if (plot != null)
            {
                var isVariableRadious = IsVariableRadius = CruiseDAL.Schema.CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(plot.CruiseMethod);

                BafOrFps = (isVariableRadious) ? plot.BAF : plot.FPS;

                Plot = plot;

                RaisePropertyChanged(nameof(BafOrFps));
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

            var limitingDistance = LimitingDistance = Logic.CalculateLimitingDistance.Calculate(BafOrFps, DBH, SlopePCT, IsVariableRadius, IsToFace);
            var slopeDistance = SlopeDistance;

            if (slopeDistance.HasValue)
            {
                IsTreeIn = Logic.CalculateLimitingDistance.DeterminTreeInOrOut(slopeDistance.Value, limitingDistance);
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
                return Logic.CalculateLimitingDistance.GenerateReport(TreeStatus, LimitingDistance, SlopeDistance.Value,
                    SlopePCT, Azimuth, BafOrFps, DBH, SlopePCT, IsVariableRadius, IsToFace, Plot.StratumCode);
            }
            else { return null; }
        }

        public void SaveReport()
        {
            var plot = Plot;
            var report = GenerateReport();
            if (!string.IsNullOrEmpty(report))
            {
                Datastore.AddPlotRemark(plot.CuttingUnitCode, plot.PlotNumber, report);
            }
        }
    }
}