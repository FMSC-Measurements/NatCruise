using System.Collections.Generic;
using NatCruise.Cruise.Models;
using NatCruise.Data;

namespace NatCruise.Cruise.Data
{
    public interface ISampleInfoDataservice : IDataservice
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