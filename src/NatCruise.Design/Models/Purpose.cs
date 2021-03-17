using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("LK_Purpose")]
    public class Purpose
    {
        [Field("Purpose")]
        public string PurposeCode { get; set; }

        public string ShortCode { get; set; }

        public override string ToString()
        {
            return $"{PurposeCode} - {ShortCode}";
        }
    }
}