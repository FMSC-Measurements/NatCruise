using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;

namespace FScruiser.Models
{
    [Table("Tree_V3")]
    public class TreeStub : IHasTreeID
    {
        [Field("TreeID")]
        public string TreeID { get; set; }

        [Field("TreeNumber")]
        public int TreeNumber { get; set; }

        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("Species")]
        public string Species { get; set; }

        [Field(Alias = "Height", SQLExpression = "max(TreeMeasurment.TotalHeight, TreeMeasurment.MerchHeightPrimary, TreeMeasurment.UpperStemHeight)")]
        public int Height { get; set; }

        [Field(Alias = "Diameter", SQLExpression = "max(TreeMeasurment.DBH, TreeMeasurment.DRC, TreeMeasurment.DBHDoubleBarkThickness)")]
        public int Diameter { get; set; }

        [Field(Name = "CountOrMeasure")]
        public string CountOrMeasure { get; set; }

        [IgnoreField]
        public int Errors { get; set; }
    }
}