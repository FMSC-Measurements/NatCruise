using CommunityToolkit.Mvvm.ComponentModel;
using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Plot")]
    public class Plot : ObservableObject
    {
        private double _slope;
        private double _aspect;
        private string _remarks;
        private int _plotNumber;
        private string _cuttingUnitCode;

        public string PlotID { get; set; }
        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set => SetProperty(ref _cuttingUnitCode, value);
        }

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

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeCount { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int ErrorCount { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeErrorCount { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeWarningCount { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string NullStrata { get; set; }

        public override string ToString()
        {
            return $"{PlotNumber}";
        }
    }
}