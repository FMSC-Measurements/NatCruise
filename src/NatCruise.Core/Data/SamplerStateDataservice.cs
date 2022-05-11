using CruiseDAL;
using NatCruise.Models;
using System.Linq;

namespace NatCruise.Data
{
    public class SamplerStateDataservice : CruiseDataserviceBase, ISamplerStateDataservice
    {
        

        public SamplerStateDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public SamplerStateDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SamplerState GetSamplerState(string stratumCode, string sampleGroupCode, string deviceID)
        {
            if (string.IsNullOrEmpty(stratumCode)) { throw new System.ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty", nameof(stratumCode)); }
            if (string.IsNullOrEmpty(sampleGroupCode)) { throw new System.ArgumentException($"'{nameof(sampleGroupCode)}' cannot be null or empty", nameof(sampleGroupCode)); }
            if (string.IsNullOrEmpty(deviceID)) { throw new System.ArgumentException($"'{nameof(deviceID)}' cannot be null or empty", nameof(deviceID)); }

            return Database.Query<SamplerState>(
@"SELECT
    ss.*
FROM SamplerState AS ss
WHERE ss.StratumCode = @p1
    AND ss.SampleGroupCode = @p2
    AND ss.DeviceID = @p3
    AND ss.CruiseID =  @p4;", stratumCode, sampleGroupCode, deviceID, CruiseID).FirstOrDefault();
        }

        public SamplerState GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            if (string.IsNullOrEmpty(stratumCode)) { throw new System.ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty", nameof(stratumCode)); }
            if (string.IsNullOrEmpty(sampleGroupCode)) { throw new System.ArgumentException($"'{nameof(sampleGroupCode)}' cannot be null or empty", nameof(sampleGroupCode)); }

            var deviceID = DeviceID;

            return GetSamplerState(stratumCode, sampleGroupCode, deviceID);
        }

        public void UpsertSamplerState(SamplerState samplerState)
        {
            if (samplerState is null) { throw new System.ArgumentNullException(nameof(samplerState)); }

            var deviceID = DeviceID;

            Database.Execute2(
@"INSERT INTO SamplerState (
    CruiseID,
    DeviceID,
    StratumCode,
    SampleGroupCode,
    SampleSelectorType,
    BlockState,
    SystematicIndex,
    Counter,
    InsuranceIndex,
    InsuranceCounter
) VALUES (
    @CruiseID,
    @DeviceID,
    @StratumCode,
    @SampleGroupCode,
    @SampleSelectorType,
    @BlockState,
    @SystematicIndex,
    @Counter,
    @InsuranceIndex,
    @InsuranceCounter
)
ON CONFLICT (CruiseID, DeviceID, StratumCode, SampleGroupCode) DO
UPDATE SET
        BlockState = @BlockState,
        SystematicIndex = @SystematicIndex,
        Counter = @Counter,
        InsuranceIndex = @InsuranceIndex,
        InsuranceCounter = @InsuranceCounter
    WHERE CruiseID = @CruiseID AND DeviceID = @DeviceID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode;",
                new
                {
                    CruiseID,
                    DeviceID = deviceID,
                    samplerState.BlockState,
                    samplerState.Counter,
                    samplerState.InsuranceCounter,
                    samplerState.InsuranceIndex,
                    samplerState.SystematicIndex,
                    samplerState.SampleSelectorType,
                    samplerState.SampleGroupCode,
                    samplerState.StratumCode,
                }
            );
        }

        public void CopySamplerStates(string deviceFrom, string deviceTo)
        {
            if (string.IsNullOrEmpty(deviceFrom)) { throw new System.ArgumentException($"'{nameof(deviceFrom)}' cannot be null or empty", nameof(deviceFrom)); }
            if (string.IsNullOrEmpty(deviceTo)) { throw new System.ArgumentException($"'{nameof(deviceTo)}' cannot be null or empty", nameof(deviceTo)); }

            Database.Execute(
@"INSERT OR Replace INTO SamplerState (
    CruiseID,
    DeviceID,
    StratumCode,
    SampleGroupCode,
    SampleSelectorType,
    BlockState,
    SystematicIndex,
    Counter,
    InsuranceIndex,
    InsuranceCounter
) SELECT
    @p3,
    @p2,
    ss.StratumCode,
    ss.SampleGroupCode,
    ss.SampleSelectorType,
    ss.BlockState,
    ss.SystematicIndex,
    ss.Counter,
    ss.InsuranceIndex,
    ss.InsuranceCounter
    FROM SamplerState AS ss
    WHERE ss.DeviceID = @p1 AND CruiseID = @p3;", deviceFrom, deviceTo, CruiseID);
        }

        public bool HasSampleStates()
        {
            return HasSampleStates(DeviceID);
        }

        public bool HasSampleStates(string currentDeviceID)
        {
            var result = Database.ExecuteScalar<int>(
                @"SELECT count(*) FROM SamplerState WHERE DeviceID = @p1 AND CruiseID = @p2;", currentDeviceID, CruiseID);
            return result > 0;
        }

        public bool HasSampleStateEnvy()
        {
            return HasSampleStateEnvy(DeviceID);
        }

        public bool HasSampleStateEnvy(string currentDeviceID)
        {
            if (string.IsNullOrEmpty(currentDeviceID)) { throw new System.ArgumentException($"'{nameof(currentDeviceID)}' cannot be null or empty", nameof(currentDeviceID)); }

            var result = Database.ExecuteScalar<int>(
@"SELECT count(*)
FROM (
    SELECT StratumCode, SampleGroupCode FROM SamplerState
        WHERE DeviceID != @p1 AND CruiseID = @p2
    EXCEPT
    SELECT StratumCode, SampleGroupCode FROM SamplerState
        WHERE DeviceID = @p1 AND CruiseID = @p2
);", currentDeviceID, CruiseID);
            return result > 0;
        }
    }
}