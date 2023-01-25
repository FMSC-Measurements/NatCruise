using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ICruisersDataservice
    {
        bool PromptCruiserOnSample { get; set; }

        IEnumerable<string> GetCruisers();

        void AddCruiser(string cruiser);

        void RemoveCruiser(string cruiser);
    }
}