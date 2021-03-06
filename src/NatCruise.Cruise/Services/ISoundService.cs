﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ISoundService : IDisposable
    {
        Task SignalMeasureTreeAsync();

        Task SignalInsuranceTreeAsync();

        Task SignalTallyAsync(bool force = false);

        Task SignalInvalidActionAsync();
    }
}
