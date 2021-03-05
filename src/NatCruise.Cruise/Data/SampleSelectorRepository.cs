using FMSC.Sampling;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Cruise.Services
{
    // TODO can this class be folded into ISampleInfoDataservie?
    public class SampleSelectorRepository : ISampleSelectorDataService
    {
        private Dictionary<string, ISampleSelector> _sampleSelectors = new Dictionary<string, ISampleSelector>();

        public SampleSelectorRepository(ISampleInfoDataservice dataservice)
        {
            Dataservice = dataservice ?? throw new ArgumentNullException(nameof(dataservice));
        }

        public ISampleInfoDataservice Dataservice { get; set; }

        public string DeviceID => Dataservice.DeviceID;

        public ISampleSelector GetSamplerBySampleGroupCode(string stratumCode, string sgCode)
        {
            if (string.IsNullOrEmpty(stratumCode)) { throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty", nameof(stratumCode)); }
            if (string.IsNullOrEmpty(sgCode)) { throw new ArgumentException($"'{nameof(sgCode)}' cannot be null or empty", nameof(sgCode)); }

            var key = stratumCode + "/" + sgCode;

            if (_sampleSelectors.ContainsKey(key) == false)
            {
                var samplerInfo = Dataservice.GetSamplerInfo(stratumCode, sgCode);

                var sampler = MakeSampleSelecter(samplerInfo);
                sampler.StratumCode = stratumCode;
                sampler.SampleGroupCode = sgCode;

                _sampleSelectors.Add(key, sampler);
            }

            return _sampleSelectors[key];
        }

        public ISampleSelector MakeSampleSelecter(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var method = samplerInfo.Method;

            //if ((sg.TallyMethod & CruiseDAL.Enums.TallyMode.Manual) == CruiseDAL.Enums.TallyMode.Manual)
            //{
            //    return null;
            //}

            switch (method)
            {
                case "100":
                case "FIX":
                case "PNT":
                case "FIXCNT":
                    {
                        return new HundredPCTSelector();
                    }

                case "STR":
                    {
                        return MakeBlockSampleSelector(samplerInfo);
                    }
                case "S3P":
                    {
                        return MakeS3PSampleSelector(samplerInfo);
                    }
                case "3P":
                case "P3P":
                case "F3P":
                    {
                        return MakeThreePSampleSelector(samplerInfo);
                    }
                case "FCM":
                case "PCM":
                    {
                        return MakeSystematicSampleSelector(samplerInfo);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public ISampleSelector MakeS3PSampleSelector(SamplerInfo samplerState)
        {
            if (samplerState is null) { throw new ArgumentNullException(nameof(samplerState)); }

            var state = Dataservice.GetSamplerState(samplerState.StratumCode, samplerState.SampleGroupCode);

            if (state != null)
            {
                return new S3PSelector(samplerState.SamplingFrequency, samplerState.KZ, state.Counter, state.BlockState);
            }
            else
            {
                return new S3PSelector(samplerState.SamplingFrequency, samplerState.KZ);
            }

        }

        public ISampleSelector MakeThreePSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            if (state != null)
            {
                return new ThreePSelecter(samplerInfo.KZ,
                    samplerInfo.InsuranceFrequency,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
            }
            else
            {
                return new ThreePSelecter(samplerInfo.KZ, samplerInfo.InsuranceFrequency);
            }


        }

        public ISampleSelector MakeSystematicSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            var freq = samplerInfo.SamplingFrequency;

            if (freq == 0) { return new ZeroFrequencySelecter(); }
            else
            {
                if (state != null)
                {
                    return new SystematicSelecter(freq,
                        samplerInfo.InsuranceFrequency,
                        state.Counter,
                        state.InsuranceIndex,
                        state.InsuranceCounter,
                        state.SystematicIndex);
                }
                else
                {
                    return new SystematicSelecter(freq, samplerInfo.InsuranceFrequency, true);
                }
            }
        }

        public ISampleSelector MakeBlockSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            var freq = samplerInfo.SamplingFrequency;

            if (freq == 0) { return null; }

            if (state == null)
            {
                return new BlockSelecter(freq, samplerInfo.InsuranceFrequency);
            }
            else
            {
                return new BlockSelecter(freq,
                    samplerInfo.InsuranceFrequency,
                    state.BlockState,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
            }
        }

        public void SaveSamplerStates()
        {
            foreach (var sampler in _sampleSelectors.Values.Select(x => x))
            {
                SaveSampler(sampler);
            }
        }

        public void SaveSampler(ISampleSelector sampler)
        {
            if (sampler is null) { throw new ArgumentNullException(nameof(sampler)); }

            var state = new SamplerState(sampler);
            Dataservice.UpsertSamplerState(state);
        }
    }
}