using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Cruise.Services
{
    public interface ITallySettingsDataService : INotifyPropertyChanged
    {
        bool EnableCruiserPopup { get; set; }

        bool EnableAskEnterTreeData { get; set; }
        IEnumerable<string> Cruisers { get; }

        double TallyButtonTrayVerticalSize { get; set; }

        double PlotTallyButtonTrayVerticalSize { get; set; }
    }
}