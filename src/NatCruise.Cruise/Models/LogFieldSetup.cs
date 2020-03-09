using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("LogFieldSetup_V3")]
    public class LogFieldSetup
    {
        [Field("Field")]
        public string Field { get; set; }

        //public int FieldOrder { get; set; }
        [Field("Heading")]
        public string Heading { get; set; }
    }
}