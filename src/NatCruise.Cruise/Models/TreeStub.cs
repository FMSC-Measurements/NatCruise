using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("Tree")]
    public class TreeStub : IHasTreeID
    {
        public string TreeID { get; set; }

        public int TreeNumber { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public string PlotNumber { get; set; }

        [Field(Alias = "Height", SQLExpression = "max(TreeMeasurment.TotalHeight, TreeMeasurment.MerchHeightPrimary, TreeMeasurment.UpperStemHeight)")]
        public int Height { get; set; }

        [Field(Alias = "Diameter", SQLExpression = "max(TreeMeasurment.DBH, TreeMeasurment.DRC, TreeMeasurment.DBHDoubleBarkThickness)")]
        public int Diameter { get; set; }

        public string CountOrMeasure { get; set; }

        [IgnoreField]
        public int Errors { get; set; }

        [IgnoreField]
        public int Warnings { get; set; }

        public int ErrorCount { get; set; }

        public int WarningCount { get; set; }
    }
}