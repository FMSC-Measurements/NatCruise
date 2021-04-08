using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("Tree")]
    public class TreeStub_Plot : TreeStub, IHasTreeID
    {
        public string CuttingUnitCode { get; set; }
        public int PlotNumber { get; set; }
        public string LiveDead { get; set; }
        public int TreeCount { get; set; }
        public bool STM { get; set; }
        public int KPI { get; set; }
        public int ThreePRandomValue { get; set; }
        public string Initials { get; set; }
    }
}