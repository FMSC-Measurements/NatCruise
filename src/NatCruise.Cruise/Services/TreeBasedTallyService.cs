using CruiseDAL.Schema;
using FMSC.Sampling;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using NatCruise.Data;
using System;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public class TreeBasedTallyService : ITreeBasedTallyService
    {
        public ISampleSelectorDataService SampleSelectorDataservice { get; }
        public ICruiseDialogService DialogService { get; }
        public ITallyDataservice TallyDataservice { get; }

        public TreeBasedTallyService(ICruiseDialogService dialogService, ITallyDataservice tallyDataservice, ISampleSelectorDataService sampleSelectorDataservice)
        {
            TallyDataservice = tallyDataservice ?? throw new ArgumentNullException(nameof(TallyDataservice));
            SampleSelectorDataservice = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(SampleSelectorDataservice));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public async Task<TallyEntry> TallyAsync(string unitCode, TallyPopulation pop)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            var samplerService = SampleSelectorDataservice;
            var dialogService = DialogService;

            TallyAction tallyAction;
            if (pop.IsClickerTally)
            {
                var clickerTallyResult = await dialogService.AskTreeCount(pop.Frequency);
                if (clickerTallyResult != null && clickerTallyResult.TreeCount.HasValue)
                {
                    tallyAction = CreateTally(unitCode, pop, SampleResult.M, treeCount: clickerTallyResult.TreeCount.Value);
                }
                else
                {
                    return null;
                }
            }

            if (pop.Method == CruiseMethods.S3P)
            {
                tallyAction = await TallyS3P(unitCode, pop);
            }
            else
            {
                var sampler = samplerService.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode);

                if (pop.Is3P)//threeP sampling
                {
                    int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                    if (kpi != null)
                    {
                        tallyAction = TallyThreeP(unitCode, pop, kpi.Value, (IThreePSelector)sampler);
                    }
                    else { return null; }//user didn't enter a kpi, so don't create a tally entry
                }
                else//non 3P sampling (STR)
                {
                    var result = ((IFrequencyBasedSelecter)sampler).Sample();
                    tallyAction = CreateTally(unitCode, pop, result);
                }

                // don't persist state of zeroFrequencySelecter
                // TODO update how sample state gets persisted to make this code cleaner
                if ((sampler is ZeroFrequencySelecter) == false)
                {
                    tallyAction.SamplerState = new SamplerState(sampler);
                }
            }

            if (tallyAction != null)
            {
                return await TallyDataservice.InsertTallyActionAsync(tallyAction);
            }
            else
            {
                return null;
            }
        }

        private static TallyAction CreateTally(string unitCode, TallyPopulation population,
            SampleResult sampleResult, int treeCount = 1, int kpi = 0, int threePRandomeValue = 0, bool stm = false)
        {
            var tallyEntry = new TallyAction(unitCode, population)
            {
                SampleResult = sampleResult,
                //CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRandomeValue,
                STM = stm,
            };

            return tallyEntry;
        }

        public async Task<TallyAction> TallyS3P(string unitCode, TallyPopulation pop)
        {
            if (pop is null) { throw new System.ArgumentNullException(nameof(pop)); }
            var samplerService = SampleSelectorDataservice;
            var dialogService = DialogService;

            var sampler = samplerService.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode) as S3PSelector;

            TallyAction tallyAction;
            //If we receive nothing from the sampler, we don't have a sample
            if (sampler.Sample() == SampleResult.M)
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    if (kpi == -1)  //user entered sure to measure
                    {
                        tallyAction = CreateTally(unitCode, pop, SampleResult.M, stm: true);
                    }
                    else
                    {
                        var result = sampler.Sample(kpi.Value, out var rand);

                        tallyAction = CreateTally(unitCode, pop, result,
                                kpi: kpi.Value, threePRandomeValue: rand);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                tallyAction = CreateTally(unitCode, pop, SampleResult.I);
            }

            return tallyAction;
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public TallyAction TallyThreeP(string unitCode,
            TallyPopulation pop,
            int kpi,
            IThreePSelector sampler)
        {
            if (pop is null) { throw new System.ArgumentNullException(nameof(pop)); }
            if (sampler is null) { throw new System.ArgumentNullException(nameof(sampler)); }
            var samplerService = SampleSelectorDataservice;

            if (kpi == -1)  //user entered sure to measure
            {
                return CreateTally(unitCode, pop, SampleResult.M, stm: true);
            }
            else
            {
                var result = sampler.Sample(kpi, out var rand);
                return CreateTally(unitCode, pop, result,
                        kpi: kpi, threePRandomeValue: rand);
            }
        }
    }
}