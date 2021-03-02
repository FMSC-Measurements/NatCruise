using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    [Table("Plot_Stratum")]
    public class Plot_Stratum : Model_Base
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
            set => SetValue(ref _inCruise, value);
        }

        //[Field("PlotID")]
        //public string PlotID { get; set; }

        public int PlotNumber
        {
            get => _plotNumber;
            set => SetValue(ref _plotNumber, value);
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
            set => SetValue(ref _isEmpty, value);
        }

        public int KPI
        {
            get => _kpi;
            set => SetValue(ref _kpi, value);
        }

        [Field("KZ3PPNT", PersistanceFlags = PersistanceFlags.Never)]
        public int KZ3PPNT { get; set; }

        public int ThreePRandomValue { get; set; }
    }
}