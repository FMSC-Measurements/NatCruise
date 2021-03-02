using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    [Table("Plot")]
    public class Plot : Model_Base
    {
        private double _slope;
        private double _aspect;
        private string _remarks;
        private int _plotNumber;

        public string PlotID { get; set; }
        public string CuttingUnitCode { get; set; }

        public int PlotNumber
        {
            get => _plotNumber;
            set => SetValue(ref _plotNumber, value);
        }

        public double Slope
        {
            get => _slope;
            set => SetValue(ref _slope, value);
        }

        public double Aspect
        {
            get => _aspect;
            set => SetValue(ref _aspect, value);
        }

        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }

        //public int CuttingUnit_CN { get; set; }
    }
}