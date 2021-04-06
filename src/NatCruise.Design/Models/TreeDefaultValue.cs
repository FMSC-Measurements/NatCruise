using FMSC.ORM.EntityModel.Attributes;
using System;

namespace NatCruise.Design.Models
{
    [Table("TreeDefaultValue")]
    public class TreeDefaultValue
    {
        public string SpeciesCode { get; set; }

        public string PrimaryProduct { get; set; }

        public double? CullPrimary { get; set; }

        public double? CullPrimaryDead { get; set; }

        public double? HiddenPrimary { get; set; }

        public double? HiddenPrimaryDead { get; set; }

        public string TreeGrade { get; set; }

        public string TreeGradeDead { get; set; }

        public double? CullSecondary { get; set; }

        public double? HiddenSecondary { get; set; }

        public double? Recoverable { get; set; }

        public int? MerchHeightLogLength { get; set; }

        public string MerchHeightType { get; set; }

        public double? FormClass { get; set; }

        public double? BarkThicknessRatio { get; set; }

        public double? AverageZ { get; set; }

        public double? ReferenceHeightPercent { get; set; }

        public string CreatedBy { get; set; }
    }
}