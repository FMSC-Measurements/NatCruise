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

        void Save();
    }
}
