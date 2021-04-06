using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("TreeFieldSetupDefault")]
    public class TreeFieldSetupDefault
    {
        public string StratumDefaultID { get; set; }

        public string SampleGroupDefaultID { get; set; }

        public string Field { get; set; }

        public int? FieldOrder { get; set; }

        public bool? IsHidden { get; set; }

        public bool? IsLocked { get; set; }

        public int? DefaultValueInt { get; set; }

        public double? DefaultValueReal { get; set; }

        public bool? DefaultValueBool { get; set; }

        public string DefaultValueText { get; set; }
    }
}