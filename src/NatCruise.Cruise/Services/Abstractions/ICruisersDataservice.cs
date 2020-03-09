using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public interface ICruisersDataservice
    {
        bool PromptCruiserOnSample { get; set; }

        IEnumerable<string> GetCruisers();

        void AddCruiser(string cruiser);

        void RemoveCruiser(string cruiser);

        //void UpdateCruiser(string oldValue, string newValue);
    }
}
