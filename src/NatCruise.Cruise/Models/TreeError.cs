using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class TreeError : Error_Base, IHasTreeID
    {
        [Field("TreeID")]
        public string TreeID { get; set; }

        [Field("TreeAuditRuleID")]
        public string TreeAuditRuleID { get; set; }
    }
}