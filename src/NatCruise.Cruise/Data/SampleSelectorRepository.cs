using FMSC.Sampling;
using Microsoft.AppCenter.Analytics;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Sampling;
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

        public SampleSelectorRepository(ISamplerStateDataservice dataservice, ISampleGroupDataservice sampleGroupDataservice)
        {
            SamplerStateDataservice = dataservice ?? throw new ArgumentNullException(nameof(dataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        }

        public ISamplerStateDataservice SamplerStateDataservice { get; set; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }

        public string DeviceID => SamplerStateDataservice.DeviceID;

        public ISampleSelector GetSamplerBySampleGroupCode(string stratumCode, string sgCode)
        {
            if (string.IsNullOrEmpty(stratumCode)) { throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty", nameof(stratumCode)); }
            if (string.IsNullOrEmpty(sgCode)) { throw new ArgumentException($"'{nameof(sgCode)}' cannot be null or empty", nameof(sgCode)); }

            var key = stratumCode + "/" + sgCode;

            if (_sampleSelectors.ContainsKey(key) == false)
            {
                var samplerInfo = SampleGroupDataservice.GetSampleGroup(stratumCode, sgCode);

                var sampler = MakeSampleSelecter(samplerInfo);

                _sampleSelectors.Add(key, sampler);
            }

            return _sampleSelectors[key];
        }

        public ISampleSelector MakeSampleSelecter(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var method = sampleGroup.CruiseMethod;
            var sampleSelectortype = sampleGroup.SampleSelectorType;

            //if ((sg.TallyMethod & CruiseDAL.Enums.TallyMode.Manual) == CruiseDAL.Enums.TallyMode.Manual)
            //{
            //    return null;
            //}

            switch (method)
            {
                case "FIXCNT":
                    {
                        return new ZeroFrequencySelecter(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);
                    }
                case "100":
                case "FIX":
                case "PNT":
                    {
                        return new HundredPCTSelector()
                        {
                            SampleGroupCode = sampleGroup.SampleGroupCode,
                            StratumCode = sampleGroup.StratumCode,
                        };
                    }

                case "STR":
                    {
                        // default sample selector for STR is blocked
                        if (sampleSelectortype == SAMPLESELECTORTYPE_SYSTEMATICSELECTER)
                        { return MakeSystematicSampleSelector(sampleGroup); }
                        else
                        { return MakeBlockSampleSelector(sampleGroup); }
                    }
                case "S3P":
                    {
                        return MakeS3PSampleSelector(sampleGroup);
                    }
                case "3P":
                case "P3P":
                case "F3P":
                    {
                        return MakeThreePSampleSelector(sampleGroup);
                    }
                case "FCM":
                case "PCM":
                    {
                        // default sample selector for plot methods is systematic
                        if (sampleSelectortype == SAMPLESELECTORTYPE_BLOCKSELECTER)
                        { return MakeBlockSampleSelector(sampleGroup); }
                        else
                        { return MakeSystematicSampleSelector(sampleGroup); }
                    }
                case null:
                    { throw new NullReferenceException("method should not be null"); }
                default:
                    {
                        return null;
                    }
            }
        }

        public ISampleSelector MakeS3PSampleSelector(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var state = SamplerStateDataservice.GetSamplerState(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);

            if (state != null)
            {
                var selector = new S3PSelector(sampleGroup.SamplingFrequency, sampleGroup.KZ, state.Counter, state.BlockState);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new S3PSelector(sampleGroup.SamplingFrequency, sampleGroup.KZ);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
                return selector;
            }
        }

        public ISampleSelector MakeThreePSampleSelector(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var state = SamplerStateDataservice.GetSamplerState(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);

            if (state != null)
            {
                var selector = new ThreePSelecter(sampleGroup.KZ,
                    sampleGroup.InsuranceFrequency,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new ThreePSelecter(sampleGroup.KZ, sampleGroup.InsuranceFrequency);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
                return selector;
            }
        }

        public ISampleSelector MakeSystematicSampleSelector(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var state = SamplerStateDataservice.GetSamplerState(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);

            var freq = sampleGroup.SamplingFrequency;

            if (freq == 0)
            {
                // frequency shouldn't be 0,
                // but I don't want it to break the program if it is, so for now lets just track it
                Analytics.TrackEvent("ZeroFrequencySelecter Created", new Dictionary<string, string> { { "Method", "MakeSystematicSampleSelector" }, });
                return new ZeroFrequencySelecter(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);
            }
            else
            {
                if (state != null)
                {
                    var selecter = new SystematicSelecter(freq,
                        sampleGroup.InsuranceFrequency,
                        state.Counter,
                        state.InsuranceIndex,
                        state.InsuranceCounter,
                        state.SystematicIndex);
                    selecter.StratumCode = sampleGroup.StratumCode;
                    selecter.SampleGroupCode = sampleGroup.SampleGroupCode;
                    return selecter;
                }
                else
                {
                    var selecter = new SystematicSelecter(freq, sampleGroup.InsuranceFrequency, true);
                    selecter.StratumCode = sampleGroup.StratumCode;
                    selecter.SampleGroupCode = sampleGroup.SampleGroupCode;
                    return selecter;
                }
            }
        }

        public ISampleSelector MakeBlockSampleSelector(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var state = SamplerStateDataservice.GetSamplerState(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);

            var freq = sampleGroup.SamplingFrequency;

            if (freq == 0)
            {
                // frequency shouldn't be 0,
                // but I don't want it to break the program if it is, so for now lets just track it
                Analytics.TrackEvent("ZeroFrequencySelecter Created", new Dictionary<string, string> { { "Method", "MakeBlockSampleSelector" }, });
                return new ZeroFrequencySelecter(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);
            }

            // we need to gruard against a empty block state
            // if a block selector is initialized with a frequency of 0
            // then the block state will be empty
            if (state != null && string.IsNullOrWhiteSpace(state.BlockState) == false)
            {
                var selector = new BlockSelecter(freq,
                    sampleGroup.InsuranceFrequency,
                    state.BlockState,
                    state.Counter,
                    state.InsuranceIndex,
                    state.InsuranceCounter);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
                return selector;
            }
            else
            {
                var selector = new BlockSelecter(freq, sampleGroup.InsuranceFrequency);
                selector.StratumCode = sampleGroup.StratumCode;
                selector.SampleGroupCode = sampleGroup.SampleGroupCode;
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