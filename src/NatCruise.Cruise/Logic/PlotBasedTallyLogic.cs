using CruiseDAL.Schema;
using FMSC.Sampling;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Logic
{
    public static class PlotBasedTallyLogic
    {
        static readonly string[] SINGLE_STAGE_PLOT = new string[] { CruiseMethods.PNT, CruiseMethods.FIX };


        public static TreeStub_Plot CreateTree(string unitCode, int plotNumber, TallyPopulation_Plot population, string countOrMeasure, int treeCount = 1, int kpi = 0, bool stm = false)
        {
            if (population is null) { throw new ArgumentNullException(nameof(population)); }

            return new TreeStub_Plot
            {
                CuttingUnitCode = unitCode,
                StratumCode = population.StratumCode,
                SampleGroupCode = population.SampleGroupCode,
                SpeciesCode = population.SpeciesCode,
                LiveDead = population.LiveDead,
                PlotNumber = plotNumber,
                CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                STM = (stm) ? "Y" : "N",
                TreeID = Guid.NewGuid().ToString()
            };
        }

        public static async Task<TreeStub_Plot> TallyAsync(TallyPopulation_Plot pop,
            string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo,
            ICruiseDialogService dialogService)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            if (sampleSelectorRepo is null) { throw new ArgumentNullException(nameof(sampleSelectorRepo)); }
            if (dialogService is null) { throw new ArgumentNullException(nameof(dialogService)); }

            if (SINGLE_STAGE_PLOT.Contains(pop.Method))
            {
                return CreateTree(unitCode, plot, pop, "M");
            }
            else if (pop.Is3P)//threeP sampling
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    return TallyThreeP(pop, kpi.Value, unitCode, plot, sampleSelectorRepo);
                }
                else
                { return null; }
            }
            else//non 3P sampling (STR)
            {
                return TallyStandard(pop, unitCode, plot, sampleSelectorRepo);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TreeStub_Plot TallyThreeP(TallyPopulation_Plot pop,
            int kpi,
            string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            if (sampleSelectorRepo is null) { throw new ArgumentNullException(nameof(sampleSelectorRepo)); }

            var sampler = sampleSelectorRepo.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode) as IThreePSelector;

            if (kpi == -1)  //user entered sure to measure
            {
                return CreateTree(unitCode, plot, pop, "M", stm: true);
            }
            else
            {
                var result = sampler.Sample(kpi, out var rand);
                return CreateTree(unitCode, plot, pop, result.ToString(), kpi: kpi);
            }
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TreeStub_Plot TallyStandard(TallyPopulation_Plot pop, string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            if (sampleSelectorRepo is null) { throw new ArgumentNullException(nameof(sampleSelectorRepo)); }

            var sampler = sampleSelectorRepo.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode) as IFrequencyBasedSelecter;
            var result = sampler.Sample();

            return CreateTree(unitCode, plot, pop, result.ToString());
        }
    }
}
