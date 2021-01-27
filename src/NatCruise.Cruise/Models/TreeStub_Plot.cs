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
        public string STM { get; set; }
        public int KPI { get; set; }
        public string Initials { get; set; }

        [IgnoreField]
        public bool IsSTM
        {
            get { return STM == "Y"; }
            set { STM = (value) ? "Y" : "N"; }
        }
    }
}