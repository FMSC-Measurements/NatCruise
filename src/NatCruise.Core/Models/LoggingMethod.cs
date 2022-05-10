using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_LoggingMethod")]
    public class LoggingMethod
    {
        [Field("LoggingMethod")]
        public string LoggingMethodCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{LoggingMethodCode} - {FriendlyName}";
        }
    }
}