using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Models
{
    public class ToleranceConfig
    {
        public ToleranceVolumeOptions VolumeConfig { get; set; }

        public IList<TolerancesList> Tolerances { get; set; }

        public double MinElementAccuracy { get; set; }
        public double MinOverallAccuracy { get; set; }

        public string ConfigVersion { get; set; }
    }
}
