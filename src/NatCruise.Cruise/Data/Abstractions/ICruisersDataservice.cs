using NatCruise.Data;
using System.Collections.Generic;

namespace NatCruise.Cruise.Services
{
    public interface ICruisersDataservice : IDataservice
    {
        bool PromptCruiserOnSample { get; set; }

        IEnumerable<string> GetCruisers();

        void AddCruiser(string cruiser);

        void RemoveCruiser(string cruiser);

        //void UpdateCruiser(string oldValue, string newValue);
    }
}