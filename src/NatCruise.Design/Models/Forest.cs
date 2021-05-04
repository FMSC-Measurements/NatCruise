using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Design.Models
{
    [Table("LK_Forest")]
    public class Forest
    {
        [Field("Region")]
        public string RegionCode { get; set; }

        [Field("Forest")]
        public string ForestCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{ForestCode} - {FriendlyName}";
        }
    }
}