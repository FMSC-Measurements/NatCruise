using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using NatCruise.Cruise.Models;
using NatCruise.Data;

namespace NatCruise.Cruise.Data
{
    public class TallyPopulationDataservice : CruiseDataserviceBase, ITallyPopulationDataservice
    {
        public TallyPopulationDataservice(string path, string cruiseID) : base(path, cruiseID)
        {
        }

        public TallyPopulationDataservice(CruiseDatastore_V3 database, string cruiseID) : base(database, cruiseID)
        {
        }

        private string SELECT_TALLYPOPULATION_CORE =
@"WITH tallyPopTreeCounts AS (
    SELECT CruiseID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        sum(TreeCount) AS TreeCount,
        sum(KPI) AS SumKPI
    FROM TallyLedger AS tl
    WHERE CuttingUnitCode = @p1 AND CruiseID = @p2
    GROUP BY
        CruiseID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        ifnull(SpeciesCode, ''),
        ifnull(LiveDead, ''))

    SELECT
        tp.Description,
        tp.StratumCode,
        st.Method AS StratumMethod,
        tp.SampleGroupCode,
        tp.SpeciesCode,
        tp.LiveDead,
        tp.HotKey,
        ifnull(tl.TreeCount, 0) AS TreeCount,
        ifnull(tl.SumKPI, 0) AS SumKPI,
        -- sum(tl.KPI) SumKPI,
        sg.SamplingFrequency AS Frequency,
        sg.MinKPI AS sgMinKPI,
        sg.MaxKPI AS sgMaxKPI,
        sg.UseExternalSampler
    -- ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally
    FROM TallyPopulation AS tp
    JOIN SampleGroup AS sg USING (CruiseID, StratumCode, SampleGroupCode)
    -- Left JOIN SamplerState ss USING (CruiseID, StratumCode, SampleGroupCode)
    JOIN Stratum AS st USING (CruiseID, StratumCode)
    JOIN CuttingUnit_Stratum AS cust ON tp.StratumCode = cust.StratumCode AND cust.CuttingUnitCode = @p1
    LEFT JOIN tallyPopTreeCounts AS tl
        ON tl.CuttingUnitCode = @p1
        AND tl.CruiseID =  @p2
        AND tp.StratumCode = tl.StratumCode
        AND tp.SampleGroupCode = tl.SampleGroupCode
        AND ifnull(tp.SpeciesCode, '') = ifnull(tl.SpeciesCode, '')
        AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '') ";

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 0)"
                , new object[] { unitCode, CruiseID }).ToArray();

            //return Database.From<TallyPopulation>()
            //    .Join("Tally", "USING (Tally_CN)")
            //    .Join("SampleGroup", "USING (SampleGroup_CN)")
            //    .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
            //    .Join("Stratum", "USING (Stratum_CN)")
            //    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
            //    .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
            //    .Query(unitCode).ToArray();
        }

        public TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            //var tPops = Database.QueryGeneric(
            //    "SELECT * FROM TallyPopulation AS tp " +
            //    "WHERE tp.StratumCode = @p2 " +
            //        "AND tp.SampleGroupCode = @p3 " +
            //        "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
            //        "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
            //    , new  { p1 = unitCode, p2 = stratumCode, p3= sampleGroupCode, p4 = species, p5 = liveDead }).ToArray();

            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE tp.StratumCode = @p3 " +
                    "AND tp.SampleGroupCode = @p4 " +
                    "AND ifNull(tp.SpeciesCode, '') = ifNull(@p5,'') " +
                    "AND ifNull(tp.LiveDead, '') = ifNull(@p6,'')"
                , new object[] { unitCode, CruiseID, stratumCode, sampleGroupCode, species, liveDead }).FirstOrDefault();
        }

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var tallyPops = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE st.Method IN (SELECT Method FROM LK_CruiseMethod WHERE IsPlotMethod = 1)"
                , new object[] { unitCode, CruiseID }).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND CruiseID = @p2 AND PlotNumber = @p3 AND StratumCode = @p4;",
                    unitCode, CruiseID, plotNumber, pop.StratumCode) == 1;
            }

            return tallyPops;
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
    }
}
