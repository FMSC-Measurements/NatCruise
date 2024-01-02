using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public class SubpopulationDataservice : CruiseDataserviceBase, ISubpopulationDataservice
    {
        public SubpopulationDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public SubpopulationDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public SubpopulationDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public void AddSubpopulation(Subpopulation subpopulation)
        {
            subpopulation.SubpopulationID ??= Guid.NewGuid().ToString();

            Database.Execute2(
@"INSERT OR IGNORE INTO Species (SpeciesCode, CruiseID) VALUES (@SpeciesCode, @CruiseID);

INSERT INTO Subpopulation (
    CruiseID,
    SubpopulationID,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    CreatedBy
) VALUES (
    @CruiseID,
    @SubpopulationID,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @DeviceID
);", new
{
    CruiseID,
    subpopulation.SubpopulationID,
    subpopulation.StratumCode,
    subpopulation.SampleGroupCode,
    subpopulation.SpeciesCode,
    subpopulation.LiveDead,
    DeviceID,
});
        }

        public void DeleteSubpopulation(Subpopulation subpopulation)
        {
            Database.Execute("DELETE FROM Subpopulation WHERE SubpopulationID = @p1;", subpopulation.SubpopulationID);
        }

        public bool Exists(string stratumCode, string sampleGroupCode, string species, string livedead = null)
        {
            return Database.ExecuteScalar<int>(
@"SELECT count(*) FROM Subpopulation
            WHERE StratumCode = @p1
            AND SampleGroupCode = @p2
            AND SpeciesCode = @p3
            AND (@p4 IS NULL OR LiveDead = @p4)
            AND CruiseID = @p5;",
                stratumCode, sampleGroupCode, species, livedead, CruiseID) > 0;
        }

        public IEnumerable<Subpopulation> GetSubpopulations(string stratumCode, string sampleGroupCode)
        {
            if (stratumCode is null) { throw new ArgumentNullException(nameof(stratumCode)); }

            return Database.Query<Subpopulation>(
@"SELECT

    sp.SubPopulationID,
    sp.CruiseID,
    sp.StratumCode,
    sp.SampleGroupCode,
    sp.SpeciesCode,
    sp.LiveDead,
    fctp.IntervalSize,
    fctp.Min,
    fctp.Max,
    (
        EXISTS (SELECT * FROM Tree AS t
        WHERE t.CruiseID = sp.CruiseID
        AND t.StratumCode = sp.StratumCode
        AND t.SampleGroupCode = sp.SampleGroupCode
        AND t.SpeciesCode = sp.SpeciesCode
        AND t.LiveDead = sp.LiveDead)
    ) AS HasTrees
FROM SubPopulation AS sp
LEFT JOIN FixCNTTallyPopulation AS fctp USING (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead)
WHERE StratumCode = @p1 AND (@p2 IS NULL OR SampleGroupCode = @p2) AND CruiseID = @p3;",
                stratumCode, sampleGroupCode, CruiseID);
        }

        public bool HasTreeCounts(string stratumCode, string sampleGroupCode, string species, string livedead)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger WHERE StratumCode = @p1 AND SampleGroupCode = @p2 AND SpeciesCode = @p3 AND LiveDead = @p4 AND CruiseID = @p4;",
                stratumCode, sampleGroupCode, species, livedead, CruiseID) > 0;
        }

        public void UpdateSubpopulation(Subpopulation subpopulation)
        {
            Database.Execute2(
@"UPDATE Subpopulation SET
    LiveDead =  @LiveDead
WHERE SubpopulationID = @SubpopulationID;", subpopulation);
        }

        public void UpsertFixCNTTallyPopulation(Subpopulation subpopulation)
        {
            Database.Execute2(
@"INSERT INTO FixCNTTallyPopulation (
    CruiseID,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    IntervalSize,
    Min,
    Max,
    CreatedBy
) VALUES (
    @CruiseID,
    @StratumCode,
    @SampleGroupCode,
    @SpeciesCode,
    @LiveDead,
    @IntervalSize,
    @Min,
    @Max,
    @DeviceID
)
ON CONFLICT (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead) DO
UPDATE SET
    IntervalSize = @IntervalSize,
    Min = @Min,
    Max = @Max,
    ModifiedBy = @DeviceID
WHERE CruiseID = @CruiseID
    AND StratumCode = @StratumCode
    AND SampleGroupCode = @SampleGroupCode
    AND SpeciesCode = @SpeciesCode
    AND LiveDead = @LiveDead;", new
{
    CruiseID,
    subpopulation.StratumCode,
    subpopulation.SampleGroupCode,
    subpopulation.SpeciesCode,
    subpopulation.LiveDead,
    subpopulation.IntervalSize,
    subpopulation.Min,
    subpopulation.Max,
    DeviceID,
});
        }
    }
}