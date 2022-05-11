using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Plot_Stratum")]
    public class Plot_Stratum : BindableBase
    {
        private int _plotNumber;
        private bool _inCruise;
        private bool _isEmpty;
        private int _kpi;

        public long? Plot_Stratum_CN { get; set; }

        [Field("InCruise", PersistanceFlags = PersistanceFlags.Never)]
        public bool InCruise
        {
            get => _inCruise;
            set => SetProperty(ref _inCruise, value);
        }

        //[Field("PlotID")]
        //public string PlotID { get; set; }

        public int PlotNumber
        {
            get => _plotNumber;
            set => SetProperty(ref _plotNumber, value);
        }

        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        [Field("CruiseMethod", PersistanceFlags = PersistanceFlags.Never)]
        public string CruiseMethod { get; set; }

        [Field("BAF", PersistanceFlags = PersistanceFlags.Never)]
        public double BAF { get; set; }

        [Field("FPS", PersistanceFlags = PersistanceFlags.Never)]
        public double FPS { get; set; }

        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }

        public int KPI
        {
            get => _kpi;
            set => SetProperty(ref _kpi, value);
        }

        public int TreeCount { get; set; }

        public double AverageHeight { get; set; }

        public string CountOrMeasure { get; set; }

        [Field("KZ3PPNT", PersistanceFlags = PersistanceFlags.Never)]
        public int KZ3PPNT { get; set; }

        public int ThreePRandomValue { get; set; }
    }
}