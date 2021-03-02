using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("Sale")]
    public class Sale : Model_Base
    {
        private string _remarks;

        public Sale()
        {
        }

        public string SaleID { get; set; }

        public string Name { get; set; }

        public int SaleNumber { get; set; }

        //public string Purpose { get; set; }

        public string Region { get; set; }

        public string Forest { get; set; }

        public string District { get; set; }

        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }

        public override string ToString()
        {
            return $"{SaleNumber} {Name}";
        }
    }
}