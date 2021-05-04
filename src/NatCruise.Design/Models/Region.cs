using FMSC.ORM.EntityModel.Attributes;
using System.Collections.Generic;

namespace NatCruise.Design.Models
{
    [Table("LK_Region")]
    public class Region
    {
        [Field("Region")]
        public string RegionCode { get; set; }

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            return $"{RegionCode} - {FriendlyName}";
        }
    }
}