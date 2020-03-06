using System.Collections.Generic;

namespace NatCruise.Design.Models
{
    public class Region
    {
        public string RegionCode { get; set; }

        public string FriendlyName { get; set; }

        public IEnumerable<Forest> Forests { get; set; }

        public override string ToString()
        {
            return $"{RegionCode} - {FriendlyName}";
        }
    }
}