using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("TreeAuditRuleSelector")]
    public class TreeAuditRuleSelector
    {
        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public string PrimaryProduct { get; set; }

        public string TreeAuditRuleID { get; set; }
    }
}