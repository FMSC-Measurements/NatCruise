using CruiseDAL.Schema;
using FMSC.Sampling;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Sampling;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public class PlotTallyService : IPlotTallyService
    {
        private static readonly string[] SINGLE_STAGE_PLOT = new string[] { CruiseMethods.PNT, CruiseMethods.FIX };

        public ISampleSelectorDataService SampleSelectorDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public IPlotTreeDataservice PlotTreeDataservice { get; }

        public PlotTallyService(INatCruiseDialogService dialogService, IPlotTreeDataservice plotTreeDataservice, ISampleSelectorDataService sampleSelectorDataservice)
        {
            PlotTreeDataservice = plotTreeDataservice ?? throw new ArgumentNullException(nameof(plotTreeDataservice));
            SampleSelectorDataservice = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(sampleSelectorDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public async Task<PlotTreeEntry> TallyAsync(TallyPopulation_Plot pop,
                                              string unitCode,
                                              int plot)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            var dialogService = DialogService;

            PlotTreeEntry tree;
            if (SINGLE_STAGE_PLOT.Contains(pop.Method))
            {
                tree = CreateTree(unitCode, plot, pop, "M");
                PlotTreeDataservice.InsertTree(tree, (SamplerState)null);
                return tree;
            }
            else
            {
                var sampleSelectorRepo = SampleSelectorDataservice;
                var sampler = sampleSelectorRepo.GetSamplerBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode);

                if (pop.Is3P)//threeP sampling
                {
                    int? nkpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                    if (nkpi != null)
                    {
                        var kpi = nkpi.Value;
                        if (kpi == -1)  //user entered sure to measure
                        {
                            // stm trees get count of 0
                            tree = CreateTree(unitCode, plot, pop, "M", treeCount:0, stm: true);
                        }
                        else
                        {
                            var result = ((IThreePSelector)sampler).Sample(kpi, out var rand);
                            tree = CreateTree(unitCode, plot, pop, result.ToString(), kpi: kpi, threePRand: rand);
                        }
                    }
                    else
                    { return null; }
                }
                else//non 3P sampling (STR)
                {
                    var result = ((IFrequencyBasedSelecter)sampler).Sample();
                    tree = CreateTree(unitCode, plot, pop, result.ToString());
                }

                if (tree != null)
                {
                    // don't persist state of zeroFrequencySelecter
                    // TODO update how sample state gets persisted to make this code cleaner
                    if ((sampler is ZeroFrequencySelecter) == false)
                    {
                        PlotTreeDataservice.InsertTree(tree, new SamplerState(sampler));
                    }
                    else
                    {
                        PlotTreeDataservice.InsertTree(tree, null);
                    }
                }
            }

            return tree;
        }

        public static PlotTreeEntry CreateTree(string unitCode,
                                        int plotNumber,
                                        TallyPopulation_Plot population,
                                        string countOrMeasure,
                                        int treeCount = 1,
                                        int kpi = 0,
                                        int threePRand = 0,
                                        bool stm = false)
        {
            if (population is null) { throw new ArgumentNullException(nameof(population)); }

            string liveDead = null;
            if(string.IsNullOrEmpty(population.LiveDead))
            {
                liveDead = population.DefaultLiveDead;
            }
            else
            {
                liveDead = population.LiveDead;
            }

            return new PlotTreeEntry
            {
                CuttingUnitCode = unitCode,
                StratumCode = population.StratumCode,
                SampleGroupCode = population.SampleGroupCode,
                SpeciesCode = population.SpeciesCode,
                LiveDead = liveDead,
                PlotNumber = plotNumber,
                CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRand,
                STM = stm,
                TreeID = Guid.NewGuid().ToString()
            };
        }
    }
}