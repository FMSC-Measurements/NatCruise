using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;

namespace FScruiser.Models
{
    [Table("Sale")]
    public class Sale : INPC_Base
    {
        private string _remarks;

        public Sale()
        {
        }

        public string Name { get; set; }

        public int SaleNumber { get; set; }

        public string Purpose { get; set; }

        public string Region { get; set; }

        public string Forest { get; set; }

        public string District { get; set; }

        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }
    }
}