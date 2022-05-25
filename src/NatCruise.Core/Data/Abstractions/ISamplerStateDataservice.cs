using System.Collections.Generic;
using NatCruise.Models;

namespace NatCruise.Data
{
    public interface ISamplerStateDataservice : IDataservice
    {
        void CopySamplerStates(string deviceFrom, string deviceTo);
        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode, string deviceID);


        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode);
        void UpsertSamplerState(SamplerState samplerState);

        bool HasSampleStateEnvy();
        bool HasSampleStates();
    }
}