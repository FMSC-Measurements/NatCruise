using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface ICruisersDataservice : IDataservice
    {
        bool PromptCruiserOnSample { get; set; }

        IEnumerable<string> GetCruisers();

        void AddCruiser(string cruiser);

        void RemoveCruiser(string cruiser);
    }
}