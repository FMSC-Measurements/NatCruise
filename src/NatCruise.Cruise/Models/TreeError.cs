using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Cruise.Models
{
    public class TreeError : Error_Base, IHasTreeID
    {
        public string TreeID { get; set; }

        public string TreeAuditRuleID { get; set; }
    }
}