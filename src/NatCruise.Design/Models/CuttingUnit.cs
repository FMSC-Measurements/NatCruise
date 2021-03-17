using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit : BindableBase
    {
        private string _cuttingUnitCode;
        private double _area;
        private string _description;
        private string _loggingMethod;
        private string _paymentUnit;
        private string _rx;


        public int CuttingUnit_CN { get; set; }

        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set => SetProperty(ref _cuttingUnitCode, value);
        }

        public double Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string LoggingMethod
        {
            get => _loggingMethod;
            set => SetProperty(ref _loggingMethod, value);
        }

        public string PaymentUnit
        {
            get => _paymentUnit;
            set => SetProperty(ref _paymentUnit, value);
        }

        public string Rx
        {
            get => _rx;
            set => SetProperty(ref _rx, value);
        }
    }
}