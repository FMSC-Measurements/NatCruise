using FMSC.Sampling;

namespace FScruiser.Services
{
    public interface ISampleSelectorDataService
    {
        ISampleSelector GetSamplerBySampleGroupCode(string stratumCode, string sgCode);

        void SaveSamplerStates();
    }
}
