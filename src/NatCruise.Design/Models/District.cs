using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Models
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
