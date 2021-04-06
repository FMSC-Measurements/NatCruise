using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Sale")]
    public class Sale : BindableBase
    {
        private string _region;
        private string _saleNumber;
        private string _name;
        private string _forest;
        private string _district;
        private int _calendarYear;
        private string _remarks;
        private string _defaultUOM;
        private string _saleID;

        [PrimaryKeyField("Sale_CN")]
        public int Sale_CN { get; set; }

        public string SaleID
        {
            get => _saleID;
            set => SetProperty(ref _saleID, value);
        }

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Region
        {
            get => _region;
            set => SetProperty(ref _region, value);
        }

        public string Forest
        {
            get => _forest;
            set => SetProperty(ref _forest, value);
        }

        public string District
        {
            get => _district;
            set => SetProperty(ref _district, value);
        }

        public int CalendarYear
        {
            get => _calendarYear;
            set => SetProperty(ref _calendarYear, value);
        }

        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public string DefaultUOM
        {
            get => _defaultUOM;
            set => SetProperty(ref _defaultUOM, value);
        }

        public override string ToString()
        {
            return $"{SaleNumber} {Name}";
        }
    }
}