using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using NatCruise.Models;
using NatCruise.Data;
using CruiseDAL.Schema;

namespace NatCruise.Data
{
    public class TallyPopulationDataservice : CruiseDataserviceBase, ITallyPopulationDataservice
    {
        public TallyPopulationDataservice(string path, string cruiseID, string deviceID)
            : base(path, cruiseID, deviceID)
        {
        }

        public TallyPopulationDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID)
            : base(database, cruiseID, deviceID)
        {
        }

        private string SELECT_TALLYPOPULATION_CORE =
$@"SELECT
        cust.CuttingUnitCode,
        tp.Description,
        tp.StratumCode,
        st.Method AS StratumMethod,
        tp.SampleGroupCode,
        tp.SpeciesCode,
        tp.LiveDead,
        sg.DefaultLiveDead,
        tp.HotKey,
            (SELECT ifnull(sum(TreeCount), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.CuttingUnitCode = cust.CuttingUnitCode
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
            ) AS TreeCount,
            
            (SELECT ifnull(sum(TreeCount), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
            ) AS TreeCountCruise,

            (SELECT ifnull(sum(KPI), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.CuttingUnitCode = cust.CuttingUnitCode
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
            ) AS SumKPI,

            

            (SELECT ifnull(sum(KPI), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
            ) AS SumKPICruise,

            (SELECT ifnull(sum(KPI), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.CuttingUnitCode = cust.CuttingUnitCode
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
                AND tl.TreeID IS NOT NULL
            ) AS TreeSumKPI,

            (SELECT ifnull(sum(KPI), 0) FROM
                TallyLedger AS tl
            WHERE
                tl.CruiseID = tp.CruiseID
                AND tl.CuttingUnitCode = cust.CuttingUnitCode
                AND tl.StratumCode = tp.StratumCode
                AND tl.SampleGroupCode = tp.SampleGroupCode
                AND (tp.SpeciesCode IS NULL OR tp.SpeciesCode = tl.SpeciesCode)
                AND (tp.LiveDead IS NULL OR tp.LiveDead = tl.LiveDead)
                AND tl.TreeID IS NOT NULL
            ) AS TreeSumKPICruise,

        sg.SamplingFrequency AS Frequency,
        sg.InsuranceFrequency,
        sg.KZ, 
        sg.MinKPI AS sgMinKPI,
        sg.MaxKPI AS sgMaxKPI,
        sg.SampleSelectorType 
    FROM TallyPopulation AS tp
    JOIN SampleGroup AS sg USING (CruiseID, StratumCode, SampleGroupCode)
    -- Left JOIN SamplerState ss USING (CruiseID, StratumCode, SampleGroupCode)
    JOIN Stratum AS st USING (CruiseID, StratumCode)
    JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
";

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE cust.CuttingUnitCode = @p1 AND tp.CruiseID = @p2 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 0)"
                , new object[] { unitCode, CruiseID }).ToArray();
        }

        public TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {

            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE cust.CuttingUnitCode = @p1 " +
                    "AND tp.CruiseID = @p2 " +
                    "AND tp.StratumCode = @p3 " +
                    "AND tp.SampleGroupCode = @p4 " +
                    "AND ifNull(tp.SpeciesCode, '') = ifNull(@p5,'') " +
                    "AND ifNull(tp.LiveDead, '') = ifNull(@p6,'')"
                , unitCode, CruiseID, stratumCode, sampleGroupCode, species, liveDead).FirstOrDefault();
        }

        public TallyPopulation_Plot GetPlotTallyPopulation(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            var tallyPop = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE cust.CuttingUnitCode = @p1 " +
                    "AND tp.CruiseID = @p2 " +
                    "AND tp.StratumCode = @p3 " +
                    "AND tp.SampleGroupCode = @p4 " +
                    "AND ifNull(tp.SpeciesCode, '') = ifNull(@p5,'') " +
                    "AND ifNull(tp.LiveDead, '') = ifNull(@p6,'')"
                , unitCode, CruiseID, stratumCode, sampleGroupCode, species, liveDead).Single();

            tallyPop.TreeCountPlot = tallyPop.PlotTreeCount = GetTreeCount(tallyPop, plotNumber);
            tallyPop.SumKPIPlot = GetSumKPI(tallyPop, plotNumber);

            return tallyPop;
        }

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var tallyPops = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE tp.CruiseID = @p2 AND cust.CuttingUnitCode = @p1 AND st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)"
                , unitCode, CruiseID).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber = @p3 AND StratumCode = @p4;",
                    unitCode, CruiseID, plotNumber, pop.StratumCode) == 1;
                pop.PlotTreeCount = GetTreeCount(pop, plotNumber);
            }

            return tallyPops;
        }

        public int GetTreeCount(TallyPopulation pop, int plotNumber)
        {
            return Database.ExecuteScalar2<int>("SELECT TreeCount FROM TallyLedger_Plot_Totals " +
                    "WHERE CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode " +
                    "AND SampleGroupCode = @SampleGroupCode " +
                    "AND (@SpeciesCode IS NULL OR  ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '')) " +
                    "AND (@LiveDead IS NULL OR ifnull(LiveDead, '') = ifnull(@LiveDead, ''));",
                    new { CruiseID, pop.CuttingUnitCode, PlotNumber = plotNumber, pop.StratumCode, pop.SampleGroupCode, pop.SpeciesCode, pop.LiveDead });
        }

        public int GetSumKPI(TallyPopulation pop, int plotNumber)
        {
            return Database.ExecuteScalar2<int>("SELECT ifnull(KPI, 0) FROM TallyLedger_Plot_Totals " +
                    "WHERE CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode " +
                    "AND SampleGroupCode = @SampleGroupCode " +
                    "AND (@SpeciesCode IS NULL OR  ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '')) " +
                    "AND (@LiveDead IS NULL OR ifnull(LiveDead, '') = ifnull(@LiveDead, ''));",
                    new { CruiseID, pop.CuttingUnitCode, PlotNumber = plotNumber, pop.StratumCode, pop.SampleGroupCode, pop.SpeciesCode, pop.LiveDead });
        }

        private bool GetIsTallyPopInCruise(string unitCode, int plotNumber, string stratumCode)
        {
            return Database.ExecuteScalar<bool?>(
                "SELECT EXISTS (" +
                    "SELECT * " +
                    "FROM Plot_Stratum " +
                    "WHERE StratumCode = @p1 " +
                        "AND CruiseID = @p2 " +
                        "AND CuttingUnitCode = @p3 " +
                        "AND PlotNumber = @p4);",
                stratumCode, CruiseID, unitCode, plotNumber) ?? false;
        }

        public IEnumerable<TallyPopulation> GetTallyPopulations(string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulations(string cuttingUnitCode = null, string stratumCode = null, string sampleGroupCode = null)
        {
            return Database.Query2<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE tp.CruiseID = @CruiseID " +
                    "AND (@CuttingUnitCode IS NULL OR cust.CuttingUnitCode = @CuttingUnitCode) " +
                    "AND (@StratumCode IS NULL OR tp.StratumCode = @StratumCode) " +
                    "AND (@SampleGroupCode IS NULL OR tp.SampleGroupCode = @SampleGroupCode);"
                ,
                new {
                    CruiseID,
                    CuttingUnitCode = cuttingUnitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode
                }).ToArray();
        }

        public void UpdateTallyPopulation(TallyPopulation tallyPop)
        {
            throw new NotImplementedException();
        }
    }
}
