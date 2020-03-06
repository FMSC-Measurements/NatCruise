using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ITallySettingsDataService : INotifyPropertyChanged
    {
        bool EnableCruiserPopup { get; set; }

        bool EnableAskEnterTreeData { get; set; }
        IEnumerable<string> Cruisers { get; }

        void AddCruiser(string cruiser);

        void RemoveCruiser(string cruiser);
    }
}
