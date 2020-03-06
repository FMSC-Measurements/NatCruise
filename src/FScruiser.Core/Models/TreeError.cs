using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;

namespace FScruiser.Models
{
    public class TreeError : Error_Base, IHasTreeID
    {
        [Field("TreeID")]
        public string TreeID { get; set; }

        [Field("TreeAuditRuleID")]
        public string TreeAuditRuleID { get; set; }
    }
}