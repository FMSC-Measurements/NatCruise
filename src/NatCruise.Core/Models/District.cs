using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    [Table("LK_District")]
    public class District
    {
        [Field("District")]
        public string DistrictCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{DistrictCode} {FriendlyName}";
        }
    }
}