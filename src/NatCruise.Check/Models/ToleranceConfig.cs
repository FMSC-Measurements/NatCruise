using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Models
{
    public class ToleranceConfig : ICloneable
    {
        public ToleranceVolumeOptions VolumeConfig { get; set; }

        public IList<TolerancesList> Tolerances { get; set; }

        public double MinElementAccuracy { get; set; }
        public double MinOverallAccuracy { get; set; }

        public string ConfigVersion { get; set; }

        public object Clone()
        {
            var clone = new ToleranceConfig()
            {
                MinElementAccuracy = MinElementAccuracy,
                MinOverallAccuracy = MinOverallAccuracy,
                ConfigVersion = ConfigVersion,
                VolumeConfig = (ToleranceVolumeOptions)VolumeConfig.Clone(),
                Tolerances = Tolerances.Select(t => (TolerancesList)t.Clone()).ToList(),
            };

            return clone;
        }
    }
}
