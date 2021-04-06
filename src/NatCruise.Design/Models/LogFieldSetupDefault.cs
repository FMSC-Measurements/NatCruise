using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("LogFieldSetupDefault")]
    public class LogFieldSetupDefault
    {
        public string StratumDefaultID { get; set; }

        public string Field { get; set; }

        public int? FieldOrder { get; set; }
    }
}