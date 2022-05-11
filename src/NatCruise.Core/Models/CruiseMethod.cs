using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_CruiseMethod")]
    public class CruiseMethod
    {
        public string Method { get; set; }

        public string FriendlyName { get; set; }

        public bool IsPlotMethod { get; set; }

        public override string ToString()
        {
            return Method;
        }
    }
}