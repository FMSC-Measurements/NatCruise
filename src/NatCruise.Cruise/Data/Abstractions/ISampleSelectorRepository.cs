using FMSC.Sampling;
using NatCruise.Data;

namespace NatCruise.Cruise.Services
{
    public interface ISampleSelectorDataService : IDataservice
    {
        ISampleSelector GetSamplerBySampleGroupCode(string stratumCode, string sgCode);

        //void SaveSampler(ISampleSelector sampler);

        //void SaveSamplerStates();
    }
}
