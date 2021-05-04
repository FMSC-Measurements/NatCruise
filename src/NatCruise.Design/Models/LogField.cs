using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("LogField")]
    public class LogField
    {
        public string Field { get; set; }

        public string Heading { get; set; }

        public string DefaultHeading { get; set; }

        public string DbType { get; set; }
    }
}