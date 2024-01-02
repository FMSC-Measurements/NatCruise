using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public class FixCNTDataservice : CruiseDataserviceBase, IFixCNTDataservice
    {
        public FixCNTDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public FixCNTDataservice(string path, string cruiseID, string deviceID)
            : base(path, cruiseID, deviceID)
        {
        }

        public FixCNTDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID)
            : base(database, cruiseID, deviceID)
        {
        }

        public bool GetOneTreePerTallyOption()
        {
            return false;
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

            Database.Execute2(
@"INSERT INTO TallyLedger (
    TallyLedgerID,
    TreeID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    EntryType,
    TreeCount,
    CreatedBy
) VALUES (
    @TallyLedgerID,
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    'tally',
    1, -- TreeCount
    @CreatedBy
);",
                new
                {
                    CruiseID,
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = species,
                    LiveDead = liveDead,
                    CreatedBy = DeviceID,
                });
        }

        public void DecrementFixCNTTreeCount(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeCount = GetTreeCount(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, fieldName, value);
            if (treeCount < 1) { return; }

            var treeID = GetFixCNTTallyTreeID(unitCode, plotNumber,
                stratumCode, sgCode, species, liveDead,
                fieldName, value);

            if (treeID != null)
            {
                var tallyLedgerID = Guid.NewGuid();

                Database.Execute2(
@"INSERT INTO TallyLedger (
    TallyLedgerID,
    TreeID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    EntryType,
    TreeCount,
    CreatedBy
) VALUES (
    @TallyLedgerID,
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    'tally',
    -1,
    @CreatedBy
);",
                    new
                    {
                        CruiseID,
                        TallyLedgerID = tallyLedgerID,
                        TreeID = treeID,
                        CuttingUnitCode = unitCode,
                        PlotNumber = plotNumber,
                        StratumCode = stratumCode,
                        SampleGroupCode = sgCode,
                        SpeciesCode = species,
                        LiveDead = liveDead,
                        CreatedBy = DeviceID,
                    });
            }
        }

        public void AddFixCNTTree(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeID = CreateFixCNTTallyTree(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, fieldName, value);

            var tallyLedgerID = Guid.NewGuid();

            Database.Execute2(
@"INSERT INTO TallyLedger (
    TallyLedgerID,
    TreeID,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    EntryType,
    TreeCount,
    CreatedBy
) VALUES (
    @TallyLedgerID,
    @TreeID,
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    'tally',
    1, -- TreeCount
    @CreatedBy
);",
                new
                {
                    CruiseID,
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = species,
                    LiveDead = liveDead,
                    CreatedBy = DeviceID,
                });
        }

        public void RemoveFixCNTTree(string unitCode, int plotNumber, string stratumCode,
            string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var treeID = GetFixCNTTallyTreeID(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, fieldName, value);

            Database.Execute("DELETE FROM Tree WHERE TreeID = @p1;", treeID);
        }

        protected string CreateFixCNTTallyTree(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            if (string.IsNullOrEmpty(unitCode)) { throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty", nameof(unitCode)); }
            if (string.IsNullOrEmpty(stratumCode)) { throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty", nameof(stratumCode)); }
            if (string.IsNullOrEmpty(sgCode)) { throw new ArgumentException($"'{nameof(sgCode)}' cannot be null or empty", nameof(sgCode)); }
            if (string.IsNullOrEmpty(species)) { throw new ArgumentException($"'{nameof(species)}' cannot be null or empty", nameof(species)); }
            if (string.IsNullOrEmpty(liveDead)) { throw new ArgumentException($"'{nameof(liveDead)}' cannot be null or empty", nameof(liveDead)); }
            if (string.IsNullOrEmpty(fieldName)) { throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or empty", nameof(fieldName)); }

            var treeID = Guid.NewGuid().ToString();

            var fieldNameStr = fieldName.ToString();

            Database.Execute2(
$@"INSERT INTO Tree (
    TreeID,
    TreeNumber,
    CruiseID,
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    CountOrMeasure,
    CreatedBy
) VALUES (
    @TreeID,
    (SELECT ifnull(max(TreeNumber), 0) + 1
        FROM Tree AS t1
        WHERE CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber), -- get highest tree number using unitCode and plot_cn
    @CruiseID,
    @CuttingUnitCode,
    @PlotNumber,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    'C', -- countMeasure
    @CreatedBy
);

INSERT INTO TreeMeasurment
(TreeID, {fieldNameStr}) VALUES (@TreeID, @value);",
                new
                {
                    CruiseID,
                    TreeID = treeID,
                    TallyLedgerID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = species,
                    LiveDead = liveDead,
                    value,
                    CreatedBy = DeviceID,
                }
            );

            return treeID;
        }

        protected string GetFixCNTTallyTreeID(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            return Database.ExecuteScalar<string>(
$@"SELECT
    t.TreeID
FROM Tree AS t
JOIN TreeMeasurment AS tm USING (TreeID)
WHERE tm.{fieldName} = @p1
    AND t.PlotNumber = @p2
    AND t.CuttingUnitCode = @p3
    AND t.StratumCode = @p4
    AND t.SampleGroupCode = @p5
    AND ifnull(t.SpeciesCode, '') = ifnull(@p6, '')
    AND ifnull(t.LiveDead, '') = ifnull(@p7, '')
    AND CruiseID = @p8
LIMIT 1;",
                 value, plotNumber, unitCode, stratumCode, sgCode, species, liveDead, CruiseID);
        }

        public IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode)
        {
            return Database.Query<FixCntTallyPopulation>(
@"SELECT
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    FixCNTField AS Field,
    Min,
    Max,
    IntervalSize
FROM FixCNTTallyPopulation AS ftp
JOIN Stratum AS tc USING (StratumCode, CruiseID)
WHERE StratumCode = @p1 AND CruiseID = @p2;",
                new object[] { stratumCode, CruiseID });
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
@"SELECT Sum(tl.TreeCount)
    FROM TallyLedger AS tl
JOIN TreeFieldValue_All AS tfv USING (TreeID)
WHERE   CruiseID = @CruiseID
        AND tl.CuttingUnitCode = @CuttingUnitCode
        AND tl.PlotNumber = @PlotNumber
        AND tl.StratumCode = @StratumCode
        AND tl.SampleGroupCode = @SampleGroupCode
        AND tl.SpeciesCode = @SpeciesCode
        AND tl.LiveDead = @LiveDead
        AND tfv.Field = @Field
        AND tfv.ValueReal = @Value;",
                new
                {
                    CruiseID,
                    CuttingUnitCode = unit,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    SpeciesCode = species,
                    LiveDead = livedead,
                    Field = field,
                    Value = value,
                });
        }
    }
}