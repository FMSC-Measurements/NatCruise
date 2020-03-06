using FMSC.ORM.EntityModel.Attributes;
using NatCruise.Models;

namespace FScruiser.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit : Model_Base
    {
        [Field("Code")]
        public string Code { get; set; }

        [Field("Description")]
        public string Description { get; set; }

        [Field("Area")]
        public string Area { get; set; }

        public override string ToString()
        {
            return $"{Code}: {Description} Area: {Area}";
        }
    }
}