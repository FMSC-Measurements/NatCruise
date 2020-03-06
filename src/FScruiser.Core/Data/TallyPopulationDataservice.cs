using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using FScruiser.Models;

namespace FScruiser.Data
{
    public class TallyPopulationDataservice : DataserviceBase, ITallyPopulationDataservice
    {
        public TallyPopulationDataservice(string path) : base(path)
        {
        }

        public TallyPopulationDataservice(CruiseDatastore_V3 database) : base(database)
        {
        }

        private string SELECT_TALLYPOPULATION_CORE =
    "WITH tallyPopTreeCounts AS (" +
        "SELECT CuttingUnitCode, " +
            "StratumCode, " +
            "SampleGroupCode, " +
            "Species, " +
            "LiveDead, " +
            "sum(TreeCount) AS TreeCount, " +
            "sum(KPI) AS SumKPI " +
        "FROM TallyLedger AS tl " +
        "GROUP BY " +
            "CuttingUnitCode, " +
            "StratumCode, " +
            "SampleGroupCode, " +
            "ifnull(Species, ''), " +
            "ifnull(LiveDead, ''))\r\n" +

        "SELECT " +
            "tp.Description, " +
            "tp.StratumCode, " +
            "st.Method AS StratumMethod, " +
            "tp.SampleGroupCode, " +
            "tp.Species, " +
            "tp.LiveDead, " +
            "tp.HotKey, " +
            "ifnull(tl.TreeCount, 0) AS TreeCount, " +
            "ifnull(tl.SumKPI, 0) AS SumKPI, " +
            //"sum(tl.KPI) SumKPI, " +
            "sg.SamplingFrequency AS Frequency, " +
            "sg.MinKPI AS sgMinKPI, " +
            "sg.MaxKPI AS sgMaxKPI, " +
            "sg.UseExternalSampler " +
        //$"ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally " +
        "FROM TallyPopulation AS tp " +
        "JOIN SampleGroup_V3 AS sg USING (StratumCode, SampleGroupCode) " +
        //"Left JOIN SamplerState ss USING (StratumCode, SampleGroupCode) " +
        "JOIN Stratum AS st ON tp.StratumCode = st.Code " +
        "JOIN CuttingUnit_Stratum AS cust ON tp.StratumCode = cust.StratumCode AND cust.CuttingUnitCode = @p1 " +
        "LEFT JOIN tallyPopTreeCounts AS tl " +
            "ON tl.CuttingUnitCode = @p1 " +
            "AND tp.StratumCode = tl.StratumCode " +
            "AND tp.SampleGroupCode = tl.SampleGroupCode " +
            "AND ifnull(tp.Species, '') = ifnull(tl.Species, '') " +
            "AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '') ";

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                $"WHERE st.Method NOT IN ({PLOT_METHODS})"
                , new object[] { unitCode }).ToArray();

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
                "WHERE tp.StratumCode = @p2 " +
                    "AND tp.SampleGroupCode = @p3 " +
                    "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
                    "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
                , new object[] { unitCode, stratumCode, sampleGroupCode, species, liveDead }).FirstOrDefault();
        }

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var tallyPops = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                $"WHERE st.Method IN ({PLOT_METHODS})"
                , new object[] { unitCode }).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;",
                    unitCode, plotNumber, pop.StratumCode) == 1;
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
                        "AND CuttingUnitCode = @p2 " +
                        "AND PlotNumber = @p3);",
                stratumCode, unitCode, plotNumber) ?? false;
        }
    }
}
