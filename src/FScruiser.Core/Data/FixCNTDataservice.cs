using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL;
using FScruiser.Models;

namespace FScruiser.Data
{
    public class FixCNTDataservice : DataserviceBase, IFixCNTDataservice
    {
        public FixCNTDataservice(string path) : base(path)
        {
        }

        public FixCNTDataservice(CruiseDatastore_V3 database) : base(database)
        {
        }

        public void IncrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeID = GetFixCNTTallyTreeID(unitCode, plotNumber,
                stratumCode, sgCode, species, liveDead,
                fieldName, value);

            if (treeID == null)
            {
                treeID = CreateFixCNTTallyTree(unitCode, plotNumber,
                stratumCode, sgCode, species, liveDead,
                fieldName, value);
            }

            var tallyLedgerID = Guid.NewGuid();

            Database.Execute2("INSERT INTO TallyLedger ( " +
                "TallyLedgerID, " +
                "TreeID, " +
                "CuttingUnitCode, " +
                "PlotNumber, " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "TreeCount " +
                ") VALUES ( " +
                "@TallyLedgerID, " +
                "@TreeID, " +
                "@CuttingUnitCode, " +
                "@PlotNumber, " +
                "@StratumCode, " +
                "@SampleGroupCode, " +
                "@Species, " +
                "@LiveDead, " +
                "1)",
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    Species = species,
                    LiveDead = liveDead,
                });
        }

        public void DecrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeCount = GetTreeCount(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, fieldName, value);
            if(treeCount < 1 ) { return; }

            var treeID = GetFixCNTTallyTreeID(unitCode, plotNumber,
                stratumCode, sgCode, species, liveDead,
                fieldName, value);

            if (treeID != null)
            {
                var tallyLedgerID = Guid.NewGuid();

                Database.Execute2("INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount " +
                    ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "-1)",
                    new
                    {
                        TallyLedgerID = tallyLedgerID,
                        TreeID = treeID,
                        CuttingUnitCode = unitCode,
                        PlotNumber = plotNumber,
                        StratumCode = stratumCode,
                        SampleGroupCode = sgCode,
                        Species = species,
                        LiveDead = liveDead,
                    });
            }
        }

        protected string CreateFixCNTTallyTree(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeID = Guid.NewGuid().ToString();

            var fieldNameStr = fieldName.ToString();

            Database.Execute2(
                "INSERT INTO Tree_V3 (" +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES (" +
                    "@TreeID,\r\n " + //treeID
                    "(SELECT ifnull(max(TreeNumber), 0) + 1  " +
                        "FROM Tree_V3 AS t1 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber),\r\n " + //get highest tree number using unitCode and plot_cn
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " + //plot_cn
                    "@StratumCode,\r\n " + //stratum_cn
                    "@SampleGroupCode,\r\n " + //sampleGroup_CN
                    "@Species,\r\n" + //species
                    "@LiveDead,\r\n" + //liveDead
                    "'M'" +
                ");" + //countMeasure

                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {fieldNameStr}) VALUES (@TreeID, @value);",
                new
                {
                    TreeID = treeID,
                    TallyLedgerID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    Species = species,
                    LiveDead = liveDead,
                    value,
                }
            );

            return treeID;
        }

        protected string GetFixCNTTallyTreeID(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var fieldNameStr = fieldName.ToString();

            return Database.ExecuteScalar<string>(
                "SELECT " +
                    "t.TreeID " +
                "FROM Tree_V3 AS t " +
                "JOIN TreeMeasurment AS tm USING (TreeID) " +
                $"WHERE tm.{fieldNameStr} = @p1 " +
                    "AND t.PlotNumber = @p2 " +
                    "AND t.CuttingUnitCode = @p3 " +
                    "AND t.StratumCode = @p4 " +
                    "AND t.SampleGroupCode = @p5 " +
                    "AND ifnull(t.Species, '') = ifnull(@p6, '') " +
                    "AND ifnull(t.LiveDead, '') = ifnull(@p7, '') " +
                "LIMIT 1;",
                 value, plotNumber, unitCode, stratumCode, sgCode, species, liveDead);
        }

        public IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode)
        {
            return Database.Query<FixCntTallyPopulation>(
                "SELECT " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "Field, " +
                    "Min, " +
                    "Max, " +
                    "IntervalSize " +
                "FROM FixCNTTallyPopulation_V3 AS ftp " +
                "JOIN FixCNTTallyClass_V3 AS tc USING (StratumCode) " +
                "WHERE StratumCode = @p1;",
                new object[] { stratumCode });
        }

        public int GetTreeCount(string unit,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode,
            string species,
            string livedead,
            string field,
            double value)
        {
            return Database.ExecuteScalar2<int>(
                "SELECT Sum(tl.TreeCount) FROM TallyLedger AS tl " +
                "JOIN TreeFieldValue_All AS tfv USING (TreeID) " +
                "WHERE tl.CuttingUnitCode = @CuttingUnitCode " +
                "AND tl.PlotNumber = @PlotNumber " +
                "AND tl.StratumCode = @StratumCode " +
                "AND tl.SampleGroupCode = @SampleGroupCode " +
                "AND tl.Species = @Species " +
                "AND tl.LiveDead = @LiveDead " +
                "AND tfv.Field = @Field " +
                "AND tfv.ValueReal = @Value;",
                new
                {
                    CuttingUnitCode = unit,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    Species = species,
                    LiveDead = livedead,
                    Field = field,
                    Value = value,
                });
        }
    }
}
