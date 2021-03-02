using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    [Table("LogFieldSetup")]
    public class LogFieldSetup
    {
        public string Field { get; set; }

        public string Heading { get; set; }
    }
}