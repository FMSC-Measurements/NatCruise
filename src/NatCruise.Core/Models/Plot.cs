using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Plot")]
    public class Plot : BindableBase
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
            set => SetProperty(ref _plotNumber, value);
        }

        public double Slope
        {
            get => _slope;
            set => SetProperty(ref _slope, value);
        }

        public double Aspect
        {
            get => _aspect;
            set => SetProperty(ref _aspect, value);
        }

        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public override string ToString()
        {
            return $"{PlotNumber}";
        }
    }
}