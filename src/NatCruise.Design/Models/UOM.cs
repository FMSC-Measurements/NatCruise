using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("LK_UOM")]
    public class UOM
    {
        [Field("UOM")]
        public string UOMCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{UOMCode} - {FriendlyName}";
        }
    }
}