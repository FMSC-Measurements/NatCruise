using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Services
{
    public interface IApplicationSettingService
    {
        bool EnableCrashReports { get; set; }

        bool EnableAnalitics { get; set; }

        bool UseNewLimitingDistanceCalculator { get; set; }

        bool SelectPrevNextTreeSkipsCountTrees { get; set; }

        void Save();
    }
}
