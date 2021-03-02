using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit : Model_Base
    {
        private string _description;
        private string _remarks;

        public string CuttingUnitCode { get; set; }

        public string Description
        {
            get => _description;
            set => SetValue(ref _description, value);
        }

        public string Area { get; set; }

        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }

        public override string ToString()
        {
            return $"{CuttingUnitCode}: {Description}";
        }
    }
}