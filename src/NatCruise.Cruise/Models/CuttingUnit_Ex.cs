using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class CuttingUnit_Ex : CuttingUnit
    {
        [IgnoreField]
        public bool HasPlotStrata { get; set; }

        [IgnoreField]
        public bool HasTreeStrata { get; set; }
    }
}