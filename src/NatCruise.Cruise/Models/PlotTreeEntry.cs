using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("Tree")]
    public class PlotTreeEntry : IHasTreeID
    {
        public string TreeID { get; set; }

        public int TreeNumber { get; set; }

        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public int PlotNumber { get; set; }

        public string CountOrMeasure { get; set; }

        public string Method { get; set; }

        public int TreeCount { get; set; }

        public bool STM { get; set; }

        public int KPI { get; set; }

        public int ThreePRandomValue { get; set; }

        public string Initials { get; set; }

        [IgnoreField]
        public int Errors { get; set; }

        [IgnoreField]
        public int Warnings { get; set; }
    }
}