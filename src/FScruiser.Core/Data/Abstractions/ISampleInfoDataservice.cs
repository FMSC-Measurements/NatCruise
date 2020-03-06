using System.Collections.Generic;
using FScruiser.Models;

namespace FScruiser.Data
{
    public interface ISampleInfoDataservice
    {
        Device CurrentDevice { get; }

        void CopySamplerStates(string deviceFrom, string deviceTo);
        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode, string deviceID);
        //Device GetCurrentDevice();
        IEnumerable<Device> GetDevices();
        IEnumerable<Device> GetOtherDevices();
        SamplerInfo GetSamplerInfo(string stratumCode, string sampleGroupCode);
        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode);
        void UpsertSamplerState(SamplerState samplerState);

        bool HasSampleStateEnvy();
        bool HasSampleStates();
    }
}