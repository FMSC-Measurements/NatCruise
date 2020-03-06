using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Services;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class TreeBasedTallyLogic
    {
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

        public static async Task<TallyAction> TallyAsync(string unitCode,
            TallyPopulation pop,
            ISampleSelectorDataService samplerService,
            IDialogService dialogService)
        {
            if (pop.IsClickerTally)
            {
                var clickerTallyResult = await dialogService.AskTreeCount(pop.Frequency);
                if (clickerTallyResult != null && clickerTallyResult.TreeCount.HasValue)
                {
                    return CreateTally(unitCode, pop, SampleResult.M, treeCount: clickerTallyResult.TreeCount.Value);
                }
                else
                {
                    return null;
                }
            }

            TallyAction tallyAction = null;
            var sampler = samplerService.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode);

            if (pop.Method == CruiseMethods.S3P)
            {
                tallyAction =  await TallyS3P(unitCode, pop, samplerService, dialogService);
            }
            else if (pop.Is3P)//threeP sampling
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

            if(tallyAction != null)
            {
                tallyAction.SamplerState = new SamplerState(sampler);
            }

            return tallyAction;
        }

        public static async Task<TallyAction> TallyS3P(string unitCode, TallyPopulation pop,
            ISampleSelectorDataService samplerService,
            IDialogService dialogService)
        {
            var sampler = samplerService.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode) as S3PSelector;

            //If we receive nothing from the sampler, we don't have a sample
            if (sampler.Sample() == SampleResult.M)
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    if (kpi == -1)  //user entered sure to measure
                    {
                        return CreateTally(unitCode, pop, SampleResult.M, stm: true);
                    }
                    else
                    {
                        var result = sampler.Sample(kpi.Value, out var rand);

                        return CreateTally(unitCode, pop, result,
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
                return CreateTally(unitCode, pop, SampleResult.I);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyAction TallyThreeP(string unitCode,
            TallyPopulation pop,
            int kpi,
            IThreePSelector sampler)
        {
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