using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Wpf.Models
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

        [PrimaryKeyField("CuttingUnit_CN")]
        public int CuttingUnit_CN { get; set; }

        [Field("Code")]
        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set => SetProperty(ref _cuttingUnitCode, value);
        }

        [Field("Area")]
        public double Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        [Field("Description")]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        [Field("LoggingMethod")]
        public string LoggingMethod
        {
            get => _loggingMethod;
            set => SetProperty(ref _loggingMethod, value);
        }

        [Field("PaymentUnit")]
        public string PaymentUnit
        {
            get => _paymentUnit;
            set => SetProperty(ref _paymentUnit, value);
        }

        [Field("Rx")]
        public string Rx
        {
            get => _rx;
            set => SetProperty(ref _rx, value);
        }
    }
}