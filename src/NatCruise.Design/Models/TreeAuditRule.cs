using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("TreeAuditRule")]
    public class TreeAuditRule
    {
        public string TreeAuditRuleID { get; set; }

        public string Field { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public string Description { get; set; }
    }
}