using CruiseDAL.Schema;
using FMSC.Sampling;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Logic;
using NatCruise.Cruise.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Services
{
    public class PlotTallyService : IPlotTallyService
    {
        private static readonly string[] SINGLE_STAGE_PLOT = new string[] { CruiseMethods.PNT, CruiseMethods.FIX };

        public ISampleSelectorDataService SampleSelectorDataservice { get; }
        public ICruiseDialogService DialogService { get; }
        public IPlotTallyDataservice PlotTallyDataservice { get; }

        public PlotTallyService(ICruiseDialogService dialogService, IPlotTallyDataservice plotTallyDataservice, ISampleSelectorDataService sampleSelectorDataservice)
        {
            PlotTallyDataservice = plotTallyDataservice ?? throw new ArgumentNullException(nameof(plotTallyDataservice));
            SampleSelectorDataservice = sampleSelectorDataservice ?? throw new ArgumentNullException(nameof(sampleSelectorDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public async Task<TreeStub_Plot> TallyAsync(TallyPopulation_Plot pop,
                                              string unitCode,
                                              int plot)
        {
            if (pop is null) { throw new ArgumentNullException(nameof(pop)); }
            var dialogService = DialogService;

            TreeStub_Plot tree;
            if (SINGLE_STAGE_PLOT.Contains(pop.Method))
            {
                tree = CreateTree(unitCode, plot, pop, "M");
                PlotTallyDataservice.InsertTree(tree, (SamplerState)null);
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
                            tree = CreateTree(unitCode, plot, pop, "M", stm: true);
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
                        PlotTallyDataservice.InsertTree(tree, new SamplerState(sampler));
                    }
                    else
                    {
                        PlotTallyDataservice.InsertTree(tree, null);
                    }
                }
            }

            return tree;
        }

        public static TreeStub_Plot CreateTree(string unitCode,
                                        int plotNumber,
                                        TallyPopulation_Plot population,
                                        string countOrMeasure,
                                        int treeCount = 1,
                                        int kpi = 0,
                                        int threePRand = 0,
                                        bool stm = false)
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
                ThreePRandomValue = threePRand,
                STM = stm,
                TreeID = Guid.NewGuid().ToString()
            };
        }
    }
}