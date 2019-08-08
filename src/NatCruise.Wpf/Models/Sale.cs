using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Wpf.Models
{
    public class Sale : BindableBase
    {
        private string _region;

        [PrimaryKeyField("Sale_CN")]
        public int Sale_CN { get; set; }

        public string SaleNumber { get; set; }

        public string Name { get; set; }

        public string Purpose { get; set; }

        public string Region
        {
            get => _region;
            set => SetProperty(ref _region, value);
        }

        public string Forest { get; set; }

        public string District { get; set; }

        public string MeasurementYear { get; set; }

        public int CalendarYear { get; set; }

        public string Remarks { get; set; }

        public string DefaultUOM { get; set; }
    }
}