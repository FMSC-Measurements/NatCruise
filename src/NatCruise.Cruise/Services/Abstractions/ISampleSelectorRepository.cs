using FMSC.Sampling;

namespace NatCruise.Cruise.Services
{
    public interface ISampleSelectorDataService
    {
        ISampleSelector GetSamplerBySampleGroupCode(string stratumCode, string sgCode);

        void SaveSamplerStates();
    }
}
