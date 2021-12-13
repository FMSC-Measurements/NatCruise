using FMSC.Sampling;
using Microsoft.AppCenter.Analytics;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Cruise.Services
{
    // TODO can this class be folded into ISampleInfoDataservie?
    public class SampleSelectorRepository : ISampleSelectorDataService
    {
        public const string SAMPLESELECTORTYPE_SYSTEMATICSELECTER = "SystematicSelecter";
        public const string SAMPLESELECTORTYPE_BLOCKSELECTER = "BlockSelecter";
        public const string SAMPLESELECTORTYPE_CLICKERSELECTER = "ClickerSelecter";

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

                _sampleSelectors.Add(key, sampler);
            }

            return _sampleSelectors[key];
        }

        public ISampleSelector MakeSampleSelecter(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var method = samplerInfo.Method;
            var sampleSelectortype = samplerInfo.SampleSelectorType;

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
                        return new HundredPCTSelector()
                        {
                            SampleGroupCode = samplerInfo.SampleGroupCode,
                            StratumCode = samplerInfo.StratumCode,
                        };
                    }

                case "STR":
                    {
                        // default sample selector for STR is blocked
                        if (sampleSelectortype == SAMPLESELECTORTYPE_SYSTEMATICSELECTER)
                        { return MakeSystematicSampleSelector(samplerInfo); }
                        else
                        { return MakeBlockSampleSelector(samplerInfo); }
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
                        // default sample selector for plot methods is systematic
                        if (sampleSelectortype == SAMPLESELECTORTYPE_BLOCKSELECTER)
                        { return MakeBlockSampleSelector(samplerInfo); }
                        else
                        { return MakeSystematicSampleSelector(samplerInfo); }
                    }
                case null:
                    { throw new NullReferenceException("method should not be null"); }
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
                var selector = new S3PSelector(samplerState.SamplingFrequency, samplerState.KZ, state.Counter, state.BlockState);
                selector.StratumCode = samplerState.StratumCode;
                selector.SampleGroupCode = samplerState.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new S3PSelector(samplerState.SamplingFrequency, samplerState.KZ);
                selector.StratumCode = samplerState.StratumCode;
                selector.SampleGroupCode = samplerState.SampleGroupCode;
                return selector;
            }
        }

        public ISampleSelector MakeThreePSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            if (state != null)
            {
                var selector = new ThreePSelecter(samplerInfo.KZ,
                    samplerInfo.InsuranceFrequency,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
                selector.StratumCode = samplerInfo.StratumCode;
                selector.SampleGroupCode = samplerInfo.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new ThreePSelecter(samplerInfo.KZ, samplerInfo.InsuranceFrequency);
                selector.StratumCode = samplerInfo.StratumCode;
                selector.SampleGroupCode = samplerInfo.SampleGroupCode;
                return selector;
            }
        }

        public ISampleSelector MakeSystematicSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            var freq = samplerInfo.SamplingFrequency;

            if (freq == 0)
            {
                // frequency shouldn't be 0,
                // but I don't want it to break the program if it is, so for now lets just track it
                Analytics.TrackEvent("ZeroFrequencySelecter Created", new Dictionary<string, string> { { "Method", "MakeSystematicSampleSelector" }, });
                return new ZeroFrequencySelecter(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);
            }
            else
            {
                if (state != null)
                {
                    var selecter = new SystematicSelecter(freq,
                        samplerInfo.InsuranceFrequency,
                        state.Counter,
                        state.InsuranceIndex,
                        state.InsuranceCounter,
                        state.SystematicIndex);
                    selecter.StratumCode = samplerInfo.StratumCode;
                    selecter.SampleGroupCode = samplerInfo.SampleGroupCode;
                    return selecter;
                }
                else
                {
                    var selecter = new SystematicSelecter(freq, samplerInfo.InsuranceFrequency, true);
                    selecter.StratumCode = samplerInfo.StratumCode;
                    selecter.SampleGroupCode = samplerInfo.SampleGroupCode;
                    return selecter;
                }
            }
        }

        public ISampleSelector MakeBlockSampleSelector(SamplerInfo samplerInfo)
        {
            if (samplerInfo is null) { throw new ArgumentNullException(nameof(samplerInfo)); }

            var state = Dataservice.GetSamplerState(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);

            var freq = samplerInfo.SamplingFrequency;

            if (freq == 0)
            {
                // frequency shouldn't be 0,
                // but I don't want it to break the program if it is, so for now lets just track it
                Analytics.TrackEvent("ZeroFrequencySelecter Created", new Dictionary<string, string> { { "Method", "MakeBlockSampleSelector" }, });
                return new ZeroFrequencySelecter(samplerInfo.StratumCode, samplerInfo.SampleGroupCode);
            }

            // we need to gruard against a empty block state
            // if a block selector is initialized with a frequency of 0
            // then the block state will be empty
            if (state != null && string.IsNullOrWhiteSpace(state.BlockState) == false)
            {
                var selector = new BlockSelecter(freq,
                    samplerInfo.InsuranceFrequency,
                    state.BlockState,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
                selector.StratumCode = samplerInfo.StratumCode;
                selector.SampleGroupCode = samplerInfo.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new BlockSelecter(freq, samplerInfo.InsuranceFrequency);
                selector.StratumCode = samplerInfo.StratumCode;
                selector.SampleGroupCode = samplerInfo.SampleGroupCode;
                return selector;
            }
        }

        //public void SaveSamplerStates()
        //{
        //    foreach (var sampler in _sampleSelectors.Values.Select(x => x))
        //    {
        //        SaveSampler(sampler);
        //    }
        //}

        //public void SaveSampler(ISampleSelector sampler)
        //{
        //    if (sampler is null) { throw new ArgumentNullException(nameof(sampler)); }

        //    var state = new SamplerState(sampler);
        //    Dataservice.UpsertSamplerState(state);
        //}
    }
}