using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace NatCruise.Cruise.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit : Model_Base
    {
        public string CuttingUnitCode { get; set; }

        public string Description { get; set; }

        public string Area { get; set; }

        public override string ToString()
        {
            return $"{CuttingUnitCode}: {Description} Area: {Area}";
        }
    }
}